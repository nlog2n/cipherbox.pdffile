using System;
using System.Collections.Generic;
using System.IO;

using iTextSharp.text.pdf;
using CipherBox.Pdf.Parser;

namespace CipherBox.Pdf
{
    /// <summary>
    /// A helper class for OCGRemover, focusing on each page
    /// </summary>
    public class OCGParser
    {
        #region Data for internal parsing
        private PdfDictionary _page;
        private PdfDictionary _properties; // The OCG properties
        private PdfDictionary _xobjects;
        private List<PdfName> _xobjects_to_remove; // The names of page xobjects that shouldn't be shown

        private static MemoryStream baos; // The OutputStream of this worker object
        private int mc_balance; // Keeps track of BMC/EMC balance
        private int page_container_index;
        #endregion

        // Setting
        public bool ReadOnly = false; // no write on page

        // Input
        private List<string> LayersToRemove; // The input OCGs that need to be removed
        private Dictionary<int, string> FilterToRemove; // The input keywords that remove OCG layers

        // Output
        // The containers in page contents that should't be shown, <OC/Artifact, layer name, wmtext>
        public Dictionary<int, string> RemovablePageContainers;
        public Dictionary<PdfName, string> RemovableXObjects;


        /// <summary>
        /// Create an instance of OCGParser for single page
        /// </summary>
        /// <param name="page">one page</param>
        public OCGParser(PdfDictionary page)
        {
            this._page = page;

            this.ReadOnly = true;

            // get the resources dictionary of that page (containing info about the OCGs)
            PdfDictionary resources = this._page.GetAsDict(PdfName.RESOURCES);
            this._properties = resources.GetAsDict(PdfName.PROPERTIES); // set value
            this._xobjects = resources.GetAsDict(PdfName.XOBJECT);

            // parse page xobjects
            this.RemovableXObjects = ParsePageXobjects(this._xobjects);

            // parse the page stream and get layer text. no remove OCGs at this time
            ParsePageContents(this._page);
        }

        /// <summary>
        /// Set a filter for removing OCG layers
        /// </summary>
        /// <param name="layers">OCG layer names for removed</param>
        public void SetLayersToRemove(List<string> layers)
        {
            if (this._xobjects_to_remove == null)
            {
                this._xobjects_to_remove = new List<PdfName>();
            }

            if (layers == null) return;
            this.LayersToRemove = layers;

            List<string> xobjnames = new List<string>(); // key = xbojname, value= layername

            foreach (string x in this.RemovablePageContainers.Values)
            {
                string[] kkk = x.Split(',');
                if (kkk.Length == 3 && kkk[2].IndexOf('/') == 0) // point to xobject
                {
                    if (layers.Contains(kkk[1])) // the 2nd keyword is for layer name
                    {
                        xobjnames.Add( kkk[2] );  
                    }
                }
            }

            foreach (PdfName name in this.RemovableXObjects.Keys)
            {
                string x = this.RemovableXObjects[name];
                string[] kkk = x.Split(',');
                if (kkk.Length == 3 && kkk[1] == "Image")
                {
                    if (layers.Contains(kkk[1]))
                    {
                        xobjnames.Add(name.ToString());
                    }
                }
            }

            if (this._xobjects != null)
            {
                foreach (PdfName x in this._xobjects.Keys)
                {
                    if (xobjnames.Contains(x.ToString()))
                    {
                        this._xobjects_to_remove.Add(x); // translated into PdfName like '/Fm0'
                    }
                }
            }
        }

        public void SetXObjectsToRemove(List<string> filter)
        {
            if (this._xobjects_to_remove == null)
            {
                this._xobjects_to_remove = new List<PdfName>();
            }

            if (filter == null) return;

            // must find out the corresponding indices for containers
            this.FilterToRemove = new Dictionary<int, string>();
            foreach (int i in this.RemovablePageContainers.Keys)
            {
                string x = this.RemovablePageContainers[i];
                if (filter.Contains(x))
                {
                    this.FilterToRemove[i] = x;
                }
            }

            List<string> xobjnames = new List<string>();
            foreach (string x in filter)
            {
                string[] kkk = x.Split(',');
                if (kkk.Length == 3 && kkk[0] == "Artifact" && kkk[2].IndexOf('/') == 0)
                {
                    xobjnames.Add(kkk[2]);
                }
            }

            foreach (PdfName name in this.RemovableXObjects.Keys)
            {
                string x = this.RemovableXObjects[name];
                string[] kkk = x.Split(',');

                if (kkk.Length == 3 && kkk[1] == "Image")
                {
                    if (filter.Contains(x))
                    {
                        xobjnames.Add(name.ToString());
                    }
                }
            }

            if (this._xobjects != null)
            {
                foreach (PdfName x in this._xobjects.Keys)
                {
                    if (xobjnames.Contains(x.ToString()))
                    {
                        this._xobjects_to_remove.Add(x); // translated into PdfName like '/Fm0'
                    }
                }
            }
        }


