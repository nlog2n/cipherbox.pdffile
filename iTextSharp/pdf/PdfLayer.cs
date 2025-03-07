using System;
using System.Collections.Generic;

namespace iTextSharp.text.pdf 
{
    /**
    * An Optional Content Group is a dictionary representing a collection of graphics
    * that can be made visible or invisible dynamically by users of viewer applications.
    * In iText they are referenced as layers.
    */
    public class PdfLayer : PdfDictionary, IPdfOCG 
    {
        protected PdfIndirectReference refi;
        protected List<PdfLayer> children;
        protected PdfLayer parent;
        protected String title;

        /**
        * Holds value of property on.
        */
        private bool on = true;
        
        /**
        * Holds value of property onPanel.
        */
        private bool onPanel = true;
        
        internal PdfLayer(String title) {
            this.title = title;
        }
        
        /**
        * Creates a title layer. A title layer is not really a layer but a collection of layers
        * under the same title heading.
        * @param title the title text
        * @param writer the <CODE>PdfWriter</CODE>
        * @return the title layer
        */    
        public static PdfLayer CreateTitle(String title, PdfWriter writer) {
            if (title == null)
                throw new ArgumentNullException("title.cannot.be.null");
            PdfLayer layer = new PdfLayer(title);
            writer.RegisterLayer(layer);
            return layer;
        }
        /**
        * Creates a new layer.
        * @param name the name of the layer
        * @param writer the writer
        */    
        public PdfLayer(String name, PdfWriter writer) : base(PdfName.OCG) {
            Name = name;
            if (writer is PdfStamperImp)
                refi = writer.AddToBody(this).IndirectReference;
            else
                refi = writer.PdfIndirectReference;
            writer.RegisterLayer(this);
        }
        
        internal String Title {
            get {
                return title;
            }
        }
        
        /**
        * Adds a child layer. Nested layers can only have one parent.
        * @param child the child layer
        */    
        public void AddChild(PdfLayer child) {
            if (child.parent != null)
                throw new ArgumentException(string.Format("the.layer.{0}.already.has.a.parent", child.GetAsString(PdfName.NAME).ToUnicodeString()));
            child.parent = this;
            if (children == null)
                children = new List<PdfLayer>();
            children.Add(child);
        }

        
        /**
        * Gets the parent layer.
        * @return the parent layer or <CODE>null</CODE> if the layer has no parent
        */    
        public PdfLayer Parent {
            get {
                return parent;
            }
        }
        
        /**
        * Gets the children layers.
        * @return the children layers or <CODE>null</CODE> if the layer has no children
        */    
        public List<PdfLayer> Children {
            get {
                return children;
            }
        }
        
        /**
        * Gets the <CODE>PdfIndirectReference</CODE> that represents this layer.
        * @return the <CODE>PdfIndirectReference</CODE> that represents this layer
        */    
        public PdfIndirectReference Ref {
            get {
                return refi;
            }
            set {
                refi = value;
            }
        }
        
        /**
        * Sets the name of this layer.
        * @param name the name of this layer
        */    
        public string Name {
            set {
                Put(PdfName.NAME, new PdfString(value, PdfObject.TEXT_UNICODE));
            }
        }
        
        /**
        * Gets the dictionary representing the layer. It just returns <CODE>this</CODE>.
        * @return the dictionary representing the layer
        */    
        public PdfObject PdfObject {
            get {
                return this;
            }
        }
        
        /**
        * Gets the initial visibility of the layer.
        * @return the initial visibility of the layer
        */
        public bool On {
            get {
                return this.on;
            }
            set {
                on = value;
            }
        }
        
        
        private PdfDictionary Usage {
            get {
                PdfDictionary usage = GetAsDict(PdfName.USAGE);
                if (usage == null) {
                    usage = new PdfDictionary();
                    Put(PdfName.USAGE, usage);
                }
                return usage;
            }
        }
        
        /**
        * Used by the creating application to store application-specific
        * data associated with this optional content group.
        * @param creator a text string specifying the application that created the group
        * @param subtype a string defining the type of content controlled by the group. Suggested
        * values include but are not limited to <B>Artwork</B>, for graphic-design or publishing
        * applications, and <B>Technical</B>, for technical designs such as building plans or
        * schematics
        */    
        public void SetCreatorInfo(String creator, String subtype) {
            PdfDictionary usage = Usage;
            PdfDictionary dic = new PdfDictionary();
            dic.Put(PdfName.CREATOR, new PdfString(creator, PdfObject.TEXT_UNICODE));
            dic.Put(PdfName.SUBTYPE, new PdfName(subtype));
            usage.Put(PdfName.CREATORINFO, dic);
        }
        
