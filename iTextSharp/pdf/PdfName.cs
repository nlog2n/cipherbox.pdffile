using System;
using System.Text;
using System.Collections.Generic;
using System.Reflection;

namespace iTextSharp.text.pdf
{
    /// <summary>
    /// PdfName is an object used as a name in PDF file. A name, like a string, is 
    /// a sequence of characters starting with a slash followed by a sequence of ASCII
    /// characters (range 32-136 except %, (, ), [, ], &lt;, &gt;, {, }, / and #.
    /// Any character except 0x00 may be included in a name by writing its twocharacter hex code, 
    /// preceded by #. The maximum number of characters in a name is 127.
    /// This object is described in the 'PDF Reference Manual version 1.3' section 4.5 (page 39-40).
    /// </summary>
    public class PdfName : PdfObject, IComparable<PdfName>
    {
        // CLASS CONSTANTS (a variety of standard names used in PDF))
        public static readonly PdfName _3D = new PdfName("3D");
        public static readonly PdfName A = new PdfName("A");
        public static readonly PdfName A85 = new PdfName("A85");
        public static readonly PdfName AA = new PdfName("AA");
        public static readonly PdfName ABSOLUTECOLORIMETRIC = new PdfName("AbsoluteColorimetric");
        public static readonly PdfName AC = new PdfName("AC");
        public static readonly PdfName ACROFORM = new PdfName("AcroForm");
        public static readonly PdfName ACTION = new PdfName("Action");
        public static readonly PdfName ACTIVATION = new PdfName("Activation");
        public static readonly PdfName ADBE = new PdfName("ADBE");
        public static readonly PdfName ACTUALTEXT = new PdfName("ActualText");
        public static readonly PdfName ADBE_PKCS7_DETACHED = new PdfName("adbe.pkcs7.detached");
        public static readonly PdfName ADBE_PKCS7_S4 = new PdfName("adbe.pkcs7.s4");
        public static readonly PdfName ADBE_PKCS7_S5 = new PdfName("adbe.pkcs7.s5");
        public static readonly PdfName ADBE_PKCS7_SHA1 = new PdfName("adbe.pkcs7.sha1");
        public static readonly PdfName ADBE_X509_RSA_SHA1 = new PdfName("adbe.x509.rsa_sha1");
        public static readonly PdfName ADOBE_PPKLITE = new PdfName("Adobe.PPKLite");
        public static readonly PdfName ADOBE_PPKMS = new PdfName("Adobe.PPKMS");
        public static readonly PdfName AESV2 = new PdfName("AESV2");
        public static readonly PdfName AESV3 = new PdfName("AESV3");
        public static readonly PdfName AHX = new PdfName("AHx");
        public static readonly PdfName AIS = new PdfName("AIS");
        public static readonly PdfName ALL = new PdfName("All");
        public static readonly PdfName ALLPAGES = new PdfName("AllPages");
        public static readonly PdfName ALT = new PdfName("Alt");
        public static readonly PdfName ALTERNATE = new PdfName("Alternate");
        public static readonly PdfName AND = new PdfName("And");
        public static readonly PdfName ANIMATION = new PdfName("Animation");
        public static readonly PdfName ANNOT = new PdfName("Annot");
        public static readonly PdfName ANNOTS = new PdfName("Annots");
        public static readonly PdfName ANTIALIAS = new PdfName("AntiAlias");
        public static readonly PdfName AP = new PdfName("AP");
        public static readonly PdfName APPDEFAULT = new PdfName("AppDefault");
        public static readonly PdfName ART = new PdfName("Art");
        public static readonly PdfName ARTBOX = new PdfName("ArtBox");
        public static readonly PdfName ARTIFACT = new PdfName("Artifact");
        public static readonly PdfName ASCENT = new PdfName("Ascent");
        public static readonly PdfName AS = new PdfName("AS");
        public static readonly PdfName ASCII85DECODE = new PdfName("ASCII85Decode");
        public static readonly PdfName ASCIIHEXDECODE = new PdfName("ASCIIHexDecode");
        public static readonly PdfName ASSET = new PdfName("Asset");
        public static readonly PdfName ASSETS = new PdfName("Assets");
        public static PdfName ATTACHED = new PdfName("Attached");
        public static readonly PdfName AUTHEVENT = new PdfName("AuthEvent");
        public static readonly PdfName AUTHOR = new PdfName("Author");

        public static readonly PdfName B = new PdfName("B");
        public static readonly PdfName BACKGROUND = new PdfName("Background");
        public static readonly PdfName BACKGROUNDCOLOR = new PdfName("BackgroundColor");
        public static readonly PdfName BASEENCODING = new PdfName("BaseEncoding");
        public static readonly PdfName BASEFONT = new PdfName("BaseFont");
        public static readonly PdfName BASEVERSION = new PdfName("BaseVersion");
        public static readonly PdfName BBOX = new PdfName("BBox");
        public static readonly PdfName BC = new PdfName("BC");
        public static readonly PdfName BG = new PdfName("BG");
        public static readonly PdfName BIBENTRY = new PdfName("BibEntry");
        public static readonly PdfName BIGFIVE = new PdfName("BigFive");
        public static readonly PdfName BINDING = new PdfName("Binding");
        public static readonly PdfName BINDINGMATERIALNAME = new PdfName("BindingMaterialName");
        public static readonly PdfName BITSPERCOMPONENT = new PdfName("BitsPerComponent");
        public static readonly PdfName BITSPERSAMPLE = new PdfName("BitsPerSample");
        public static readonly PdfName BL = new PdfName("Bl");
        public static readonly PdfName BLACKIS1 = new PdfName("BlackIs1");
        public static readonly PdfName BLACKPOINT = new PdfName("BlackPoint");
        public static readonly PdfName BLOCKQUOTE = new PdfName("BlockQuote");
        public static readonly PdfName BLEEDBOX = new PdfName("BleedBox");
        public static readonly PdfName BLINDS = new PdfName("Blinds");
        public static readonly PdfName BM = new PdfName("BM");
        public static readonly PdfName BORDER = new PdfName("Border");
        public static readonly PdfName BOTH = new PdfName("Both");
        public static readonly PdfName BOUNDS = new PdfName("Bounds");
        public static readonly PdfName BOX = new PdfName("Box");
        public static readonly PdfName BS = new PdfName("BS");
        public static readonly PdfName BTN = new PdfName("Btn");
        public static readonly PdfName BYTERANGE = new PdfName("ByteRange");

