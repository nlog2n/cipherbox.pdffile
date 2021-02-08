using System;
using System.Collections;
using System.Text;

using CipherBox.Pdf.Utility;
using iTextSharp.text.pdf;
using iTextSharp.text.error_messages;

namespace iTextSharp.text {
    /// <summary>
    /// A RectangleReadOnly is the representation of a geometric figure.
    /// It's the same as a Rectangle but immutable.
    /// </summary>
    /// <seealso cref="T:iTextSharp.text.Element"/>
    /// <seealso cref="T:iTextSharp.text.Table"/>
    /// <seealso cref="T:iTextSharp.text.Cell"/>
    /// <seealso cref="T:iTextSharp.text.HeaderFooter"/>
    public class RectangleReadOnly : Rectangle {
    
        // constructors
    
        /// <summary>
        /// Constructs a RectangleReadOnly-object.
        /// </summary>
        /// <param name="llx">lower left x</param>
        /// <param name="lly">lower left y</param>
        /// <param name="urx">upper right x</param>
        /// <param name="ury">upper right y</param>
        public RectangleReadOnly(float llx, float lly, float urx, float ury) : base(llx, lly, urx, ury) {
        }
    
	    /**
	     * Constructs a <CODE>RectangleReadOnly</CODE> -object.
	     * 
	     * @param llx	lower left x
	     * @param lly	lower left y
	     * @param urx	upper right x
	     * @param ury	upper right y
	     * @param rotation	the rotation of the Rectangle (0, 90, 180, 270)
	     * @since iText 5.0.6
	     */
	    public RectangleReadOnly(float llx, float lly, float urx, float ury, int rotation) : base(llx, lly, urx, ury) {
            base.Rotation = rotation;
	    }

        /// <summary>
        /// Constructs a RectangleReadOnly-object starting from the origin (0, 0).
        /// </summary>
        /// <param name="urx">upper right x</param>
        /// <param name="ury">upper right y</param>
        public RectangleReadOnly(float urx, float ury) : base(0, 0, urx, ury) {}

        /**
         * Constructs a <CODE>RectangleReadOnly</CODE>-object starting from the origin
         * (0, 0) and with a specific rotation (valid values are 0, 90, 180, 270).
         * 
         * @param urx   upper right x
         * @param ury   upper right y
         * @since iText 5.0.6
         */
        public RectangleReadOnly(float urx, float ury, int rotation) : base(0, 0, urx, ury) {
            base.Rotation = rotation;
        }
    
        /// <summary>
        /// Constructs a RectangleReadOnly-object.
        /// </summary>
        /// <param name="rect">another Rectangle</param>
        public RectangleReadOnly(Rectangle rect) : base(rect.Left, rect.Bottom, rect.Right, rect.Top) {
            base.CloneNonPositionParameters(rect);
        }

        /**
        * Copies all of the parameters from a <CODE>Rectangle</CODE> object
        * except the position.
        * 
        * @param rect
        *            <CODE>Rectangle</CODE> to copy from
        */
        public override void CloneNonPositionParameters(Rectangle rect) {
            ThrowReadOnlyError();
        }

        private void ThrowReadOnlyError() {
            throw new InvalidOperationException(MessageLocalization.GetComposedMessage("rectanglereadonly.this.rectangle.is.read.only"));
        }

        /**
        * Copies all of the parameters from a <CODE>Rectangle</CODE> object
        * except the position.
        * 
        * @param rect
        *            <CODE>Rectangle</CODE> to copy from
        */

        public override void SoftCloneNonPositionParameters(Rectangle rect) {
            ThrowReadOnlyError();
        }

        // methods
    
        /**
        * Switches lowerleft with upperright
        */
        public override void Normalize() {
            ThrowReadOnlyError();
        }

        // methods to set the membervariables
    
        /// <summary>
        /// Get/set the upper right y-coordinate. 
        /// </summary>
        /// <value>a float</value>
        public override float Top {
            set {
                ThrowReadOnlyError();
            }
        }

