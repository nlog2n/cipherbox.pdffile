using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

// use iTextSharp to edit or remove the pdf layer
// OCGRemover.cs and OCGParser.cs are copied from iTextSharp 5.4.4 src-xtra.zip
// refer to: http://stackoverflow.com/questions/17687663/itext-edit-or-remove-the-layer-on-pdf

using iTextSharp.text.pdf;

namespace CipherBox.Pdf
{
    /// <summary>
    /// Class that knows how to remove OCG layers.
    /// </summary>
    public class OCGRemover
    {
        public  const string CipherboxWaterMarkLayerName = "CipherboxWatermark";
        public  const string CipherboxImageLayerName = "CipherboxImage";
        private const string AdobeWaterMarkLayerName = "Watermark";
        private const string AdobeBackgroundLayerName = "Background";
        private const string AdobeHeaderLayerName = "Header";
        private const string AdobeFooterLayerName = "Footer";
        private const string XObjectImageName = "Image";

        public void Remove(PdfReader reader)
        {
            List<string> layers = new List<string>{
                            CipherboxWaterMarkLayerName, 
                            CipherboxImageLayerName,
                            AdobeWaterMarkLayerName,
                            AdobeBackgroundLayerName,
                            AdobeHeaderLayerName,
                            AdobeFooterLayerName,
                            XObjectImageName};
            Remove(reader, layers, null);
        }


        public virtual void RemoveByLayer(PdfReader reader, List<string> layers)
        {
            Remove(reader, layers, null);
        }

        public void RemoveByFilter(PdfReader reader, List<string> filter)
        {
            Remove(reader, null, filter);
        }


        /// <summary>
        /// Removes layers from a PDF document </summary>
        /// <param name="reader">	a PdfReader containing a PDF document </param>
        /// <param name="layers">	a sequence of names of OCG layers </param>
        /// <exception cref="IOException"> </exception>
        public virtual void Remove(PdfReader reader, List<string> layers, List<string> filter)
        {
            if (layers == null)
            {
                layers = new List<string>();
            }

            // check each page
            int N = reader.NumberOfPages;
            for (int i = 1; i <= N; i++)
            {
                reader.SetPageContent(i, reader.GetPageContent(i));
                // trick: set page content so "PdfName.CONTENTS"is a single stream rather than an array
                
                //Console.WriteLine("parsing page #" + i.ToString());
                PdfDictionary page = reader.GetPageN(i);

                OCGParser parser = new OCGParser(page);
                parser.SetLayersToRemove(layers);
                parser.SetXObjectsToRemove(filter);
                parser.Remove();

                RemoveAnnots(page, layers);
                RemoveProperties(page, layers);
            }

            // overall check on root OC properties
            RemoveOCProperties(reader, layers);

            //Clean up the reader, optional
            reader.RemoveUnusedObjects();
        }


        private void RemoveOCProperties(PdfReader reader, List<string> layers)
        {
            if (layers == null) return;

            // overall check
            PdfDictionary root = reader.Catalog;

            PdfDictionary ocproperties = root.GetAsDict(PdfName.OCPROPERTIES);
            if (ocproperties != null)
            {
                RemoveOCGsFromDict(ocproperties, PdfName.OCGS, layers);
                PdfDictionary d = ocproperties.GetAsDict(PdfName.D);
                if (d != null)
                {
                    RemoveOCGsFromDict(d, PdfName.ON, layers);
                    RemoveOCGsFromDict(d, PdfName.OFF, layers);
                    RemoveOCGsFromDict(d, PdfName.LOCKED, layers);
                    RemoveOCGsFromDict(d, PdfName.RBGROUPS, layers);
                    RemoveOCGsFromDict(d, PdfName.ORDER, layers);
                    RemoveOCGsFromDict(d, PdfName.AS, layers);

                    // seems the /Name(CipherboxWatermark) still persists in /D
                    if (d.Contains(PdfName.NAME))
                    {
                        string cipherboxwatermark = d.GetAsString(PdfName.NAME).ToString();
                        if (layers.Contains(cipherboxwatermark))
                        {
                            //NOTE, This will destroy all layers in the document, only use if you don't have additional layers
                            root.Remove(PdfName.OCPROPERTIES); //Remove the OCG group completely from the document.
                        }
                    }
                }
            }
        }