        public static readonly PdfName C = new PdfName("C");
        public static readonly PdfName C0 = new PdfName("C0");
        public static readonly PdfName C1 = new PdfName("C1");
        public static readonly PdfName CA = new PdfName("CA");
        public static readonly PdfName ca_ = new PdfName("ca");
        public static readonly PdfName CALGRAY = new PdfName("CalGray");
        public static readonly PdfName CALRGB = new PdfName("CalRGB");
        public static readonly PdfName CAPHEIGHT = new PdfName("CapHeight");
        public static readonly PdfName CAPTION = new PdfName("Caption");
        public static readonly PdfName CATALOG = new PdfName("Catalog");
        public static readonly PdfName CATEGORY = new PdfName("Category");
        public static readonly PdfName CCITTFAXDECODE = new PdfName("CCITTFaxDecode");
        public static readonly PdfName CENTER = new PdfName("Center");
        public static readonly PdfName CENTERWINDOW = new PdfName("CenterWindow");
        public static readonly PdfName CERT = new PdfName("Cert");
        public static readonly PdfName CERTS = new PdfName("Certs");
        public static readonly PdfName CF = new PdfName("CF");
        public static readonly PdfName CFM = new PdfName("CFM");
        public static readonly PdfName CH = new PdfName("Ch");
        public static readonly PdfName CHARPROCS = new PdfName("CharProcs");
        public static readonly PdfName CHECKSUM = new PdfName("CheckSum");
        public static readonly PdfName CI = new PdfName("CI");
        public static readonly PdfName CIDFONTTYPE0 = new PdfName("CIDFontType0");
        public static readonly PdfName CIDFONTTYPE2 = new PdfName("CIDFontType2");
        public static readonly PdfName CIDSET = new PdfName("CIDSet");
        public static readonly PdfName CIDSYSTEMINFO = new PdfName("CIDSystemInfo");
        public static readonly PdfName CIDTOGIDMAP = new PdfName("CIDToGIDMap");
        public static readonly PdfName CIRCLE = new PdfName("Circle");
        public static readonly PdfName CLASSMAP = new PdfName("ClassMap");
        public static readonly PdfName CLOUD = new PdfName("Cloud");
        public static readonly PdfName CMD = new PdfName("CMD");
        public static readonly PdfName CO = new PdfName("CO");
        public static readonly PdfName CODE = new PdfName("Code");
        public static readonly PdfName COLOR = new PdfName("Color");
        public static readonly PdfName COLORS = new PdfName("Colors");
        public static readonly PdfName COLORSPACE = new PdfName("ColorSpace");
        public static readonly PdfName COLLECTION = new PdfName("Collection");
        public static readonly PdfName COLLECTIONFIELD = new PdfName("CollectionField");
        public static readonly PdfName COLLECTIONITEM = new PdfName("CollectionItem");
        public static readonly PdfName COLLECTIONSCHEMA = new PdfName("CollectionSchema");
        public static readonly PdfName COLLECTIONSORT = new PdfName("CollectionSort");
        public static readonly PdfName COLLECTIONSUBITEM = new PdfName("CollectionSubitem");
        public static readonly PdfName COLSPAN = new PdfName("Colspan");
        public static readonly PdfName COLUMN = new PdfName("Column");
        public static readonly PdfName COLUMNS = new PdfName("Columns");
        public static readonly PdfName CONDITION = new PdfName("Condition");
        public static readonly PdfName CONFIGS = new PdfName("Configs");
        public static readonly PdfName CONFIGURATION = new PdfName("Configuration");
        public static readonly PdfName CONFIGURATIONS = new PdfName("Configurations");
        public static readonly PdfName CONTACTINFO = new PdfName("ContactInfo");
        public static readonly PdfName CONTENT = new PdfName("Content");
        public static readonly PdfName CONTENTS = new PdfName("Contents");
        public static readonly PdfName COORDS = new PdfName("Coords");
        public static readonly PdfName COUNT = new PdfName("Count");
        public static readonly PdfName COURIER = new PdfName("Courier");         /** A name of a base 14 type 1 font */
        public static readonly PdfName COURIER_BOLD = new PdfName("Courier-Bold");         /** A name of a base 14 type 1 font */
        public static readonly PdfName COURIER_OBLIQUE = new PdfName("Courier-Oblique"); /** A name of a base 14 type 1 font */
        public static readonly PdfName COURIER_BOLDOBLIQUE = new PdfName("Courier-BoldOblique"); /** A name of a base 14 type 1 font */
        public static readonly PdfName CREATIONDATE = new PdfName("CreationDate");
        public static readonly PdfName CREATOR = new PdfName("Creator");
        public static readonly PdfName CREATORINFO = new PdfName("CreatorInfo");
        public static readonly PdfName CRL = new PdfName("CRL");
        public static readonly PdfName CRLS = new PdfName("CRLs");
        public static readonly PdfName CROPBOX = new PdfName("CropBox");
        public static readonly PdfName CRYPT = new PdfName("Crypt");
        public static readonly PdfName CS = new PdfName("CS");
        public static readonly PdfName CUEPOINT = new PdfName("CuePoint");
        public static readonly PdfName CUEPOINTS = new PdfName("CuePoints");
        public static readonly PdfName CYX = new PdfName("CYX"); /**         * A name of an attribute.         */

        public static readonly PdfName D = new PdfName("D");
        public static readonly PdfName DA = new PdfName("DA");
        public static readonly PdfName DATA = new PdfName("Data");
        public static readonly PdfName DC = new PdfName("DC");
        public static readonly PdfName DCS = new PdfName("DCS"); /**         * A name of an attribute.  */
        public static readonly PdfName DCTDECODE = new PdfName("DCTDecode");
        public static readonly PdfName DECIMAL = new PdfName("Decimal");
        public static readonly PdfName DEACTIVATION = new PdfName("Deactivation");
        public static readonly PdfName DECODE = new PdfName("Decode");
        public static readonly PdfName DECODEPARMS = new PdfName("DecodeParms");
        public static readonly PdfName DEFAULT = new PdfName("Default");
        public static readonly PdfName DEFAULTCRYPTFILTER = new PdfName("DefaultCryptFilter");
        public static readonly PdfName DEFAULTCMYK = new PdfName("DefaultCMYK");
        public static readonly PdfName DEFAULTGRAY = new PdfName("DefaultGray");
        public static readonly PdfName DEFAULTRGB = new PdfName("DefaultRGB");
        public static readonly PdfName DESC = new PdfName("Desc");
        public static readonly PdfName DESCENDANTFONTS = new PdfName("DescendantFonts");
        public static readonly PdfName DESCENT = new PdfName("Descent");
        public static readonly PdfName DEST = new PdfName("Dest");
        public static readonly PdfName DESTOUTPUTPROFILE = new PdfName("DestOutputProfile");
        public static readonly PdfName DESTS = new PdfName("Dests");
        public static readonly PdfName DEVICEGRAY = new PdfName("DeviceGray");
        public static readonly PdfName DEVICERGB = new PdfName("DeviceRGB");
        public static readonly PdfName DEVICECMYK = new PdfName("DeviceCMYK");
        public static readonly PdfName DEVICEN = new PdfName("DeviceN");
        public static readonly PdfName DI = new PdfName("Di");
        public static readonly PdfName DIFFERENCES = new PdfName("Differences");
        public static readonly PdfName DISSOLVE = new PdfName("Dissolve");
        public static readonly PdfName DIRECTION = new PdfName("Direction");
        public static readonly PdfName DISPLAYDOCTITLE = new PdfName("DisplayDocTitle");
        public static readonly PdfName DIV = new PdfName("Div");
        public static readonly PdfName DL = new PdfName("DL");
        public static readonly PdfName DM = new PdfName("Dm");
        public static readonly PdfName DOS = new PdfName("DOS");
        public static readonly PdfName DOCMDP = new PdfName("DocMDP");
        public static readonly PdfName DOCOPEN = new PdfName("DocOpen");
        public static readonly PdfName DOCTIMESTAMP = new PdfName("DocTimeStamp");
        public static readonly PdfName DOCUMENT = new PdfName("Document");
        public static readonly PdfName DOMAIN = new PdfName("Domain");
        public static readonly PdfName DP = new PdfName("DP");
        public static readonly PdfName DR = new PdfName("DR");
        public static readonly PdfName DS = new PdfName("DS");
        public static readonly PdfName DSS = new PdfName("DSS");
        public static readonly PdfName DUR = new PdfName("Dur");
        public static readonly PdfName DUPLEX = new PdfName("Duplex");
        public static readonly PdfName DUPLEXFLIPSHORTEDGE = new PdfName("DuplexFlipShortEdge");
        public static readonly PdfName DUPLEXFLIPLONGEDGE = new PdfName("DuplexFlipLongEdge");
        public static readonly PdfName DV = new PdfName("DV");
        public static readonly PdfName DW = new PdfName("DW");

