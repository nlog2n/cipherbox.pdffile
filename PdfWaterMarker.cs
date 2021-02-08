using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

// 1. Add to an already existing pdf with watermark text as PdfLayer
// 2. Check, whether the document already has this sign
// 3. Remove the sign

// Note: the iText PdfLayer is called optional content groups in Adobe PDF document.

using iTextSharp.text;
using iTextSharp.text.pdf;

namespace CipherBox.Pdf
{
    public static partial class PDFHelper
    {
        #region Add/remove watermark texts

        // result example: <Xobject,Watermark,> which is an xobject
        //                 <Layer,CipherboxWatermark,sample> which is a layer in page contents
        private static Dictionary<string, bool> CheckPages(PdfReader reader, PdfStamper stamper)
        {
            Dictionary<string, bool> result = new Dictionary<string, bool>(); //
            for (int i = 0; i < reader.NumberOfPages; i++)
            {
                //Get the page
                int pageno = i + 1; // index from 1
                PdfDictionary page = reader.GetPageN(pageno);

                // parse the page
                OCGParser parser = new OCGParser(page);

                // consolidate the result
                Dictionary<string, bool> result_page = parser.GetRemoveFilter();
                foreach (string rem in result_page.Keys)
                {
                    if (!result.ContainsKey(rem))
                    {
                        result[rem] = true;
                    }
                }
            }
            return result;
        }