        /**
        * Enables the border on the specified side.
        * 
        * @param side
        *            the side to enable. One of <CODE>LEFT, RIGHT, TOP, BOTTOM
        *            </CODE>
        */
        public override void EnableBorderSide(int side) {
            ThrowReadOnlyError();
        }

        /**
        * Disables the border on the specified side.
        * 
        * @param side
        *            the side to disable. One of <CODE>LEFT, RIGHT, TOP, BOTTOM
        *            </CODE>
        */
        public override void DisableBorderSide(int side) {
            ThrowReadOnlyError();
        }


        /// <summary>
        /// Get/set the border
        /// </summary>
        /// <value>a int</value>
        public override int Border {
            set {
                ThrowReadOnlyError();
            }
        }
    
        /// <summary>
        /// Get/set the grayscale of the rectangle.
        /// </summary>
        /// <value>a float</value>
        public override float GrayFill {
            set {
                ThrowReadOnlyError();
            }
        }
    
        // methods to get the membervariables
    
        /// <summary>
        /// Get/set the lower left x-coordinate.
        /// </summary>
        /// <value>a float</value>
        public override float Left {
            set {
                ThrowReadOnlyError();
            }
        }
    
        /// <summary>
        /// Get/set the upper right x-coordinate.
        /// </summary>
        /// <value>a float</value>
        public override float Right {
            set {
                ThrowReadOnlyError();
            }
        }
    
        /// <summary>
        /// Get/set the lower left y-coordinate.
        /// </summary>
        /// <value>a float</value>
        public override float Bottom {
            set {
                ThrowReadOnlyError();
            }
        }
    
        public override BaseColor BorderColorBottom {
            set {
                ThrowReadOnlyError();
            }
        }
    
        public override BaseColor BorderColorTop {
            set {
                ThrowReadOnlyError();
            }
        }
    
        public override BaseColor BorderColorLeft {
            set {
                ThrowReadOnlyError();
            }
        }
    
        public override BaseColor BorderColorRight {
            set {
                ThrowReadOnlyError();
            }
        }
    
        /// <summary>
        /// Get/set the borderwidth.
        /// </summary>
        /// <value>a float</value>
        public override float BorderWidth {
            set {
                ThrowReadOnlyError();
            }
        }
    
        /**
         * Gets the color of the border.
         *
         * @return    a value
         */
        /// <summary>
        /// Get/set the color of the border.
        /// </summary>
        /// <value>a BaseColor</value>
        public override BaseColor BorderColor {
            set {
                ThrowReadOnlyError();
            }
        }
    
        /**
         * Gets the backgroundcolor.
         *
         * @return    a value
         */
        /// <summary>
        /// Get/set the backgroundcolor.
        /// </summary>
        /// <value>a BaseColor</value>
        public override BaseColor BackgroundColor {
            set {
                ThrowReadOnlyError();
            }
        }

        /// <summary>
        /// Set/gets the rotation
        /// </summary>
        /// <value>a int</value>    
        public override int Rotation {
            set {
                ThrowReadOnlyError();
            }
        }

        public override float BorderWidthLeft {
            set {
                ThrowReadOnlyError();
            }
        }

        public override float BorderWidthRight {
            set {
                ThrowReadOnlyError();
            }
        }

        public override float BorderWidthTop {
            set {
                ThrowReadOnlyError();
            }
        }

        public override float BorderWidthBottom {
            set {
                ThrowReadOnlyError();
            }
        }

        /**
        * Sets a parameter indicating if the rectangle has variable borders
        * 
        * @param useVariableBorders
        *            indication if the rectangle has variable borders
        */
        public override bool UseVariableBorders{
            set {
                ThrowReadOnlyError();
            }
        }

	    public override String ToString() {
		    StringBuilder buf = new StringBuilder("RectangleReadOnly: ");
		    buf.Append(Width);
		    buf.Append('x');
		    buf.Append(Height);
		    buf.Append(" (rot: ");
		    buf.Append(rotation);
		    buf.Append(" degrees)");
		    return buf.ToString();
	    }
    }
}