        public static readonly PdfName E = new PdfName("E");
        public static readonly PdfName EARLYCHANGE = new PdfName("EarlyChange");
        public static readonly PdfName EF = new PdfName("EF");
        public static readonly PdfName EFF = new PdfName("EFF");
        public static readonly PdfName EFOPEN = new PdfName("EFOpen");
        public static readonly PdfName EMBEDDED = new PdfName("Embedded");
        public static readonly PdfName EMBEDDEDFILE = new PdfName("EmbeddedFile");
        public static readonly PdfName EMBEDDEDFILES = new PdfName("EmbeddedFiles");
        public static readonly PdfName ENCODE = new PdfName("Encode");
        public static readonly PdfName ENCODEDBYTEALIGN = new PdfName("EncodedByteAlign");
        public static readonly PdfName ENCODING = new PdfName("Encoding");
        public static readonly PdfName ENCRYPT = new PdfName("Encrypt");
        public static readonly PdfName ENCRYPTMETADATA = new PdfName("EncryptMetadata");
        public static readonly PdfName END = new PdfName("End");
        public static readonly PdfName ENDINDENT = new PdfName("EndIndent");
        public static readonly PdfName ENDOFBLOCK = new PdfName("EndOfBlock");
        public static readonly PdfName ENDOFLINE = new PdfName("EndOfLine");
        public static readonly PdfName EPSG = new PdfName("EPSG");
        public static readonly PdfName ETSI_CADES_DETACHED = new PdfName("ETSI.CAdES.detached");
        public static readonly PdfName ETSI_RFC3161 = new PdfName("ETSI.RFC3161");
        public static readonly PdfName EXCLUDE = new PdfName("Exclude");
        public static readonly PdfName EXTEND = new PdfName("Extend");
        public static readonly PdfName EXTENSIONS = new PdfName("Extensions");
        public static readonly PdfName EXTENSIONLEVEL = new PdfName("ExtensionLevel");
        public static readonly PdfName EXTGSTATE = new PdfName("ExtGState");
        public static readonly PdfName EXPORT = new PdfName("Export");
        public static readonly PdfName EXPORTSTATE = new PdfName("ExportState");
        public static readonly PdfName EVENT = new PdfName("Event");

        public static readonly PdfName F = new PdfName("F");
        public static readonly PdfName FAR = new PdfName("Far");
        public static readonly PdfName FB = new PdfName("FB");
        public static readonly PdfName FD = new PdfName("FD");
        public static readonly PdfName FDECODEPARMS = new PdfName("FDecodeParms");
        public static readonly PdfName FDF = new PdfName("FDF");
        public static readonly PdfName FF = new PdfName("Ff");
        public static readonly PdfName FFILTER = new PdfName("FFilter");
        public static readonly PdfName FG = new PdfName("FG");
        public static readonly PdfName FIELDMDP = new PdfName("FieldMDP");
        public static readonly PdfName FIELDS = new PdfName("Fields");
        public static readonly PdfName FIGURE = new PdfName("Figure");
        public static readonly PdfName FILEATTACHMENT = new PdfName("FileAttachment");
        public static readonly PdfName FILESPEC = new PdfName("Filespec");
        public static readonly PdfName FILTER = new PdfName("Filter");
        public static readonly PdfName FIRST = new PdfName("First");
        public static readonly PdfName FIRSTCHAR = new PdfName("FirstChar");
        public static readonly PdfName FIRSTPAGE = new PdfName("FirstPage");
        public static readonly PdfName FIT = new PdfName("Fit");
        public static readonly PdfName FITH = new PdfName("FitH");
        public static readonly PdfName FITV = new PdfName("FitV");
        public static readonly PdfName FITR = new PdfName("FitR");
        public static readonly PdfName FITB = new PdfName("FitB");
        public static readonly PdfName FITBH = new PdfName("FitBH");
        public static readonly PdfName FITBV = new PdfName("FitBV");
        public static readonly PdfName FITWINDOW = new PdfName("FitWindow");
        public static readonly PdfName FL = new PdfName("Fl");
        public static readonly PdfName FLAGS = new PdfName("Flags");
        public static readonly PdfName FLASH = new PdfName("Flash");
        public static readonly PdfName FLASHVARS = new PdfName("FlashVars");
        public static readonly PdfName FLATEDECODE = new PdfName("FlateDecode");
        public static readonly PdfName FO = new PdfName("Fo");
        public static readonly PdfName FONT = new PdfName("Font");
        public static readonly PdfName FONTBBOX = new PdfName("FontBBox");
        public static readonly PdfName FONTDESCRIPTOR = new PdfName("FontDescriptor");
        public static readonly PdfName FONTFAMILY = new PdfName("FontFamily");
        public static readonly PdfName FONTFILE = new PdfName("FontFile");
        public static readonly PdfName FONTFILE2 = new PdfName("FontFile2");
        public static readonly PdfName FONTFILE3 = new PdfName("FontFile3");
        public static readonly PdfName FONTMATRIX = new PdfName("FontMatrix");
        public static readonly PdfName FONTNAME = new PdfName("FontName");
        public static readonly PdfName FONTWEIGHT = new PdfName("FontWeight");
        public static readonly PdfName FOREGROUND = new PdfName("Foreground");
        public static readonly PdfName FORM = new PdfName("Form");
        public static readonly PdfName FORMTYPE = new PdfName("FormType");
        public static readonly PdfName FORMULA = new PdfName("Formula");
        public static readonly PdfName FREETEXT = new PdfName("FreeText");
        public static readonly PdfName FRM = new PdfName("FRM");
        public static readonly PdfName FS = new PdfName("FS");
        public static readonly PdfName FT = new PdfName("FT");
        public static readonly PdfName FULLSCREEN = new PdfName("FullScreen");
        public static readonly PdfName FUNCTION = new PdfName("Function");
        public static readonly PdfName FUNCTIONS = new PdfName("Functions");
        public static readonly PdfName FUNCTIONTYPE = new PdfName("FunctionType");

        public static readonly PdfName GAMMA = new PdfName("Gamma");         /** A name of an attribute. */
        public static readonly PdfName GBK = new PdfName("GBK"); /** A name of an attribute. */
        public static readonly PdfName GCS = new PdfName("GCS"); /*** A name of an attribute.       */
        public static readonly PdfName GEO = new PdfName("GEO");  // A name of an attribute
        public static readonly PdfName GEOGCS = new PdfName("GEOGCS");  // A name of an attribute
        public static readonly PdfName GLITTER = new PdfName("Glitter");  // A name of an attribute
        public static readonly PdfName GOTO = new PdfName("GoTo");  // A name of an attribute
        public static readonly PdfName GOTOE = new PdfName("GoToE");  // A name of an attribute
        public static readonly PdfName GOTOR = new PdfName("GoToR");  // A name of an attribute
        public static readonly PdfName GPTS = new PdfName("GPTS");  // A name of an attribute
        public static readonly PdfName GROUP = new PdfName("Group"); // A name of an attribute
        public static readonly PdfName GTS_PDFA1 = new PdfName("GTS_PDFA1");  // A name of an attribute
        public static readonly PdfName GTS_PDFX = new PdfName("GTS_PDFX");  // A name of an attribute
        public static readonly PdfName GTS_PDFXVERSION = new PdfName("GTS_PDFXVersion");  // A name of an attribute