        public Dictionary<string, bool> GetRemoveFilter()
        {
            Dictionary<string, bool> result = new Dictionary<string, bool>();

            foreach (string rem in this.RemovablePageContainers.Values)
            {
                result[rem] = true;
            }

            foreach (string rem in this.RemovableXObjects.Values)
            {
                result[rem] = true;
            }
            
            return result;
        }



        /// <summary>
        /// Uses the OCGParser on a page </summary>
        /// <param name="parser">	the OCGParser </param>
        /// <param name="page">		the page dictionary of the page that needs to be parsed. </param>
        /// <exception cref="IOException"> </exception>
        public void Remove()
        {
            this.ReadOnly = false;

            foreach (PdfName name in this._xobjects_to_remove)
            {
                //Console.WriteLine("\txobject to be removed: " + name);
                this._xobjects.Remove(name);
            }

            // parse the page stream again and remove OCGs
            ParsePageContents(this._page);

            this._page.Remove(new PdfName("PieceInfo"));
        }

        // parse page xobjects
        // Catalog - Pages - page - resources - xobjects
        private static Dictionary<PdfName, string> ParsePageXobjects(PdfDictionary xobjects)
        {
            //PdfDictionary resources = page.GetAsDict(PdfName.RESOURCES);
            //if (resources == null) return false;
            //PdfDictionary xobjects = resources.GetAsDict(PdfName.XOBJECT);
            //if (xobjects == null) return false;

            Dictionary<PdfName, string> removables = new Dictionary<PdfName, string>();
            if (xobjects != null)
            {
                // remove XObject (form or image) that belong to an OCG that needs to be removed
                foreach (PdfName name in xobjects.Keys)
                {
                    PdfObject obj = xobjects.Get(name); // fanghui: not necessarily stream for Adobe watermark
                    if (obj.IsIndirect())  // for Adobe xobjects: Watermark, Background, or Headers/Footers or Header, Footer
                    {
                        PdfIndirectReference refi = (PdfIndirectReference)obj;
                        PdfDictionary tg = (PdfDictionary)PdfReader.GetPdfObject(refi);

                        PdfName subtype = (PdfName)tg.GetAsName(PdfName.SUBTYPE);
                        if ( PdfName.FORM.Equals(subtype))  // is form type, may be OC
                        {
                            if (tg.Get(PdfName.OC) != null)
                            {
                                // further look into OC object => /OCGs => /Name, or just look pieceinfo
                                string keyword = "";
                                PdfDictionary pieceinfo = tg.GetAsDict(new PdfName("PieceInfo"));
                                if (pieceinfo != null)
                                {
                                    PdfDictionary adbe = pieceinfo.GetAsDict(new PdfName("ADBE_CompoundType"));
                                    if (adbe != null)
                                    {
                                        keyword = adbe.GetAsName(PdfName.PRIVATE).ToString().TrimStart('/');
                                    }
                                }

                                // Note: should extract watermark text here for Adobe generated watermark

                                removables[name] = "XObject," + keyword + "," + name.ToString(); // mark removed
                                //PdfReader.KillIndirect(obj);
                            }
                        }
                        else if (PdfName.IMAGE.Equals(subtype))  // is image type
                        {
                            string filter = tg.Get(PdfName.FILTER).ToString();
                            //string imgname = tg.Get(PdfName.NAME).ToString();
                            string width = tg.Get(PdfName.WIDTH).ToString();
                            string height = tg.Get(PdfName.HEIGHT).ToString();

                            removables[name] = "XObject,Image," + "W=" + width + " H=" + height; // marked
                        }
                    }
                    else if (obj.IsStream())  // OC, for cipherbox watermark
                    {
                        PRStream ocstream = (PRStream)xobjects.GetAsStream(name);  //  PRStream ocstream = (PRStream)obj; more simpler??
                        PdfDictionary oc = ocstream.GetAsDict(PdfName.OC);   // if (ocstream.Get(PdfName.OC) != null) ??
                        if (oc != null)
                        {
                            PdfString ocname = oc.GetAsString(PdfName.NAME);
                            //Console.WriteLine("OC xobject found: " + name + ", layer name = " + ocname);
                            if (ocname != null)
                            {
                                removables[name] = "XObject," + ocname + ",";// mark removed
                            }
                        }
                    }
                    else if (obj.IsDictionary())  // for image type
                    {
                        // About Xobject Image:
                        // the stored JPEG data will appear in the XObject image. Here is an example:
                        // The /Type shows that this is an image. 
                        // The /Filter value ¨C DCTDecode indicates a JPEG
                        // The /Length value shows how long it is.
                        // The data is between stream and endstream. You need to extract the raw data (cut and paste of text is unlikely to work) for the jpeg file.
                        // if you take the binary data out and save it in a file with a .jpeg format, you can open it. 
                        // It includes not just the pixel data but also the JPEG header at the start ¨C it is a complete file.
                        /*
                        14 0 obj
                        <<
                        /Intent/RelativeColorimetric
                        /Type/XObject
                        /ColorSpace/DeviceGray
                        /Subtype/Image
                        /Name/X
                        /Width 2988
                        /BitsPerComponent 8
                        /Length 134030
                        /Height 2286
                        /Filter/DCTDecode
                        >>
                        stream (binary data) endstream
                        */

                        // TODO:
                        PdfDictionary tg = (PdfDictionary)obj;
                        PdfName type = (PdfName)tg.GetAsName(PdfName.SUBTYPE);
                        if (PdfName.IMAGE.Equals(type))  // is image type
                        {
                            string filter = tg.Get(PdfName.FILTER).ToString();
                            string imgname = tg.Get(PdfName.NAME).ToString();
                            string width = tg.Get(PdfName.WIDTH).ToString();
                            string height = tg.Get(PdfName.HEIGHT).ToString();

                            //Console.WriteLine("Image xobject found: " + imgname + ", width= " + width + " height= " + height + " filter= " + filter);
                            removables[name] = "XObject,Image," + " W=" + width + " H=" + height; // marked

                            /*
                            int xrefIdx = ((PRIndirectReference)obj).Number;
                            PdfObject pdfObj = reader.GetPdfObject(xrefIdx);
                            PdfStream str = (PdfStream)(pdfObj);
                            byte[] bytes = PdfReader.GetStreamBytesRaw((PRStream)str);
                            iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance((PRIndirectReference)obj);
                            */

                            if (filter == "/DCTDecode")  // for JPEG image file format
                            {
                                /*
                                PdfReader.KillIndirect(obj);

                                // replace it
                                Stream another = File.OpenRead("another.jpg");
                                iTextSharp.text.Image img2 = iTextSharp.text.Image.GetInstance(another);
                                PdfWriter writer = stamper.Writer;
                                writer.AddDirectImageSimple(img2, (PRIndirectReference)obj);
                                */
                            }
                        }
                    }
                }
            }

            return removables;
        }


