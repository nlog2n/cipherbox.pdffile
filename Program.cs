/*
 *  test password verification, stream read/write
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Cryptography;

using CipherBox.Pdf;


namespace CipherBox.Pdf.Test
{
    partial class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Help:");
                Console.WriteLine(" program show    pdffile           - show enc info");
                Console.WriteLine(" program verify  pdffile password  - verify password");
                Console.WriteLine(" program lock    pdffile password  - lock file");
                Console.WriteLine(" program unlock  pdffile password  - unlock file");
                Console.WriteLine(" program stamp   pdffile watermark - add watermark");
                Console.WriteLine(" program unstamp pdffile           - remove watermark");
                Console.WriteLine(" program layers  pdffile           - view layers");
                Console.WriteLine(" program parse   pdffile [pagenum] - parse content");
                return;
            }

            string cmd = "show";
            string filename = "test.pdf";
            string password = "password";
            if (args.Length > 0) { cmd = args[0]; }
            if (args.Length > 1) { filename = args[1]; }
            if (args.Length > 2) { password = args[2]; }


            if (args.Length == 2 && cmd == "show")
            {
                string dis = PDFHelper.GetEncryptionInfo(filename);
                Console.WriteLine(dis);
                return;
            }

            if (cmd == "parse")  // for reading plain PDF content text only
            {
                if (args.Length != 2 && args.Length != 3)
                {
                    Console.Out.WriteLine("Usage:  program parse pdffile [pagenum]");
                    return;
                }

                TextWriter writer = Console.Out;
                int pageNum = -1; if (args.Length >= 3) { pageNum = int.Parse(args[2]); }

                PdfContentReaderTool.ShowPage(filename, pageNum, writer);

                writer.Flush();
                writer.Close();
                return;
            }

            if (args.Length == 3 && cmd == "verify")
            {
                if (PDFHelper.VerifyPassword(filename, password))
                {
                    Console.WriteLine("success");                    
                }
                else
                {
                    Console.WriteLine("fail");
                }
                return;
            }

            if (args.Length == 3 && cmd == "lock")
            {
                PDFHelper.AddPassword(filename, password);
            }

            if (args.Length == 3 && cmd == "unlock")
            {
                PDFHelper.RemovePassword(filename, password);
            }

            if (args.Length == 3 && cmd == "stamp")
            {
                string watermark = password;
                PDFHelper.AddWaterMark(filename, watermark, null);
            }

            if (args.Length == 2 && cmd == "unstamp")
            {
                PDFHelper.RemoveWaterMarks(filename);
            }

            if (args.Length == 2 && cmd == "layers")
            {
                Dictionary<string,bool> layernames = PDFHelper.GetWaterMarks(filename);
                Console.WriteLine("removable PDF layers: ");
                foreach (string rem in layernames.Keys)
                {
                    Console.WriteLine("\t" + rem);
                }
            }

        }


    }
}