        public static readonly PdfName H = new PdfName("H");  // A name of an attribute
        public static readonly PdfName H1 = new PdfName("H1");
        public static readonly PdfName H2 = new PdfName("H2");
        public static readonly PdfName H3 = new PdfName("H3");
        public static readonly PdfName H4 = new PdfName("H4");
        public static readonly PdfName H5 = new PdfName("H5");
        public static readonly PdfName H6 = new PdfName("H6");
        public static readonly PdfName HALIGN = new PdfName("HAlign");
        public static readonly PdfName HEADERS = new PdfName("Headers");
        public static readonly PdfName HEIGHT = new PdfName("Height");         /** A name of an attribute. */
        public static readonly PdfName HELV = new PdfName("Helv");
        public static readonly PdfName HELVETICA = new PdfName("Helvetica"); /** A name of a base 14 type 1 font */
        public static readonly PdfName HELVETICA_BOLD = new PdfName("Helvetica-Bold"); /** A name of a base 14 type 1 font */
        public static readonly PdfName HELVETICA_OBLIQUE = new PdfName("Helvetica-Oblique"); /** A name of a base 14 type 1 font */
        public static readonly PdfName HELVETICA_BOLDOBLIQUE = new PdfName("Helvetica-BoldOblique"); /** A name of a base 14 type 1 font */
        public static readonly PdfName HF = new PdfName("HF");
        public static readonly PdfName HID = new PdfName("Hid");
        public static readonly PdfName HIDE = new PdfName("Hide");
        public static readonly PdfName HIDEMENUBAR = new PdfName("HideMenubar");
        public static readonly PdfName HIDETOOLBAR = new PdfName("HideToolbar");
        public static readonly PdfName HIDEWINDOWUI = new PdfName("HideWindowUI");
        public static readonly PdfName HIGHLIGHT = new PdfName("Highlight");
        public static readonly PdfName HOFFSET = new PdfName("HOffset");

        public static readonly PdfName I = new PdfName("I");
        public static readonly PdfName ICCBASED = new PdfName("ICCBased");
        public static readonly PdfName ID = new PdfName("ID");
        public static readonly PdfName IDENTITY = new PdfName("Identity");
        public static readonly PdfName IF = new PdfName("IF");
        public static readonly PdfName IMAGE = new PdfName("Image");
        public static readonly PdfName IMAGEB = new PdfName("ImageB");
        public static readonly PdfName IMAGEC = new PdfName("ImageC");
        public static readonly PdfName IMAGEI = new PdfName("ImageI");
        public static readonly PdfName IMAGEMASK = new PdfName("ImageMask");
        public static readonly PdfName INCLUDE = new PdfName("Include");
        public static readonly PdfName IND = new PdfName("Ind");
        public static readonly PdfName INDEX = new PdfName("Index");
        public static readonly PdfName INDEXED = new PdfName("Indexed");
        public static readonly PdfName INFO = new PdfName("Info");
        public static readonly PdfName INK = new PdfName("Ink");
        public static readonly PdfName INKLIST = new PdfName("InkList");
        public static readonly PdfName INSTANCES = new PdfName("Instances");
        public static readonly PdfName IMPORTDATA = new PdfName("ImportData");
        public static readonly PdfName INTENT = new PdfName("Intent");
        public static readonly PdfName INTERPOLATE = new PdfName("Interpolate");
        public static readonly PdfName ISMAP = new PdfName("IsMap");
        public static readonly PdfName IRT = new PdfName("IRT");
        public static readonly PdfName ITALICANGLE = new PdfName("ItalicAngle");
        public static readonly PdfName ITXT = new PdfName("ITXT");
        public static readonly PdfName IX = new PdfName("IX");

        public static readonly PdfName JAVASCRIPT = new PdfName("JavaScript");
        public static readonly PdfName JBIG2DECODE = new PdfName("JBIG2Decode");
        public static readonly PdfName JBIG2GLOBALS = new PdfName("JBIG2Globals");
        public static readonly PdfName JPXDECODE = new PdfName("JPXDecode");
        public static readonly PdfName JS = new PdfName("JS");
        public static readonly PdfName JUSTIFY = new PdfName("Justify");

        public static readonly PdfName K = new PdfName("K");
        public static readonly PdfName KEYWORDS = new PdfName("Keywords");
        public static readonly PdfName KIDS = new PdfName("Kids");

        public static readonly PdfName L = new PdfName("L");
        public static readonly PdfName L2R = new PdfName("L2R");
        public static readonly PdfName LAB = new PdfName("Lab");
        public static readonly PdfName LANG = new PdfName("Lang");
        public static readonly PdfName LANGUAGE = new PdfName("Language");
        public static readonly PdfName LAST = new PdfName("Last");
        public static readonly PdfName LASTCHAR = new PdfName("LastChar");
        public static readonly PdfName LASTPAGE = new PdfName("LastPage");
        public static readonly PdfName LAUNCH = new PdfName("Launch");
        public static readonly PdfName LBL = new PdfName("Lbl");
        public static readonly PdfName LBODY = new PdfName("LBody");
        public static readonly PdfName LENGTH = new PdfName("Length");
        public static readonly PdfName LENGTH1 = new PdfName("Length1");
        public static readonly PdfName LI = new PdfName("LI");
        public static readonly PdfName LIMITS = new PdfName("Limits");
        public static readonly PdfName LINE = new PdfName("Line");
        public static readonly PdfName LINEAR = new PdfName("Linear");
        public static readonly PdfName LINEHEIGHT = new PdfName("LineHeight");
        public static readonly PdfName LINK = new PdfName("Link");
        public static readonly PdfName LIST = new PdfName("List");
        public static readonly PdfName LISTMODE = new PdfName("ListMode");
        public static readonly PdfName LISTNUMBERING = new PdfName("ListNumbering");
        public static readonly PdfName LOCATION = new PdfName("Location");
        public static readonly PdfName LOCK = new PdfName("Lock");
        public static readonly PdfName LOCKED = new PdfName("Locked");
        public static readonly PdfName LOWERALPHA = new PdfName("LowerAlpha");
        public static readonly PdfName LOWERROMAN = new PdfName("LowerRoman");
        public static readonly PdfName LPTS = new PdfName("LPTS");  //  A name of an attribute.
        public static readonly PdfName LZWDECODE = new PdfName("LZWDecode");

        public static readonly PdfName M = new PdfName("M");
        public static readonly PdfName MAC = new PdfName("Mac");
        public static readonly PdfName MATERIAL = new PdfName("Material");
        public static readonly PdfName MATRIX = new PdfName("Matrix");
        public static readonly PdfName MAC_EXPERT_ENCODING = new PdfName("MacExpertEncoding"); /** A name of an encoding */
        public static readonly PdfName MAC_ROMAN_ENCODING = new PdfName("MacRomanEncoding"); /** A name of an encoding */
        public static readonly PdfName MARKED = new PdfName("Marked");
        public static readonly PdfName MARKINFO = new PdfName("MarkInfo");
        public static readonly PdfName MASK = new PdfName("Mask");
        public static readonly PdfName MAX_LOWER_CASE = new PdfName("max");
        public static readonly PdfName MAX_CAMEL_CASE = new PdfName("Max");
        public static readonly PdfName MAXLEN = new PdfName("MaxLen");
        public static readonly PdfName MEDIABOX = new PdfName("MediaBox");
        public static readonly PdfName MCID = new PdfName("MCID");
        public static readonly PdfName MCR = new PdfName("MCR");
        public static readonly PdfName MEASURE = new PdfName("Measure");
        public static readonly PdfName METADATA = new PdfName("Metadata");
        public static readonly PdfName MIN_LOWER_CASE = new PdfName("min");
        public static readonly PdfName MIN_CAMEL_CASE = new PdfName("Min");
        public static readonly PdfName MK = new PdfName("MK");
        public static readonly PdfName MMTYPE1 = new PdfName("MMType1");
        public static readonly PdfName MODDATE = new PdfName("ModDate");