        // Catalog - Pages - page - contents
        public void ParsePageContents(PdfDictionary page)
        {
            // reset parsing state
            this.mc_balance = 0;
            this.page_container_index = 0;
            if (this.RemovablePageContainers == null)
            {
                this.RemovablePageContainers = new Dictionary<int, string>();
            }

            // parse the page stream and remove OCGs
            PRStream stream = (PRStream)page.GetAsStream(PdfName.CONTENTS);
            if (stream != null)
            {
                // Note: if the page content is defined as an array instead of a stream, 
                // you need to transform the array of separate streams into one single stream first.

                // In our case we call reader.SetPageContent( reader.GetPageContent ), and then 
                // reader.GetPageN() to get one stream.

                // Another way to handle the content stream possibly being an array is to use ContentByteUtils.getContentBytesForPage() 
                // to get the effective content stream bytes of any page. 

                ParsePageStream(stream);
            }
            else
            {
                // Note: page contents do not need to be a single stream, they may also be an array of streams.
                // so we set page content before getting it as a stream.
                // otherwise falling into this branch may cause error!!

                //Get the raw content, and loop through content
                PdfArray contentarray = page.GetAsArray(PdfName.CONTENTS);
                if (contentarray == null) return;
                for (int j = 0; j < contentarray.Size; j++)
                {
                    //Get the raw byte stream
                    stream = (PRStream)contentarray.GetAsStream(j);

                    ParsePageStream(stream);
                }
            }
        }


