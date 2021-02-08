using System;

/*
 * This program is based on zlib-1.1.3, so all credit should go authors
 * Jean-loup Gailly(jloup@gzip.org) and Mark Adler(madler@alumni.caltech.edu)
 * and contributors of zlib.
 */

namespace CipherBox.Pdf.Utility.Zlib {

    public sealed class JZlib{
        private const String _version="1.0.7";
        public static String version()
		{
			return _version;
		}

        // compression levels
        public const int Z_NO_COMPRESSION=0;
        public const int Z_BEST_SPEED=1;
        public const int Z_BEST_COMPRESSION=9;
        public const int Z_DEFAULT_COMPRESSION=-1;

        // compression strategy
        public const int Z_FILTERED=1;
        public const int Z_HUFFMAN_ONLY=2;
        public const int Z_DEFAULT_STRATEGY=0;

        public const int Z_NO_FLUSH=0;
        public const int Z_PARTIAL_FLUSH=1;
        public const int Z_SYNC_FLUSH=2;
        public const int Z_FULL_FLUSH=3;
        public const int Z_FINISH=4;

        public const int Z_OK=0;
        public const int Z_STREAM_END=1;
        public const int Z_NEED_DICT=2;
        public const int Z_ERRNO=-1;
        public const int Z_STREAM_ERROR=-2;
        public const int Z_DATA_ERROR=-3;
        public const int Z_MEM_ERROR=-4;
        public const int Z_BUF_ERROR=-5;
        public const int Z_VERSION_ERROR=-6;
    }
}