        #region PdfDictionary or PdfArray recursive operations

        /// <summary>
        /// Checks if an OCG dictionary is on the list for removal. </summary>
        /// <param name="ocg">	a dictionary </param>
        /// <param name="names">	the removal list
        /// @return	true if the dictionary should be removed </param>
        private static bool IsToBeRemoved(PdfDictionary ocg, ICollection<string> names)
        {
            if (ocg == null)
                return false;

            PdfString n = ocg.GetAsString(PdfName.NAME);
            if (n == null)
                return false;

            return names.Contains(n.ToString());
        }


        /// <summary>
        /// Gets an array from a dictionary and checks if it contains references to OCGs that need to be removed </summary>
        /// <param name="dict">	the dictionary </param>
        /// <param name="name">	the name of an array entry </param>
        /// <param name="ocgs">	the removal list </param>
        private static void RemoveOCGsFromDict(PdfDictionary dict, PdfName name, ICollection<string> ocgs)
        {
            if (dict == null) return;
            
            PdfArray array = dict.GetAsArray(name);
            if (array == null) return;

            RemoveOCGsFromArray(array, ocgs);
        }

        /// <summary>
        /// Searches an array for references to OCGs that need to be removed. </summary>
        /// <param name="array">	the array </param>
        /// <param name="ocgs">	the removal list </param>
        private static void RemoveOCGsFromArray(PdfArray array, ICollection<string> ocgs)
        {
            if (array == null) return;
            
            PdfObject o;
            PdfDictionary dict;
            IList<int?> remove = new List<int?>();
            for (int i = array.Size; i > 0; )
            {
                o = array.GetDirectObject(--i);
                if (o.IsDictionary())
                {
                    dict = (PdfDictionary)o;
                    if (IsToBeRemoved(dict, ocgs))
                    {
                        remove.Add(i);
                    }
                    else
                    {
                        RemoveOCGsFromDict(dict, PdfName.OCGS, ocgs);
                    }
                }
                if (o.IsArray())
                {
                    RemoveOCGsFromArray((PdfArray)o, ocgs);
                }
            }
            foreach (int i in remove)
            {
                array.Remove(i);
            }
        }

        #endregion

        /// <summary>
        /// Removes annotations from a page dictionary </summary>
        /// <param name="page">	a page dictionary </param>
        /// <param name="ocgs">	a set of names of OCG layers </param>
        private void RemoveAnnots(PdfDictionary page, ICollection<string> ocgs)
        {
            PdfArray annots = page.GetAsArray(PdfName.ANNOTS);
            if (annots == null)  return;

            IList<int?> remove = new List<int?>();
            for (int i = annots.Size; i > 0; )
            {
                PdfDictionary annot = annots.GetAsDict(--i);
                if (IsToBeRemoved(annot.GetAsDict(PdfName.OC), ocgs))
                {
                    remove.Add(i);
                }
                else
                {
                    RemoveOCGsFromDict(annot.GetAsDict(PdfName.A), PdfName.STATE, ocgs);
                }
            }
            foreach (int i in remove)
            {
                annots.Remove(i);
            }
        }

        /// <summary>
        /// Removes ocgs from a page resources </summary>
        /// <param name="page">	a page dictionary </param>
        /// <param name="ocgs">	a set of names of OCG layers </param>
        private void RemoveProperties(PdfDictionary page, ICollection<string> ocgs)
        {
            PdfDictionary resources = page.GetAsDict(PdfName.RESOURCES);
            if (resources == null) return;
            PdfDictionary properties = resources.GetAsDict(PdfName.PROPERTIES);
            if (properties == null) return;

            ICollection<PdfName> names = properties.Keys;
            IList<PdfName> remove = new List<PdfName>();
            foreach (PdfName name in names)
            {
                PdfDictionary dict = properties.GetAsDict(name);
                if (IsToBeRemoved(dict, ocgs))
                {
                    remove.Add(name);
                }
                else
                {
                    RemoveOCGsFromDict(dict, PdfName.OCGS, ocgs);
                }
            }
            foreach (PdfName name in remove)
            {
                properties.Remove(name);
            }
        }


    }
}