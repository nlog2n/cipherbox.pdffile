/*
 * This program is based on zlib-1.1.3, so all credit should go authors
 * Jean-loup Gailly(jloup@gzip.org) and Mark Adler(madler@alumni.caltech.edu)
 * and contributors of zlib.
 */
/* This file is a port of jzlib v1.0.7, com.jcraft.jzlib.ZOutputStream.java
 */

using System;
using System.Diagnostics;
using System.IO;

namespace CipherBox.Pdf.Utility.Zlib {
    public class ZOutputStream : Stream
	{
		private const int BufferSize = 512;

		protected ZStream z = new ZStream();
		protected int flushLevel = JZlib.Z_NO_FLUSH;
		// TODO Allow custom buf
		protected byte[] buf = new byte[BufferSize];
		protected byte[] buf1 = new byte[1];
		protected bool compress;

		protected Stream output;
		protected bool closed;

		public ZOutputStream(Stream output)
			: base()
		{
			Debug.Assert(output.CanWrite);

			this.output = output;
			this.z.inflateInit();
			this.compress = false;
		}

		public ZOutputStream(Stream output, int level)
			: this(output, level, false)
		{
		}

		public ZOutputStream(Stream output, int level, bool nowrap)
			: base()
		{
			Debug.Assert(output.CanWrite);

			this.output = output;
			this.z.deflateInit(level, nowrap);
			this.compress = true;
		}

		public sealed override bool CanRead { get { return false; } }
        public sealed override bool CanSeek { get { return false; } }
        public sealed override bool CanWrite { get { return !closed; } }

		public override void Close()
		{
			if (this.closed)
				return;

			try
			{
				try
				{
					Finish();
				}
				catch (IOException)
				{
					// Ignore
				}
			}
			finally
			{
				this.closed = true;
				End();
				output.Close();
				output = null;
			}
		}

		public virtual void End()
		{
			if (z == null)
				return;
			if (compress)
				z.deflateEnd();
			else
				z.inflateEnd();
			z.free();
			z = null;
		}

		public virtual void Finish()
		{
			do
			{
				z.next_out = buf;
				z.next_out_index = 0;
				z.avail_out = buf.Length;

				int err = compress
					?	z.deflate(JZlib.Z_FINISH)
					:	z.inflate(JZlib.Z_FINISH);

				if (err != JZlib.Z_STREAM_END && err != JZlib.Z_OK)
					// TODO
//					throw new ZStreamException((compress?"de":"in")+"flating: "+z.msg);
					throw new IOException((compress ? "de" : "in") + "flating: " + z.msg);

				int count = buf.Length - z.avail_out;
				if (count > 0)
				{
					output.Write(buf, 0, count);
				}
			}
			while (z.avail_in > 0 || z.avail_out == 0);

			Flush();
		}

		public override void Flush()
		{
			output.Flush();
		}

		public virtual int FlushMode
		{
			get { return flushLevel; }
			set { this.flushLevel = value; }
		}

        public sealed override long Length { get { throw new NotSupportedException(); } }
        public sealed override long Position
        {
            get { throw new NotSupportedException(); }
            set { throw new NotSupportedException(); }
        }
        public sealed override int Read(byte[] buffer, int offset, int count) { throw new NotSupportedException(); }
        public sealed override long Seek(long offset, SeekOrigin origin) { throw new NotSupportedException(); }
        public sealed override void SetLength(long value) { throw new NotSupportedException(); }

		public virtual long TotalIn
		{
			get { return z.total_in; }
		}

		public virtual long TotalOut
		{
			get { return z.total_out; }
		}

		public override void Write(byte[] b, int off, int len)
		{
			if (len == 0)
				return;

			z.next_in = b;
			z.next_in_index = off;
			z.avail_in = len;

			do
			{
				z.next_out = buf;
				z.next_out_index = 0;
				z.avail_out = buf.Length;

				int err = compress
					?	z.deflate(flushLevel)
					:	z.inflate(flushLevel);

				if (err != JZlib.Z_OK)
					// TODO
//					throw new ZStreamException((compress ? "de" : "in") + "flating: " + z.msg);
					throw new IOException((compress ? "de" : "in") + "flating: " + z.msg);

				output.Write(buf, 0, buf.Length - z.avail_out);
			}
			while (z.avail_in > 0 || z.avail_out == 0);
		}

		public override void WriteByte(byte b)
		{
			buf1[0] = b;
			Write(buf1, 0, 1);
		}
	}
}
