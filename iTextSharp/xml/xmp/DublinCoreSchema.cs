using System;
/*
 * $Id: DublinCoreSchema.cs 551 2013-06-03 10:01:29Z pavel-alay $
 * 
 *
 * This file is part of the iText project.
 * Copyright (c) 1998-2012 1T3XT BVBA
 * Authors: Bruno Lowagie, Paulo Soares, et al.
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU Affero General Public License version 3
 * as published by the Free Software Foundation with the addition of the
 * following permission added to Section 15 as permitted in Section 7(a):
 * FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY 1T3XT,
 * 1T3XT DISCLAIMS THE WARRANTY OF NON INFRINGEMENT OF THIRD PARTY RIGHTS.
 *
 * This program is distributed in the hope that it will be useful, but
 * WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
 * or FITNESS FOR A PARTICULAR PURPOSE.
 * See the GNU Affero General Public License for more details.
 * You should have received a copy of the GNU Affero General Public License
 * along with this program; if not, see http://www.gnu.org/licenses or write to
 * the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
 * Boston, MA, 02110-1301 USA, or download the license from the following URL:
 * http://itextpdf.com/terms-of-use/
 *
 * The interactive user interfaces in modified source and object code versions
 * of this program must display Appropriate Legal Notices, as required under
 * Section 5 of the GNU Affero General Public License.
 *
 * In accordance with Section 7(b) of the GNU Affero General Public License,
 * you must retain the producer line in every PDF that is created or manipulated
 * using iText.
 *
 * You can be released from the requirements of the license by purchasing
 * a commercial license. Buying such a license is mandatory as soon as you
 * develop commercial activities involving the iText software without
 * disclosing the source code of your own applications.
 * These activities include: offering paid services to customers as an ASP,
 * serving PDFs on the fly in a web application, shipping iText with a closed
 * source product.
 *
 * For more information, please contact iText Software Corp. at this
 * address: sales@itextpdf.com
 */

namespace iTextSharp.text.xml.xmp {

    /**
    * An implementation of an XmpSchema.
    */
    public class DublinCoreSchema : XmpSchema {
        
        /** default namespace identifier*/
        public const String DEFAULT_XPATH_ID = "dc";
        /** default namespace uri*/
        public const String DEFAULT_XPATH_URI = "http://purl.org/dc/elements/1.1/";
        /** External Contributors to the resource (other than the authors). */
        public const String CONTRIBUTOR = "dc:contributor";
        /** The extent or scope of the resource. */
        public const String COVERAGE = "dc:coverage";
        /** The authors of the resource (listed in order of precedence, if significant). */
        public const String CREATOR = "dc:creator";
        /** Date(s) that something interesting happened to the resource. */
        public const String DATE = "dc:date";
        /** A textual description of the content of the resource. Multiple values may be present for different languages. */
        public const String DESCRIPTION = "dc:description";
        /** The file format used when saving the resource. Tools and applications should set this property to the save format of the data. It may include appropriate qualifiers. */
        public const String FORMAT = "dc:format";
        /** Unique identifier of the resource. */
        public const String IDENTIFIER = "dc:identifier";
        /** An unordered array specifying the languages used in the resource. */
        public const String LANGUAGE = "dc:language";
        /** Publishers. */
        public const String PUBLISHER = "dc:publisher";
        /** Relationships to other documents. */
        public const String RELATION = "dc:relation";
        /** Informal rights statement, selected by language. */
        public const String RIGHTS = "dc:rights";
        /** Unique identifier of the work from which this resource was derived. */
        public const String SOURCE = "dc:source";
        /** An unordered array of descriptive phrases or keywords that specify the topic of the content of the resource. */
        public const String SUBJECT = "dc:subject";
        /** The title of the document, or the name given to the resource. Typically, it will be a name by which the resource is formally known. */
        public const String TITLE = "dc:title";
        /** A document type; for example, novel, poem, or working paper. */
        public const String TYPE = "dc:type";
        
        /**
        * @param shorthand
        * @throws IOException
        */
        public DublinCoreSchema() : base("xmlns:" + DEFAULT_XPATH_ID + "=\"" + DEFAULT_XPATH_URI + "\"") {
            this[FORMAT] = "application/pdf";
        }
        
        /**
        * Adds a title.
        * @param title
        */
        public void AddTitle(String title) {
            XmpArray array = new XmpArray(XmpArray.ALTERNATIVE);
            array.Add(title);
            SetProperty(TITLE, array);
        }

        /**
         * Adds a title.
         * @param title
         */
        public void AddTitle(LangAlt title) {
            SetProperty(TITLE, title);
        }



        /**
        * Adds a description.
        * @param desc
        */
        public void AddDescription(String desc) {
            XmpArray array = new XmpArray(XmpArray.ALTERNATIVE);
            array.Add(desc);
            SetProperty(DESCRIPTION, array);
        }

        /**
         * Adds a description.
         * @param desc
         */
        public void AddDescription(LangAlt desc) {
            SetProperty(DESCRIPTION, desc);
        }


        /**
        * Adds a subject.
        * @param subject
        */
        public void AddSubject(String subject) {
            XmpArray array = new XmpArray(XmpArray.UNORDERED);
            array.Add(subject);
            SetProperty(SUBJECT, array);
        }

        
        /**
        * Adds a subject.
        * @param subject array of subjects
        */
        public void AddSubject(String[] subject) {
            XmpArray array = new XmpArray(XmpArray.UNORDERED);
            for (int i = 0; i < subject.Length; i++) {
                array.Add(subject[i]);
            }
            SetProperty(SUBJECT, array);
        }
        
        /**
        * Adds a single author.
        * @param author
        */
        public void AddAuthor(String author) {
            XmpArray array = new XmpArray(XmpArray.ORDERED);
            array.Add(author);
            SetProperty(CREATOR, array);
        }
        
        /**
        * Adds an array of authors.
        * @param author
        */
        public void AddAuthor(String[] author) {
            XmpArray array = new XmpArray(XmpArray.ORDERED);
            for (int i = 0; i < author.Length; i++) {
                array.Add(author[i]);
            }
            SetProperty(CREATOR, array);
        }

        /**
        * Adds a single publisher.
        * @param publisher
        */
        public void AddPublisher(String publisher) {
            XmpArray array = new XmpArray(XmpArray.ORDERED);
            array.Add(publisher);
            SetProperty(PUBLISHER, array);
        }

        /**
        * Adds an array of publishers.
        * @param publisher
        */
        public void AddPublisher(String[] publisher) {
            XmpArray array = new XmpArray(XmpArray.ORDERED);
            for (int i = 0; i < publisher.Length; i++) {
                array.Add(publisher[i]);
            }
            SetProperty(PUBLISHER, array);
        }
    }
}