        private void ParsePageStream(PRStream stream)
        {
            // create a new stream for this page
            baos = new MemoryStream();

            // get content bytes from page stream
            byte[] contentBytes = PdfReader.GetStreamBytes(stream);

            // parse the content stream. refer to: PdfContentStreamProcessor.cs ProcessContent()
            PRTokeniser tokeniser = new PRTokeniser(new RandomAccessFileOrArray(contentBytes));
            PdfContentParser ps = new PdfContentParser(tokeniser);
            List<PdfObject> operands = new List<PdfObject>();
            while (ps.Parse(operands).Count > 0)
            {
                // process an operator
                PdfLiteral oper = (PdfLiteral)operands[operands.Count - 1];

                // inline image (embedded) requires special handling because it does not follow the standard operator syntax
                if ("BI".Equals(oper.ToString()))
                {
                    // further parse the inline image information dictionary
                    PdfDictionary resources = this._page.GetAsDict(PdfName.RESOURCES);
                    PdfDictionary colorSpaceDic = (resources != null ? resources.GetAsDict(PdfName.COLORSPACE) : null);
                    InlineImageInfo info = InlineImageUtils.ParseInlineImage(ps, colorSpaceDic);

                    // TODO: to remove this embedded image upon user request
                    // See: http://superuser.com/questions/455462/how-to-remove-a-watermark-from-a-pdf-file

                    // write the inline image object to stream
                    // TODO: as a function of InlineImageUtils
                    if (true)
                    {
                        PdfLiteral bi = new PdfLiteral("BI");
                        bi.ToPdf(null, baos);
                        baos.WriteByte((byte)'\n');

                        // do not write info.ImageDictionary directly as PdfDictionary, like info.ImageDictionary.ToPdf(null, baos);
                        // because inline image does not contain << ... >>
                        PdfObject value;
                        foreach (KeyValuePair<PdfName, PdfObject> e in info.ImageDictionary)
                        {
                            value = e.Value;
                            e.Key.ToPdf(null, baos);
                            int type = value.Type;
                            if (type != PdfObject.ARRAY && type != PdfObject.DICTIONARY && type != PdfObject.NAME && type != PdfObject.STRING)
                                baos.WriteByte((byte)' ');
                            value.ToPdf(null, baos);
                        }
                        baos.WriteByte((byte)'\n');

                        PdfLiteral id = new PdfLiteral("ID");
                        id.ToPdf(null, baos);
                        baos.WriteByte((byte)'\n');

                        // write info samples
                        baos.Write(info.Samples, 0, info.Samples.Length);
                        baos.WriteByte((byte)'\n');

                        PdfLiteral ei = new PdfLiteral("EI");
                        ei.ToPdf(null, baos);
                        baos.WriteByte((byte)'\n');
                    }
                }
                else
                {
                    PdfOperator aaa = new PdfOperator(oper, operands);
                    aaa.Process(this);
                }
            }

            // reset the page stream
            baos.Flush();
            baos.Close();
            stream.SetData(baos.GetBuffer());
        }