        public static readonly PdfName N = new PdfName("N");
        public static readonly PdfName N0 = new PdfName("n0");
        public static readonly PdfName N1 = new PdfName("n1");
        public static readonly PdfName N2 = new PdfName("n2");
        public static readonly PdfName N3 = new PdfName("n3");
        public static readonly PdfName N4 = new PdfName("n4");
        public static new readonly PdfName NAME = new PdfName("Name");
        public static readonly PdfName NAMED = new PdfName("Named");
        public static readonly PdfName NAMES = new PdfName("Names");
        public static readonly PdfName NAVIGATION = new PdfName("Navigation");
        public static readonly PdfName NAVIGATIONPANE = new PdfName("NavigationPane");
        public static readonly PdfName NEAR = new PdfName("Near");
        public static readonly PdfName NEEDAPPEARANCES = new PdfName("NeedAppearances");
        public static readonly PdfName NEWWINDOW = new PdfName("NewWindow");
        public static readonly PdfName NEXT = new PdfName("Next");
        public static readonly PdfName NEXTPAGE = new PdfName("NextPage");
        public static readonly PdfName NM = new PdfName("NM");
        public static readonly PdfName NONE = new PdfName("None");
        public static readonly PdfName NONFULLSCREENPAGEMODE = new PdfName("NonFullScreenPageMode");
        public static readonly PdfName NONSTRUCT = new PdfName("NonStruct");
        public static readonly PdfName NOT = new PdfName("Not");
        public static readonly PdfName NOTE = new PdfName("Note");
        public static readonly PdfName NUMBERFORMAT = new PdfName("NumberFormat");
        public static readonly PdfName NUMCOPIES = new PdfName("NumCopies");
        public static readonly PdfName NUMS = new PdfName("Nums");

        public static readonly PdfName O = new PdfName("O");
        public static readonly PdfName OBJ = new PdfName("Obj"); /** A name used with Document Structure         */
        public static readonly PdfName OBJR = new PdfName("OBJR"); /**      * a name used with Document Structure         */
        public static readonly PdfName OBJSTM = new PdfName("ObjStm");
        public static readonly PdfName OC = new PdfName("OC");
        public static readonly PdfName OCG = new PdfName("OCG");
        public static readonly PdfName OCGS = new PdfName("OCGs");
        public static readonly PdfName OCMD = new PdfName("OCMD");
        public static readonly PdfName OCPROPERTIES = new PdfName("OCProperties");
        public static readonly PdfName OCSP = new PdfName("OCSP");
        public static readonly PdfName OCSPS = new PdfName("OCSPs");
        public static readonly PdfName OE = new PdfName("OE");
        public static readonly PdfName Off_ = new PdfName("Off");
        public static readonly PdfName OFF = new PdfName("OFF");
        public static readonly PdfName ON = new PdfName("ON");
        public static readonly PdfName ONECOLUMN = new PdfName("OneColumn");
        public static readonly PdfName OPEN = new PdfName("Open");
        public static readonly PdfName OPENACTION = new PdfName("OpenAction");
        public static readonly PdfName OP = new PdfName("OP");
        public static readonly PdfName op_ = new PdfName("op");
        public static readonly PdfName OPM = new PdfName("OPM");
        public static readonly PdfName OPT = new PdfName("Opt");
        public static readonly PdfName OR = new PdfName("Or");
        public static readonly PdfName ORDER = new PdfName("Order");
        public static readonly PdfName ORDERING = new PdfName("Ordering");
        public static readonly PdfName ORG = new PdfName("Org");
        public static readonly PdfName OSCILLATING = new PdfName("Oscillating");
        public static readonly PdfName OUTLINES = new PdfName("Outlines");
        public static readonly PdfName OUTPUTCONDITION = new PdfName("OutputCondition");
        public static readonly PdfName OUTPUTCONDITIONIDENTIFIER = new PdfName("OutputConditionIdentifier");
        public static readonly PdfName OUTPUTINTENT = new PdfName("OutputIntent");
        public static readonly PdfName OUTPUTINTENTS = new PdfName("OutputIntents");

        public static readonly PdfName P = new PdfName("P");
        public static readonly PdfName PAGE = new PdfName("Page");
        public static readonly PdfName PAGEELEMENT = new PdfName("PageElement");
        public static readonly PdfName PAGELABELS = new PdfName("PageLabels");
        public static readonly PdfName PAGELAYOUT = new PdfName("PageLayout");
        public static readonly PdfName PAGEMODE = new PdfName("PageMode");
        public static readonly PdfName PAGES = new PdfName("Pages");
        public static readonly PdfName PAINTTYPE = new PdfName("PaintType");
        public static readonly PdfName PANOSE = new PdfName("Panose");
        public static readonly PdfName PARAMS = new PdfName("Params");
        public static readonly PdfName PARENT = new PdfName("Parent");
        public static readonly PdfName PARENTTREE = new PdfName("ParentTree");
        public static readonly PdfName PARENTTREENEXTKEY = new PdfName("ParentTreeNextKey"); /**        * A name used in defining Document Structure.         */
        public static readonly PdfName PART = new PdfName("Part");
        public static readonly PdfName PASSCONTEXTCLICK = new PdfName("PassContextClick");
        public static readonly PdfName PATTERN = new PdfName("Pattern");
        public static readonly PdfName PATTERNTYPE = new PdfName("PatternType");
        public static readonly PdfName PC = new PdfName("PC");
        public static readonly PdfName PDF = new PdfName("PDF");
        public static readonly PdfName PDFDOCENCODING = new PdfName("PDFDocEncoding");
        public static readonly PdfName PDU = new PdfName("PDU"); /**        * A name of an attribute.         */
        public static readonly PdfName PERCEPTUAL = new PdfName("Perceptual");
        public static readonly PdfName PERMS = new PdfName("Perms");
        public static readonly PdfName PG = new PdfName("Pg");
        public static readonly PdfName PI = new PdfName("PI");
        public static readonly PdfName PICKTRAYBYPDFSIZE = new PdfName("PickTrayByPDFSize");
        public static readonly PdfName PLAYCOUNT = new PdfName("PlayCount");
        public static readonly PdfName PO = new PdfName("PO");
        public static readonly PdfName POLYGON = new PdfName("Polygon");
        public static readonly PdfName POLYLINE = new PdfName("Polyline");
        public static readonly PdfName POPUP = new PdfName("Popup");
        public static readonly PdfName POSITION = new PdfName("Position");
        public static readonly PdfName PREDICTOR = new PdfName("Predictor");
        public static readonly PdfName PREFERRED = new PdfName("Preferred");
        public static readonly PdfName PRESENTATION = new PdfName("Presentation");
        public static readonly PdfName PRESERVERB = new PdfName("PreserveRB");
        public static readonly PdfName PREV = new PdfName("Prev");
        public static readonly PdfName PREVPAGE = new PdfName("PrevPage");
        public static readonly PdfName PRINT = new PdfName("Print");
        public static readonly PdfName PRINTAREA = new PdfName("PrintArea");
        public static readonly PdfName PRINTCLIP = new PdfName("PrintClip");
        public static readonly PdfName PRINTPAGERANGE = new PdfName("PrintPageRange");
        public static readonly PdfName PRINTSCALING = new PdfName("PrintScaling");
        public static readonly PdfName PRINTSTATE = new PdfName("PrintState");
        public static readonly PdfName PRIVATE = new PdfName("Private");
        public static readonly PdfName PROCSET = new PdfName("ProcSet");
        public static readonly PdfName PRODUCER = new PdfName("Producer");
        public static readonly PdfName PROJCS = new PdfName("PROJCS"); /**        * A name of an attribute.         */
        public static readonly PdfName PROPERTIES = new PdfName("Properties");
        public static readonly PdfName PS = new PdfName("PS");
        public static readonly PdfName PTDATA = new PdfName("PtData");
        public static readonly PdfName PUBSEC = new PdfName("Adobe.PubSec");
        public static readonly PdfName PV = new PdfName("PV");

