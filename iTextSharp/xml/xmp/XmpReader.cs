using System;
using System.IO;
using System.Xml;
using System.Text;

namespace iTextSharp.text.xml.xmp 
{
    /**
    * Reads an XMP stream into an org.w3c.dom.Document objects.
    * Allows you to replace the contents of a specific tag.
    * @since 2.1.3
    */
    public class XmpReader 
    {
        private XmlDocument domDocument;
        
        /**
        * Constructs an XMP reader
        * @param	bytes	the XMP content
        * @throws ExceptionConverter 
        * @throws IOException 
        * @throws SAXException 
        */
	    public XmpReader(byte[] bytes)  {
            MemoryStream bout = new MemoryStream();
            bout.Write(bytes, 0, bytes.Length);
            bout.Seek(0, SeekOrigin.Begin);
            XmlTextReader xtr = new XmlTextReader(bout);
            domDocument = new XmlDocument();
            domDocument.PreserveWhitespace = true;
            domDocument.Load(xtr);
	    }
    	
	    /**
	    * Replaces the content of a tag.
	    * @param	namespaceURI	the URI of the namespace
	    * @param	localName		the tag name
	    * @param	value			the new content for the tag
	    * @return	true if the content was successfully replaced
	    * @since	2.1.6 the return type has changed from void to boolean
	    */
	    public bool ReplaceNode(String namespaceURI, String localName, String value) {
		    XmlNodeList nodes = domDocument.GetElementsByTagName(localName, namespaceURI);
		    XmlNode node;
		    if (nodes.Count == 0)
			    return false;
		    for (int i = 0; i < nodes.Count; i++) {
			    node = nodes[i];
			    SetNodeText(domDocument, node, value);
		    }
		    return true;
	    }    
        
        /**
        * Replaces the content of an attribute in the description tag.
        * @param    namespaceURI    the URI of the namespace
        * @param    localName       the tag name
        * @param    value           the new content for the tag
        * @return   true if the content was successfully replaced
        * @since    5.0.0 the return type has changed from void to boolean
        */
        public bool ReplaceDescriptionAttribute(String namespaceURI, String localName, String value) {
            XmlNodeList descNodes = domDocument.GetElementsByTagName("Description", "http://www.w3.org/1999/02/22-rdf-syntax-ns#");
            if(descNodes.Count == 0) {
                return false;
            }
            XmlNode node;
            for(int i = 0; i < descNodes.Count; i++) {
                node = descNodes.Item(i);
                XmlNode attr = node.Attributes.GetNamedItem(localName, namespaceURI);
                if(attr != null) {
                    attr.Value = value;
                    return true;
                }
            }
            return false;
        }

        /**
	    * Adds a tag.
	    * @param	namespaceURI	the URI of the namespace
	    * @param	parent			the tag name of the parent
	    * @param	localName		the name of the tag to add
	    * @param	value			the new content for the tag
	    * @return	true if the content was successfully added
	    * @since	2.1.6
	    */
	    public bool Add(String parent, String namespaceURI, String localName, String value) {
		    XmlNodeList nodes = domDocument.GetElementsByTagName(parent);
		    if (nodes.Count == 0)
			    return false;
		    XmlNode pNode;
		    XmlNode node;
            String prefix;
		    for (int i = 0; i < nodes.Count; i++) {
			    pNode = nodes[i];
			    XmlAttributeCollection attrs = pNode.Attributes;
			    for (int j = 0; j < attrs.Count; j++) {
				    node = attrs[j];
				    if (namespaceURI.Equals(node.Value)) {
                        prefix = node.LocalName;
                        node = domDocument.CreateElement(localName, namespaceURI);
                        node.Prefix = prefix;
					    node.AppendChild(domDocument.CreateTextNode(value));
					    pNode.AppendChild(node);
					    return true;
				    }
			    }
		    }
		    return false;
	    }
    	
        /**
        * Sets the text of this node. All the child's node are deleted and a new
        * child text node is created.
        * @param domDocument the <CODE>Document</CODE> that contains the node
        * @param n the <CODE>Node</CODE> to add the text to
        * @param value the text to add
        */
        public bool SetNodeText(XmlDocument domDocument, XmlNode n, String value) {
            if (n == null)
                return false;
            XmlNode nc = null;
            while ((nc = n.FirstChild) != null) {
                n.RemoveChild(nc);
            }
            n.AppendChild(domDocument.CreateTextNode(value));
            return true;
        }
    	
        /**
         * Writes the document to a byte array.
         */
        public byte[] SerializeDoc() {
            XmlDomWriter xw = new XmlDomWriter();
            MemoryStream fout = new MemoryStream();
            xw.SetOutput(fout, null);
            byte[] b = new UTF8Encoding(false).GetBytes(XmpWriter.XPACKET_PI_BEGIN);
            fout.Write(b, 0, b.Length);
            fout.Flush();
            XmlNodeList xmpmeta = domDocument.GetElementsByTagName("x:xmpmeta");
            xw.Write(xmpmeta[0]);
            fout.Flush();
            b = new UTF8Encoding(false).GetBytes(XmpWriter.EXTRASPACE);
            for (int i = 0; i < 2; i++) {
                fout.Write(b, 0, b.Length);
            }
            b = new UTF8Encoding(false).GetBytes(XmpWriter.XPACKET_PI_END_W);
            fout.Write(b, 0, b.Length);
            fout.Close();
            return fout.ToArray();
        }
    }
}
