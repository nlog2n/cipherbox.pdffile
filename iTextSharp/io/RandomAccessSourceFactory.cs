using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using iTextSharp.text.error_messages;

namespace iTextSharp.text.io 
{

    /**
     * Factory to create {@link RandomAccessSource} objects based on various types of sources
     * @since 5.3.5
     *
     */

    public sealed class RandomAccessSourceFactory 
    {

        /**
         * whether the full content of the source should be read into memory at construction
         */
        private bool forceRead = false;
        
        /**
         * Whether {@link RandomAccessFile} should be used instead of a {@link FileChannel}, where applicable
         */
        //private bool usePlainRandomAccess = false;
        
        /**
         * Whether the underlying file should have a RW lock on it or just an R lock
         */
        private bool exclusivelyLockFile = false;

        /**
         * Creates a factory that will give preference to accessing the underling data source using memory mapped files
         */
        public RandomAccessSourceFactory() {
        }
        
        /**
         * Determines whether the full content of the source will be read into memory
         * @param forceRead true if the full content will be read, false otherwise
         * @return this object (this allows chaining of method calls)
         */
        public RandomAccessSourceFactory SetForceRead(bool forceRead){
            this.forceRead = forceRead;
            return this;
        }
        
        public RandomAccessSourceFactory SetExclusivelyLockFile(bool exclusivelyLockFile){
            this.exclusivelyLockFile = exclusivelyLockFile;
            return this;
        }

        /**
         * Creates a {@link RandomAccessSource} based on a byte array
         * @param data the byte array
         * @return the newly created {@link RandomAccessSource}
         */
        public IRandomAccessSource CreateSource(byte[] data){
            return new ArrayRandomAccessSource(data); 
        }
        
        public IRandomAccessSource CreateSource(FileStream raf) {
            return new RAFRandomAccessSource(raf); 
        }
        
        /**
         * Creates a {@link RandomAccessSource} based on a URL.  The data available at the URL is read into memory and used
         * as the source for the {@link RandomAccessSource}
         * @param url the url to read from
         * @return the newly created {@link RandomAccessSource}
         */
        public IRandomAccessSource CreateSource(Uri url) {
            WebRequest wr = WebRequest.Create(url);
            wr.Credentials = CredentialCache.DefaultCredentials;
            Stream isp = wr.GetResponse().GetResponseStream();
            try {
                return CreateSource(isp);
            }
            finally {
                try {isp.Close();}catch{}
            }
        }
        
        /**
         * Creates a {@link RandomAccessSource} based on an {@link InputStream}.  The full content of the InputStream is read into memory and used
         * as the source for the {@link RandomAccessSource}
         * @param is the stream to read from
         * @return the newly created {@link RandomAccessSource}
         */
        public IRandomAccessSource CreateSource(Stream inp) {
           try {
                return CreateSource(StreamUtil.InputStreamToArray(inp));
            }
            finally {
                try {inp.Close();}catch{}
            }       
        }
        
        /**
         * Creates a {@link RandomAccessSource} based on a filename string.
         * If the filename describes a URL, a URL based source is created
         * If the filename describes a file on disk, the contents may be read into memory (if forceRead is true), opened using memory mapped file channel (if usePlainRandomAccess is false), or opened using {@link RandomAccessFile} access (if usePlainRandomAccess is true)
         * This call will automatically failover to using {@link RandomAccessFile} if the memory map operation fails
         * @param filename the name of the file or resource to create the {@link RandomAccessSource} for
         * @return the newly created {@link RandomAccessSource}
         */
        public IRandomAccessSource CreateBestSource(String filename) 
        {
            if (!File.Exists(filename)) 
            {
                if (filename.StartsWith("file:/")
                        || filename.StartsWith("http://") 
                        || filename.StartsWith("https://")) 
                {
                    return CreateSource(new Uri(filename));
                } else 
                {
                    return CreateByReadingToMemory(filename);
                }
            }
                
            if (forceRead)
            {
                return CreateByReadingToMemory(new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read));
            }
            return new RAFRandomAccessSource(new FileStream(filename, FileMode.Open, FileAccess.Read, exclusivelyLockFile ? FileShare.None : FileShare.Read));
        }
        
        public IRandomAccessSource CreateRanged(IRandomAccessSource source, IList<long> ranges) {
            IRandomAccessSource[] sources = new IRandomAccessSource[ranges.Count/2];
            for(int i = 0; i < ranges.Count; i+=2){
                sources[i/2] = new WindowRandomAccessSource(source, ranges[i], ranges[i+1]);
            }
            return new GroupedRandomAccessSource(sources);
        }
        
        /**
         * Creates a new {@link RandomAccessSource} by reading the specified file/resource into memory
         * @param filename the name of the resource to read
         * @return the newly created {@link RandomAccessSource}
         * @throws IOException if reading the underling file or stream fails
         */
        private IRandomAccessSource CreateByReadingToMemory(String filename) {
            //TODO: seems odd that we are using BaseFont here...
            Stream inp = StreamUtil.GetResourceStream(filename);
            if (inp == null)
                throw new IOException(MessageLocalization.GetComposedMessage("1.not.found.as.file.or.resource", filename));
            return CreateByReadingToMemory(inp);
        }
        
        /**
         * Creates a new {@link RandomAccessSource} by reading the specified file/resource into memory
         * @param filename the name of the resource to read
         * @return the newly created {@link RandomAccessSource}
         * @throws IOException if reading the underling file or stream fails
         */
        private IRandomAccessSource CreateByReadingToMemory(Stream inp) {
            try {
                return new ArrayRandomAccessSource(StreamUtil.InputStreamToArray(inp));
            }
            finally {
                try {inp.Close();}catch{}
            }
        }
    }
}