        // Keeps track of the MarkedContent state
        // Example of marked content in page content stream:
/*
q
/OC /Xi0 BDC  
/Xi1 30 Tf  
/Xi2 gs
1 0 0 rg
BT  
0.70711 0.70711 -0.70711 0.70711 263.31 386.81 Tm 
(sample) Tj 
1 0 0 1 0 0 Tm 
ET  
EMC 
Q
*/
        protected internal virtual void CheckMarkedContentStart(PdfLiteral op, IList<PdfObject> operands)
        {
            if (!(op.ToString() == "BDC" || op.ToString() == "BMC")) return;

            // its parent was already marked as removed, so does itself
            if (mc_balance > 0)
            {
                mc_balance++;
                return;
            }

            // do not touch BMC at this time
            if (op.ToString() == "BMC") return;
            
            // then handle BDC part: further identify whether current operator should be removed

            if (operands.Count < 2) return;

            if ( PdfName.OC.Equals(operands[0]))
            {
                // OC, example: /OC /Xi0 BDC
                PdfName ocref = (PdfName)operands[1]; // a reference to an OCG dictionary

                // get OC layer name
                string ocname = "";
                if (this._properties != null)
                {
                    PdfDictionary ocdict = this._properties.GetAsDict(ocref);
                    if (ocdict != null)
                    {
                        PdfString OCname = ocdict.GetAsString(PdfName.NAME);
                        if (OCname != null)
                        {
                            ocname = OCname.ToString();
                        }
                    }
                }

                // save
                this.RemovablePageContainers[this.page_container_index] = "OC," + ocname + ",";
            }
            else if ( PdfName.ARTIFACT.Equals(operands[0]))
            {
                // Artifact, example: /Artifact <</Subtype /Watermark /Type /Pagination>> BDC
                PdfDictionary artifact = (PdfDictionary)operands[1];

                // get layer name
                string layername = "";
                PdfName LayerName = artifact.GetAsName(PdfName.SUBTYPE);
                if (LayerName != null)
                {
                    layername = LayerName.ToString().TrimStart('/');
                }

                // save
                this.RemovablePageContainers[this.page_container_index] = "Artifact," + layername + ",";
            }

            // do removal
            if (!this.ReadOnly)
            {
                if (this.RemovablePageContainers.ContainsKey(this.page_container_index))
                {
                    string sentence = this.RemovablePageContainers[this.page_container_index];
                    string[] kkk = sentence.Split(',');
                    
                    // marked as removed
                    if (   ( this.FilterToRemove != null && this.FilterToRemove.ContainsKey(this.page_container_index)  )
                        || (kkk.Length >=2 && this.LayersToRemove != null && this.LayersToRemove.Contains(kkk[1]))
                        )
                    {
                        mc_balance++;
                    }
                }
            }
        }


        /// <summary>
        /// Keeps track of the MarkedContent state.
        /// </summary>
        protected internal virtual void CheckMarkedContentEnd(PdfLiteral op)
        {
            if (op.ToString() != "EMC") return;

            // recover MC balance
            if (mc_balance > 0)
            {
                mc_balance--;
            }

            // increase index for page containers
            this.page_container_index++;
        }


        /// <summary>
        /// Checks operands to find out if the corresponding operator needs to be present or not. </summary>
        /// <param name="operands">	a list of operands
        /// @return	true if the operators needs to be present. </param>
        protected internal virtual bool IsVisible(IList<PdfObject> operands)
        {
            if (operands.Count > 1
                && this._xobjects_to_remove != null
                && this._xobjects_to_remove.Contains((PdfName)operands[0]))
            {
                return false;
            }

            return true;
        }


        /// <summary>
        /// Processes an operator </summary>
        /// <param name="operator">	the operator </param>
        /// <param name="operands">	its operands </param>
        /// <param name="removable">	is the operator eligable for removal? </param>
        /// <exception cref="IOException"> </exception>
        protected internal virtual void Process(PdfLiteral op, IList<PdfObject> operands, bool removable)
        {
            if (removable)
            {
                // populate the watermark text in findings
                //Console.WriteLine("\tcontent operator to be removed: " + @operator);
                if (op.ToString() == "Tj")
                {
                    string wmtext = operands[0].ToString();
                    if (this.RemovablePageContainers.ContainsKey(this.page_container_index))
                    {
                        // note: cipherbox watermark can be applied many times, but currently with same layer name!
                        // TODO: differentiate the layer names
                        string kk = this.RemovablePageContainers[this.page_container_index];

                        this.RemovablePageContainers[this.page_container_index] = kk + wmtext;
                    }
                }
                else if (op.ToString() == "Do")  // reference to Xobject
                {
                    string xobjref = operands[0].ToString();  // example: /Fm0
                    if (this.RemovablePageContainers.ContainsKey(this.page_container_index))
                    {
                        string kk = this.RemovablePageContainers[this.page_container_index];

                        // TODO: should append watermark text here
                        this.RemovablePageContainers[this.page_container_index] = kk +  xobjref ;
                    }
                }


                // Checks if the parser is currently parsing content that needs to be ignored.
                if (mc_balance > 0)  // marked as to be removed 
                {
                    return;  // skip writing for this operator
                }
            }

            // Read-only mode: still output as before
            operands.Remove(op);
            foreach (PdfObject o in operands)
            {
                // Writes a PDF object to the OutputStream, followed by a space character
                o.ToPdf(null, baos);
                baos.WriteByte((byte)' ');
            }
            // Writes a PDF object to the OutputStream, followed by a newline character.
            op.ToPdf(null, baos);
            baos.WriteByte((byte)'\n');
        }
    }
}