        public static Dictionary<string, bool> GetWaterMarks(string filename)
        {
            Dictionary<string, bool> result = new Dictionary<string, bool>();

            try
            {
                // PdfReader reader = new PdfReader(filename);
                byte[] docBytes = File.ReadAllBytes(filename);
                PdfReader reader = new PdfReader(docBytes); 

                using (MemoryStream ms = new MemoryStream())
                {
                    using (PdfStamper stamper = new PdfStamper(reader, ms))
                    {
                        /*
                        // check all OC layers
                        // Note: GetPdfLayers() will re-read document, which may cause little trouble
                        Dictionary<string, PdfLayer> layers = stamper.GetPdfLayers();
                        if (layers != null && layers.Count != 0)
                        {
                            foreach (string k in layers.Keys)
                            {
                                if (k == CipherboxWaterMarkLayerName)
                                {
                                    // disable visibility
                                    // Note: this method is not easy to modify watermark text in reference pdf object.
                                    //layers[k].On = false;
                                    //layers[k].OnPanel = false;
                                }

                                result.Add(k);
                            }
                        }
                        */

                        result = CheckPages(reader, stamper);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("PDF read OCG layer error: " + ex.Message);
            }

            return result;
        }

        // Example: add watermark on undercontent,and add page number on overcontent
        // http://www.pdfhome.com.cn/Article.aspx?CID=bf51a5b6-78a5-4fa3-9310-16e04aee8c78&AID=4c0f4d57-12e8-4b06-869e-65aac77dde41
        public static void AddWaterMark(string filename, PdfWaterMarkOption wm)
        {
            if (wm == null) return;
            if (wm.Text == null && wm.ImageFileName == null) return;

            try
            {
                byte[] docBytes = File.ReadAllBytes(filename);
                PdfReader reader = new PdfReader(docBytes);
                // PdfReader reader = new PdfReader(filename);

                byte[] docNewBytes = null;
                using (MemoryStream ms = new MemoryStream())
                {
                    using (PdfStamper stamper = new PdfStamper(reader, ms))
                    {
                        // do watermarking on a separate layer

                        // get image file
                        Image img = null;
                        if (!string.IsNullOrEmpty(wm.ImageFileName))
                        {
                            img = Image.GetInstance(wm.ImageFileName); // "images/watermark.jpg"
                        }

                        // Working with iTextSharp Fonts: http://www.mikesdotnetting.com/Article/81/iTextSharp-Working-with-Fonts
                        BaseFont bf = null;
                        switch (wm.TextFont)
                        {
                            case "COURIER":
                                bf = BaseFont.CreateFont(BaseFont.COURIER, BaseFont.WINANSI, BaseFont.NOT_EMBEDDED);
                                //BaseFont bf = FontFactory.GetFont(FontFactory.COURIER).GetCalculatedBaseFont(false)
                                break;
                            case "STSong-Light":   // support chinese characters
                                //////bf = FontFactory.GetFont("MSung-Light", "UniCNS-UCS2-H", BaseFont.NOT_EMBEDDED).GetCalculatedBaseFont(false); 
                                bf = new CJKFont("STSong-Light", "UniGB-UTF16-H", BaseFont.NOT_EMBEDDED);
                                break;

                            case "HELVETICA":
                                bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.WINANSI, BaseFont.NOT_EMBEDDED);
                                break;
                            default:
                                bf = BaseFont.CreateFont();  // equal to HELVETICA above
                                break;
                        }

                        // Create a new separate layer, PdfLayer, which is an OCG or Optional Content Group.
                        PdfLayer layer = new PdfLayer(OCGRemover.CipherboxWaterMarkLayerName, stamper.Writer);
                        {
                            //layer.On = true;
                            //layer.OnPanel = true;
                            //layer.View = true;
                        }

                        // Apply watermark to each page
                        for (int i = 0; i < reader.NumberOfPages; i++)
                        {
                            int pageno = i + 1; // index from 1
                            if (pageno < wm.PageStart || pageno > wm.PageEnd) continue;

                            // Note: the watermark may be hidden by overlay content.
                            // so we choose to write watermark on top of content instead of underneath
                            PdfContentByte canvas = stamper.GetOverContent(pageno); // stamper.GetUnderContent(pageno)

                            // get location
                            Rectangle rect = reader.GetPageSize(pageno);

                            // set transparency
                            PdfGState gState = new PdfGState();
                            gState.FillOpacity = wm.Opacity;
                            //gState.StrokeOpacity = wm.Opacity; 
                            canvas.SetGState(gState);

                            // add image to underlying canvas
                            if (img != null)  
                            {
                                img.ScalePercent(wm.ImageScalePercentage); // 0-100
                                img.RotationDegrees = wm.Rotation;
                                //img.Transparency = new int[] { 10, 10 };
                                img.SetAbsolutePosition((rect.Width - img.ScaledWidth) / 2, (rect.Height - img.ScaledHeight) / 2);

                                // create an Xobject for this image. can be also put inside a layer
                                canvas.AddImage(img);
                            }

                            if (!string.IsNullOrEmpty(wm.Text))
                            {
                                //Tell the CB that the next commands should be "bound" to this new layer
                                canvas.BeginLayer(layer); // still works if without layer setup
                                {
                                    // add watermark text to overlay content
                                    {
                                        canvas.BeginText();
                                        canvas.SetFontAndSize(bf, wm.TextFontSize);
                                        switch (wm.TextColor)
                                        {
                                            case "BLACK": canvas.SetColorFill(BaseColor.BLACK); break;
                                            case "BLUE": canvas.SetColorFill(BaseColor.BLUE); break;
                                            case "GREEN": canvas.SetColorFill(BaseColor.GREEN); break;
                                            case "YELLOW": canvas.SetColorFill(BaseColor.YELLOW); break;
                                            case "RED": canvas.SetColorFill(BaseColor.RED); break;
                                            default: canvas.SetColorFill(BaseColor.RED); break;
                                        }
                                        //canvas.SetTextMatrix(250, 500); //(30, 30) is bottom;
                                        //canvas.ShowText(wmtext);
                                        canvas.ShowTextAligned(PdfContentByte.ALIGN_CENTER, wm.Text, rect.Width / 2, rect.Height / 2, wm.Rotation);
                                        canvas.EndText();
                                    }

                                    // draw something
                                    ///canvas.SetRGBColorStroke(0xFF, 0x00, 0x00);
                                    ///canvas.SetLineWidth(5f);
                                    ///canvas.Ellipse(250, 450, 350, 550);
                                    ///canvas.Stroke();
                                }
                                canvas.EndLayer(); //"Close" the layer
                            }
                        }
                    }
                    docNewBytes = ms.ToArray();
                }
                if (docNewBytes != null)
                {
                    File.WriteAllBytes(filename, docNewBytes);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("PDF add watermark error: " + ex.Message);
            }
        }

        public static void AddWaterMark(string filename, string wmtext, string picfilename)
        {
            PdfWaterMarkOption wm = new PdfWaterMarkOption(wmtext, picfilename);
            AddWaterMark(filename, wm);
        }

        // remove watermarks created by cipherbox
        // refer to: http://stackoverflow.com/questions/8768130/removing-watermark-from-a-pdf-using-itextsharp
        public static void RemoveWaterMarks(string filename)
        {
            try
            {
                byte[] docBytes = File.ReadAllBytes(filename);
                PdfReader reader = new PdfReader(docBytes);
                // PdfReader reader = new PdfReader(filename);

                byte[] docNewBytes = null;
                using (MemoryStream ms = new MemoryStream())
                {
                    using (PdfStamper stamper = new PdfStamper(reader, ms))
                    {
                        OCGRemover remover = new OCGRemover();
                        remover.Remove(reader);
                    }
                    docNewBytes = ms.ToArray();
                }
                if (docNewBytes != null)
                {
                    File.WriteAllBytes(filename, docNewBytes);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("PDF remove watermark error: " + ex.Message);
            }
        }


        public static void RemoveWaterMarks(string filename, Dictionary<string, bool> filter)
        {
            try
            {
                List<string> ft = new List<string>();
                foreach (string x in filter.Keys)
                {
                    if (filter[x])
                    {
                        ft.Add(x);
                    }
                }

                byte[] docBytes = File.ReadAllBytes(filename);
                PdfReader reader = new PdfReader(docBytes);
                // PdfReader reader = new PdfReader(filename);

                byte[] docNewBytes = null;
                using (MemoryStream ms = new MemoryStream())
                {
                    using (PdfStamper stamper = new PdfStamper(reader, ms))
                    {
                        OCGRemover remover = new OCGRemover();
                        remover.RemoveByFilter(reader, ft);
                    }
                    docNewBytes = ms.ToArray();
                }
                if (docNewBytes != null)
                {
                    File.WriteAllBytes(filename, docNewBytes);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("PDF remove watermark error: " + ex.Message);
            }
        }

        #endregion

    }
}