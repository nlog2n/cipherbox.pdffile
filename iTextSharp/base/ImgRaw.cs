using System;
using iTextSharp.text.error_messages;

/*
 * $Id: ImgRaw.cs 318 2012-02-27 22:46:07Z psoares33 $
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

namespace iTextSharp.text {
    /// <summary>
    /// Raw Image data that has to be inserted into the document
    /// </summary>
    /// <seealso cref="T:iTextSharp.text.Element"/>
    /// <seealso cref="T:iTextSharp.text.Image"/>
    public class ImgRaw : Image {

        public ImgRaw(Image image) : base(image) {}

        /// <summary>
        /// Creats an Image in raw mode.
        /// </summary>
        /// <param name="width">the exact width of the image</param>
        /// <param name="height">the exact height of the image</param>
        /// <param name="components">1,3 or 4 for GrayScale, RGB and CMYK</param>
        /// <param name="bpc">bits per component. Must be 1,2,4 or 8</param>
        /// <param name="data">data the image data</param>
        public ImgRaw(int width, int height, int components, int bpc, byte[] data) : base((Uri)null) 
        {
            type = Element.IMGRAW;
            scaledHeight = height;
            this.Top = scaledHeight;
            scaledWidth = width;
            this.Right = scaledWidth;
            if (components != 1 && components != 3 && components != 4)
                throw new DocumentException("BadElement: components.must.be.1.3.or.4");
            if (bpc != 1 && bpc != 2 && bpc != 4 && bpc != 8)
                throw new DocumentException("BadElement: bits.per.component.must.be.1.2.4.or.8");
            colorspace = components;
            this.bpc = bpc;
            rawData = data;
            plainWidth = this.Width;
            plainHeight = this.Height;
        }
    }
}