        public static readonly PdfName Q = new PdfName("Q");
        public static readonly PdfName QUADPOINTS = new PdfName("QuadPoints");
        public static readonly PdfName QUOTE = new PdfName("Quote");

        public static readonly PdfName R = new PdfName("R");
        public static readonly PdfName R2L = new PdfName("R2L");
        public static readonly PdfName RANGE = new PdfName("Range");
        public static readonly PdfName RBGROUPS = new PdfName("RBGroups");
        public static readonly PdfName RC = new PdfName("RC");
        public static readonly PdfName RD = new PdfName("RD");
        public static readonly PdfName REASON = new PdfName("Reason");
        public static readonly PdfName RECIPIENTS = new PdfName("Recipients");
        public static readonly PdfName RECT = new PdfName("Rect");
        public static readonly PdfName REFERENCE = new PdfName("Reference");
        public static readonly PdfName REGISTRY = new PdfName("Registry");
        public static readonly PdfName REGISTRYNAME = new PdfName("RegistryName");
        public static readonly PdfName RELATIVECOLORIMETRIC = new PdfName("RelativeColorimetric");
        public static readonly PdfName RENDITION = new PdfName("Rendition");
        public static readonly PdfName RESETFORM = new PdfName("ResetForm");
        public static readonly PdfName RESOURCES = new PdfName("Resources");
        public static readonly PdfName RI = new PdfName("RI");
        public static readonly PdfName RICHMEDIA = new PdfName("RichMedia");
        public static readonly PdfName RICHMEDIAACTIVATION = new PdfName("RichMediaActivation");
        public static readonly PdfName RICHMEDIAANIMATION = new PdfName("RichMediaAnimation");
        public static readonly PdfName RICHMEDIACOMMAND = new PdfName("RichMediaCommand");
        public static readonly PdfName RICHMEDIACONFIGURATION = new PdfName("RichMediaConfiguration");
        public static readonly PdfName RICHMEDIACONTENT = new PdfName("RichMediaContent");
        public static readonly PdfName RICHMEDIADEACTIVATION = new PdfName("RichMediaDeactivation");
        public static readonly PdfName RICHMEDIAEXECUTE = new PdfName("RichMediaExecute");
        public static readonly PdfName RICHMEDIAINSTANCE = new PdfName("RichMediaInstance");
        public static readonly PdfName RICHMEDIAPARAMS = new PdfName("RichMediaParams");
        public static readonly PdfName RICHMEDIAPOSITION = new PdfName("RichMediaPosition");
        public static readonly PdfName RICHMEDIAPRESENTATION = new PdfName("RichMediaPresentation");
        public static readonly PdfName RICHMEDIASETTINGS = new PdfName("RichMediaSettings");
        public static readonly PdfName RICHMEDIAWINDOW = new PdfName("RichMediaWindow");
        public static readonly PdfName RL = new PdfName("RL");
        public static readonly PdfName ROLEMAP = new PdfName("RoleMap");
        public static readonly PdfName ROOT = new PdfName("Root");
        public static readonly PdfName ROTATE = new PdfName("Rotate");
        public static readonly PdfName ROW = new PdfName("Row");
        public static readonly PdfName ROWS = new PdfName("Rows");
        public static readonly PdfName ROWSPAN = new PdfName("RowSpan");
        public static readonly PdfName RT = new PdfName("RT");
        public static readonly PdfName RUBY = new PdfName("Ruby");
        public static readonly PdfName RUNLENGTHDECODE = new PdfName("RunLengthDecode");
        public static readonly PdfName RV = new PdfName("RV");

        public static readonly PdfName S = new PdfName("S");
        public static readonly PdfName SATURATION = new PdfName("Saturation");
        public static readonly PdfName SCHEMA = new PdfName("Schema");
        public static readonly PdfName SCOPE = new PdfName("Scope");
        public static readonly PdfName SCREEN = new PdfName("Screen");
        public static readonly PdfName SCRIPTS = new PdfName("Scripts");
        public static readonly PdfName SECT = new PdfName("Sect");
        public static readonly PdfName SEPARATION = new PdfName("Separation");
        public static readonly PdfName SETOCGSTATE = new PdfName("SetOCGState");
        public static readonly PdfName SETTINGS = new PdfName("Settings");
        public static readonly PdfName SHADING = new PdfName("Shading");
        public static readonly PdfName SHADINGTYPE = new PdfName("ShadingType");
        public static readonly PdfName SHIFT_JIS = new PdfName("Shift-JIS");
        public static readonly PdfName SIG = new PdfName("Sig");
        public static readonly PdfName SIGFIELDLOCK = new PdfName("SigFieldLock");
        public static readonly PdfName SIGFLAGS = new PdfName("SigFlags");
        public static readonly PdfName SIGREF = new PdfName("SigRef");
        public static readonly PdfName SIMPLEX = new PdfName("Simplex");
        public static readonly PdfName SINGLEPAGE = new PdfName("SinglePage");
        public static readonly PdfName SIZE = new PdfName("Size");
        public static readonly PdfName SMASK = new PdfName("SMask");
        public static readonly PdfName SORT = new PdfName("Sort");
        public static readonly PdfName SOUND = new PdfName("Sound");
        public static readonly PdfName SPACEAFTER = new PdfName("SpaceAfter");
        public static readonly PdfName SPACEBEFORE = new PdfName("SpaceBefore");
        public static readonly PdfName SPAN = new PdfName("Span");
        public static readonly PdfName SPEED = new PdfName("Speed");
        public static readonly PdfName SPLIT = new PdfName("Split");
        public static readonly PdfName SQUARE = new PdfName("Square");
        public static readonly PdfName SQUIGGLY = new PdfName("Squiggly");
        public static readonly PdfName SS = new PdfName("SS");
        public static readonly PdfName ST = new PdfName("St");
        public static readonly PdfName STAMP = new PdfName("Stamp");
        public static readonly PdfName STANDARD = new PdfName("Standard");
        public static readonly PdfName START = new PdfName("Start");
        public static readonly PdfName STARTINDENT = new PdfName("StartIndent");
        public static readonly PdfName STATE = new PdfName("State");
        public static readonly PdfName STDCF = new PdfName("StdCF");
        public static readonly PdfName STEMV = new PdfName("StemV");
        public static readonly PdfName STMF = new PdfName("StmF");
        public static readonly PdfName STRF = new PdfName("StrF");
        public static readonly PdfName STRIKEOUT = new PdfName("StrikeOut");
        public static readonly PdfName STRUCTELEM = new PdfName("StructElem");
        public static readonly PdfName STRUCTPARENT = new PdfName("StructParent");
        public static readonly PdfName STRUCTPARENTS = new PdfName("StructParents");
        public static readonly PdfName STRUCTTREEROOT = new PdfName("StructTreeRoot");
        public static readonly PdfName STYLE = new PdfName("Style");
        public static readonly PdfName SUBFILTER = new PdfName("SubFilter");
        public static readonly PdfName SUBJECT = new PdfName("Subject");
        public static readonly PdfName SUBMITFORM = new PdfName("SubmitForm");
        public static readonly PdfName SUBTYPE = new PdfName("Subtype");
        public static readonly PdfName SUPPLEMENT = new PdfName("Supplement");
        public static readonly PdfName SV = new PdfName("SV");
        public static readonly PdfName SW = new PdfName("SW");
        public static readonly PdfName SYMBOL = new PdfName("Symbol"); /** A name of a base 14 type 1 font */