        /**
        * Specifies the language of the content controlled by this
        * optional content group
        * @param lang a language string which specifies a language and possibly a locale
        * (for example, <B>es-MX</B> represents Mexican Spanish)
        * @param preferred used by viewer applications when there is a partial match but no exact
        * match between the system language and the language strings in all usage dictionaries
        */    
        public void SetLanguage(String lang, bool preferred) {
            PdfDictionary usage = Usage;
            PdfDictionary dic = new PdfDictionary();
            dic.Put(PdfName.LANG, new PdfString(lang, PdfObject.TEXT_UNICODE));
            if (preferred)
                dic.Put(PdfName.PREFERRED, PdfName.ON);
            usage.Put(PdfName.LANGUAGE, dic);
        }
        
        /**
        * Specifies the recommended state for content in this
        * group when the document (or part of it) is saved by a viewer application to a format
        * that does not support optional content (for example, an earlier version of
        * PDF or a raster image format).
        * @param export the export state
        */    
        public bool Export {
            set {
                PdfDictionary usage = Usage;
                PdfDictionary dic = new PdfDictionary();
                dic.Put(PdfName.EXPORTSTATE, value ? PdfName.ON : PdfName.OFF);
                usage.Put(PdfName.EXPORT, dic);
            }
        }
        
        /**
        * Specifies a range of magnifications at which the content
        * in this optional content group is best viewed.
        * @param min the minimum recommended magnification factors at which the group
        * should be ON. A negative value will set the default to 0
        * @param max the maximum recommended magnification factor at which the group
        * should be ON. A negative value will set the largest possible magnification supported by the
        * viewer application
        */    
        public void SetZoom(float min, float max) {
            if (min <= 0 && max < 0)
                return;
            PdfDictionary usage = Usage;
            PdfDictionary dic = new PdfDictionary();
            if (min > 0)
                dic.Put(PdfName.MIN_LOWER_CASE, new PdfNumber(min));
            if (max >= 0)
                dic.Put(PdfName.MAX_LOWER_CASE, new PdfNumber(max));
            usage.Put(PdfName.ZOOM, dic);
        }

        /**
        * Specifies that the content in this group is intended for
        * use in printing
        * @param subtype a name specifying the kind of content controlled by the group;
        * for example, <B>Trapping</B>, <B>PrintersMarks</B> and <B>Watermark</B>
        * @param printstate indicates that the group should be
        * set to that state when the document is printed from a viewer application
        */    
        public void SetPrint(String subtype, bool printstate) 
        {
            PdfDictionary usage = Usage;
            PdfDictionary dic = new PdfDictionary();
            dic.Put(PdfName.SUBTYPE, new PdfName(subtype));
            dic.Put(PdfName.PRINTSTATE, printstate ? PdfName.ON : PdfName.OFF);
            usage.Put(PdfName.PRINT, dic);
        }

        /**
        * Indicates that the group should be set to that state when the
        * document is opened in a viewer application.
        * @param view the view state
        */    
        public bool View {
            set {
                PdfDictionary usage = Usage;
                PdfDictionary dic = new PdfDictionary();
                dic.Put(PdfName.VIEWSTATE, value ? PdfName.ON : PdfName.OFF);
                usage.Put(PdfName.VIEW, dic);
            }
        }
        
        /**
         * Indicates that the group contains a pagination artifact.
         * @param pe one of the following names: "HF" (Header Footer),
         * "FG" (Foreground), "BG" (Background), or "L" (Logo).
         * @since 5.0.2
         */
        public string PageElement {
            set {
                PdfDictionary usage = Usage;
                PdfDictionary dic = new PdfDictionary();
                dic.Put(PdfName.SUBTYPE, new PdfName(value));
                usage.Put(PdfName.PAGEELEMENT, dic);
            }
        }
        
        /**
         * One of more users for whom this optional content group is primarily intended.
         * @param type should be "Ind" (Individual), "Ttl" (Title), or "Org" (Organization).
         * @param names one or more names
         * @since 5.0.2
         */
        public void SetUser(String type, String[] names) {
            PdfDictionary usage = Usage;
            PdfDictionary dic = new PdfDictionary();
            dic.Put(PdfName.TYPE, new PdfName(type));
            PdfArray arr = new PdfArray();
            foreach (String s in names)
                arr.Add(new PdfString(s, PdfObject.TEXT_UNICODE));
            usage.Put(PdfName.NAME, arr);
            usage.Put(PdfName.USER, dic);
        }

        /**
        * Gets the layer visibility in Acrobat's layer panel
        * @return the layer visibility in Acrobat's layer panel
        */
        /**
        * Sets the visibility of the layer in Acrobat's layer panel. If <CODE>false</CODE>
        * the layer cannot be directly manipulated by the user. Note that any children layers will
        * also be absent from the panel.
        * @param onPanel the visibility of the layer in Acrobat's layer panel
        */
        public bool OnPanel {
            get {
                return this.onPanel;
            }
            set {
                onPanel = value;
            }
        }
    }
}