        public static readonly PdfName T = new PdfName("T");
        public static readonly PdfName TA = new PdfName("TA");
        public static readonly PdfName TABLE = new PdfName("Table");
        public static readonly PdfName TABS = new PdfName("Tabs");
        public static readonly PdfName TBODY = new PdfName("TBody");
        public static readonly PdfName TD = new PdfName("TD");
        public static PdfName TR = new PdfName("TR");
        public static readonly PdfName TEXT = new PdfName("Text");
        public static readonly PdfName TEXTALIGN = new PdfName("TextAlign");
        public static readonly PdfName TEXTDECORATIONCOLOR = new PdfName("TextDecorationColor");
        public static readonly PdfName TEXTDECORATIONTHICKNESS = new PdfName("TextDecorationThickness");
        public static readonly PdfName TEXTDECORATIONTYPE = new PdfName("TextDecorationType");
        public static readonly PdfName TEXTINDENT = new PdfName("TextIndent");
        public static readonly PdfName TFOOT = new PdfName("TFoot");
        public static readonly PdfName TH = new PdfName("TH");
        public static readonly PdfName THEAD = new PdfName("THead");
        public static readonly PdfName THUMB = new PdfName("Thumb");
        public static readonly PdfName THREADS = new PdfName("Threads");
        public static readonly PdfName TI = new PdfName("TI");
        public static readonly PdfName TIME = new PdfName("Time");
        public static readonly PdfName TILINGTYPE = new PdfName("TilingType");
        public static readonly PdfName TIMES_ROMAN = new PdfName("Times-Roman"); /** A name of a base 14 type 1 font */
        public static readonly PdfName TIMES_BOLD = new PdfName("Times-Bold"); /** A name of a base 14 type 1 font */
        public static readonly PdfName TIMES_ITALIC = new PdfName("Times-Italic"); /** A name of a base 14 type 1 font */
        public static readonly PdfName TIMES_BOLDITALIC = new PdfName("Times-BoldItalic"); /** A name of a base 14 type 1 font */
        public static readonly PdfName TITLE = new PdfName("Title");
        public static readonly PdfName TK = new PdfName("TK");
        public static readonly PdfName TM = new PdfName("TM");
        public static readonly PdfName TOC = new PdfName("TOC");
        public static readonly PdfName TOCI = new PdfName("TOCI");
        public static readonly PdfName TOGGLE = new PdfName("Toggle");
        public static readonly PdfName TOOLBAR = new PdfName("Toolbar");
        public static readonly PdfName TOUNICODE = new PdfName("ToUnicode");
        public static readonly PdfName TP = new PdfName("TP");
        public static readonly PdfName TABLEROW = new PdfName("TR");
        public static readonly PdfName TRANS = new PdfName("Trans");
        public static readonly PdfName TRANSFORMPARAMS = new PdfName("TransformParams");
        public static readonly PdfName TRANSFORMMETHOD = new PdfName("TransformMethod");
        public static readonly PdfName TRANSPARENCY = new PdfName("Transparency");
        public static readonly PdfName TRANSPARENT = new PdfName("Transparent");
        public static readonly PdfName TRAPPED = new PdfName("Trapped");
        public static readonly PdfName TRIMBOX = new PdfName("TrimBox");
        public static readonly PdfName TRUETYPE = new PdfName("TrueType");
        public static readonly PdfName TS = new PdfName("TS");
        public static readonly PdfName TTL = new PdfName("Ttl");
        public static readonly PdfName TU = new PdfName("TU");
        public static readonly PdfName TWOCOLUMNLEFT = new PdfName("TwoColumnLeft");
        public static readonly PdfName TWOCOLUMNRIGHT = new PdfName("TwoColumnRight");
        public static readonly PdfName TWOPAGELEFT = new PdfName("TwoPageLeft");
        public static readonly PdfName TWOPAGERIGHT = new PdfName("TwoPageRight");
        public static readonly PdfName TX = new PdfName("Tx");
        public static readonly PdfName TYPE = new PdfName("Type");
        public static readonly PdfName TYPE0 = new PdfName("Type0");
        public static readonly PdfName TYPE1 = new PdfName("Type1");
        public static readonly PdfName TYPE3 = new PdfName("Type3");        /** A name of an attribute. */

        public static readonly PdfName U = new PdfName("U"); /** A name of an attribute. */
        public static readonly PdfName UE = new PdfName("UE");
        public static readonly PdfName UF = new PdfName("UF");         /** A name of an attribute. */
        public static readonly PdfName UHC = new PdfName("UHC");         /** A name of an attribute. */
        public static readonly PdfName UNDERLINE = new PdfName("Underline"); /** A name of an attribute. */
        public static readonly PdfName UNIX = new PdfName("Unix");
        public static readonly PdfName UPPERALPHA = new PdfName("UpperAlpha");
        public static readonly PdfName UPPERROMAN = new PdfName("UpperRoman");
        public static readonly PdfName UR = new PdfName("UR");
        public static readonly PdfName UR3 = new PdfName("UR3");
        public static readonly PdfName URI = new PdfName("URI");
        public static readonly PdfName URL = new PdfName("URL");
        public static readonly PdfName USAGE = new PdfName("Usage");
        public static readonly PdfName USEATTACHMENTS = new PdfName("UseAttachments");
        public static readonly PdfName USENONE = new PdfName("UseNone");
        public static readonly PdfName USEOC = new PdfName("UseOC");
        public static readonly PdfName USEOUTLINES = new PdfName("UseOutlines");
        public static readonly PdfName USER = new PdfName("User");
        public static readonly PdfName USERPROPERTIES = new PdfName("UserProperties");
        public static readonly PdfName USERUNIT = new PdfName("UserUnit");
        public static readonly PdfName USETHUMBS = new PdfName("UseThumbs");
        public static readonly PdfName UTF_8 = new PdfName("utf_8");

        public static readonly PdfName V = new PdfName("V");
        public static readonly PdfName V2 = new PdfName("V2");
        public static readonly PdfName VALIGN = new PdfName("VAlign");
        public static readonly PdfName VE = new PdfName("VE");
        public static readonly PdfName VERISIGN_PPKVS = new PdfName("VeriSign.PPKVS");
        public static readonly PdfName VERSION = new PdfName("Version");
        public static readonly PdfName VERTICES = new PdfName("Vertices");
        public static readonly PdfName VIDEO = new PdfName("Video");
        public static readonly PdfName VIEW = new PdfName("View");
        public static readonly PdfName VIEWS = new PdfName("Views");
        public static readonly PdfName VIEWAREA = new PdfName("ViewArea");
        public static readonly PdfName VIEWCLIP = new PdfName("ViewClip");
        public static readonly PdfName VIEWERPREFERENCES = new PdfName("ViewerPreferences");
        public static readonly PdfName VIEWPORT = new PdfName("Viewport");
        public static readonly PdfName VIEWSTATE = new PdfName("ViewState");
        public static readonly PdfName VISIBLEPAGES = new PdfName("VisiblePages");
        public static readonly PdfName VOFFSET = new PdfName("VOffset");
        public static readonly PdfName VP = new PdfName("VP");
        public static readonly PdfName VRI = new PdfName("VRI");

        public static readonly PdfName W = new PdfName("W"); /** A name of an attribute. */
        public static readonly PdfName W2 = new PdfName("W2"); /** A name of an attribute. */
        public static readonly PdfName WARICHU = new PdfName("Warichu");
        public static readonly PdfName WC = new PdfName("WC"); /** A name of an attribute. */
        public static readonly PdfName WIDGET = new PdfName("Widget"); /** A name of an attribute. */
        public static readonly PdfName WIDTH = new PdfName("Width"); /** A name of an attribute. */
        public static readonly PdfName WIDTHS = new PdfName("Widths");
        public static readonly PdfName WIN = new PdfName("Win"); /** A name of an encoding */
        public static readonly PdfName WIN_ANSI_ENCODING = new PdfName("WinAnsiEncoding"); /** A name of an encoding */
        public static readonly PdfName WINDOW = new PdfName("Window");
        public static readonly PdfName WINDOWED = new PdfName("Windowed");
        public static readonly PdfName WIPE = new PdfName("Wipe"); /** A name of an encoding */
        public static readonly PdfName WHITEPOINT = new PdfName("WhitePoint");
        public static readonly PdfName WKT = new PdfName("WKT"); /*** A name of an attribute. */
        public static readonly PdfName WP = new PdfName("WP");
        public static readonly PdfName WS = new PdfName("WS");         /** A name of an encoding */

        public static readonly PdfName X = new PdfName("X");
        public static readonly PdfName XA = new PdfName("XA");
        public static readonly PdfName XD = new PdfName("XD");
        public static readonly PdfName XFA = new PdfName("XFA");
        public static readonly PdfName XML = new PdfName("XML");
        public static readonly PdfName XOBJECT = new PdfName("XObject");
        public static readonly PdfName XPTS = new PdfName("XPTS");
        public static readonly PdfName XREF = new PdfName("XRef");
        public static readonly PdfName XREFSTM = new PdfName("XRefStm");
        public static readonly PdfName XSTEP = new PdfName("XStep");
        public static readonly PdfName XYZ = new PdfName("XYZ");

        public static readonly PdfName YSTEP = new PdfName("YStep");

        public static readonly PdfName ZADB = new PdfName("ZaDb");
        public static readonly PdfName ZAPFDINGBATS = new PdfName("ZapfDingbats"); /** A name of a base 14 type 1 font */
        public static readonly PdfName ZOOM = new PdfName("Zoom");

        public static Dictionary<string, PdfName> staticNames;  // map strings to all known static names

        /**
         * Use reflection to cache all the static public readonly names so
         * future <code>PdfName</code> additions don't have to be "added twice".
         * A bit less efficient (around 50ms spent here on a 2.2ghz machine),
         *  but Much Less error prone.
         */
        static PdfName()
        {
            FieldInfo[] fields = typeof(PdfName).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);
            staticNames = new Dictionary<string, PdfName>(fields.Length);
            try
            {
                for (int fldIdx = 0; fldIdx < fields.Length; ++fldIdx)
                {
                    FieldInfo curFld = fields[fldIdx];
                    if (curFld.FieldType.Equals(typeof(PdfName)))
                    {
                        PdfName name = (PdfName)curFld.GetValue(null);
                        staticNames[DecodeName(name.ToString())] = name;
                    }
                }
            }
            catch { }
        }

        private int hash = 0;         // CLASS VARIABLES

        // create a new PdfName
        public PdfName(string name)  : base(PdfObject.NAME)
        {
            // Note: The minimum number of characters in a name is 0, the maximum is 127 (the '/' not included)
            int length = name.Length;
            if (length > 127)
            {
                Console.WriteLine("warning: PdfName " + name + " is too long (" + length + " characters)");
            }
            bytes = EncodeName(name);
        }

        public PdfName(byte[] bytes)  : base(PdfObject.NAME, bytes)
        {
        }

        // methods

        /**
         * Compares this object with the specified object for order.  Returns a
         * negative int, zero, or a positive int as this object is less
         * than, equal to, or greater than the specified object.<p>
         *
         *
         * @param   object the Object to be compared.
         * @return  a negative int, zero, or a positive int as this object
         *        is less than, equal to, or greater than the specified object.
         *
         * @throws Exception if the specified object's type prevents it
         *         from being compared to this Object.
         */
        public int CompareTo(PdfName name)
        {
            byte[] myBytes = bytes;
            byte[] objBytes = name.bytes;
            int len = Math.Min(myBytes.Length, objBytes.Length);
            for (int i = 0; i < len; i++)
            {
                if (myBytes[i] > objBytes[i])
                    return 1;

                if (myBytes[i] < objBytes[i])
                    return -1;
            }
            if (myBytes.Length < objBytes.Length)
                return -1;
            if (myBytes.Length > objBytes.Length)
                return 1;
            return 0;
        }

        /**
         * Indicates whether some other object is "equal to" this one.
         *
         * @param   obj   the reference object with which to compare.
         * @return  <code>true</code> if this object is the same as the obj
         *          argument; <code>false</code> otherwise.
         */
        public override bool Equals(Object obj)
        {
            if (this == obj)
                return true;
            if (obj is PdfName)
                return CompareTo((PdfName)obj) == 0;
            return false;
        }

        /**
         * Returns a hash code value for the object. This method is
         * supported for the benefit of hashtables such as those provided by
         * <code>java.util.Hashtable</code>.
         *
         * @return  a hash code value for this object.
         */
        public override int GetHashCode()
        {
            int h = hash;
            if (h == 0)
            {
                int ptr = 0;
                int len = bytes.Length;

                for (int i = 0; i < len; i++)
                    h = 31 * h + (bytes[ptr++] & 0xff);
                hash = h;
            }
            return h;
        }

        /**
        * Encodes a plain name given in the unescaped form "AB CD" into "/AB#20CD".
        *
        * @param name the name to encode
        * @return the encoded name
        * @since	2.1.5
        */
        public static byte[] EncodeName(String name)
        {
            int length = name.Length;
            // every special character has to be substituted
            ByteBuffer pdfName = new ByteBuffer(length + 20);
            pdfName.Append('/');
            char[] chars = name.ToCharArray();
            char character;
            // loop over all the characters
            foreach (char cc in chars)
            {
                character = (char)(cc & 0xff);
                // special characters are escaped (reference manual p.39)
                switch (character)
                {
                    case ' ':
                    case '%':
                    case '(':
                    case ')':
                    case '<':
                    case '>':
                    case '[':
                    case ']':
                    case '{':
                    case '}':
                    case '/':
                    case '#':
                        pdfName.Append('#');
                        pdfName.Append(System.Convert.ToString(character, 16));
                        break;
                    default:
                        if (character > 126 || character < 32)
                        {
                            pdfName.Append('#');
                            if (character < 16)
                                pdfName.Append('0');
                            pdfName.Append(System.Convert.ToString(character, 16));
                        }
                        else
                            pdfName.Append(character);
                        break;
                }
            }
            return pdfName.ToByteArray();
        }

        /** Decodes an escaped name in the form "/AB#20CD" into "AB CD".
         * @param name the name to decode
         * @return the decoded name
         */
        public static string DecodeName(string name)
        {
            StringBuilder buf = new StringBuilder();
            int len = name.Length;
            for (int k = 1; k < len; ++k)
            {
                char c = name[k];
                if (c == '#')
                {
                    c = (char)((PRTokeniser.GetHex(name[k + 1]) << 4) + PRTokeniser.GetHex(name[k + 2]));
                    k += 2;
                }
                buf.Append(c);
            }
            return buf.ToString();
        }
    }
}
