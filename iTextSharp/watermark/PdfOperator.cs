using System;
using System.Collections.Generic;
using System.IO;

using iTextSharp.text.pdf;


namespace CipherBox.Pdf
{
    /// <summary>
    /// PDF Operator interface. A series of content operators for removing watermark only
    /// More complicated operations pls refer to IContentOperator.cs and PdfContentStreamProcessor.cs
    /// </summary>
    public class PdfOperator
    {
        /// <summary>
        /// Handlers that processes an operator </summary>
        /// <param name="parser">	the parser </param>
        /// <param name="operator">	the operator </param>
        /// <param name="operands">	its operands </param>
        /// <exception cref="IOException"> </exception>
        public delegate void PdfOperatorHandler(OCGParser parser, PdfLiteral @operator, IList<PdfObject> operands);

        protected const string DEFAULTOPERATOR = "DefaultOperator"; // Constant used for the default operator
        protected static IDictionary<string, PdfOperatorHandler> _operators; // A map with all supported operators operators (PDF syntax)

        static PdfOperator()
        {
            PopulateOperators();
        }

        /// <summary>
        /// Class that processes unknown content.
        /// </summary>
        public static void ProcessUnknownOperator(OCGParser parser, PdfLiteral @operator, IList<PdfObject> operands)
        {
            // copy content simply
            parser.Process(@operator, operands, true);
        }

        /// <summary>
        /// Class that knows how to process graphics state operators.
        /// </summary>
        public static void ProcessGraphicsOperator(OCGParser parser, PdfLiteral @operator, IList<PdfObject> operands)
        {
            parser.Process(@operator, operands, false);
        }

        /// <summary>
        /// Class that knows how to process inline image operators.
        /// </summary>
        public static void ProcessInlineImageOperator(OCGParser parser, PdfLiteral @operator, IList<PdfObject> operands)
        {
            Console.WriteLine("found inline image operator " + @operator.ToString());
            // On how to add image to page as inline image object.
            // Basically you need to write directly on writer.GetDirectContent.AddImage()
            // http://itextpdf.com/examples/iia.php?id=72
/*
q 
9 0 0 9 2997 4118.67 cm
BI
  /CS/RGB
  /W 1
  /H 1
  /BPC 8
ID  [image raw data]
EI 
Q
*/
            parser.Process(@operator, operands, true);
        }

        /// <summary>
        /// Class that knows how to process marked content operators.
        /// </summary>
        public static void ProcessMarkedContentOperator(OCGParser parser, PdfLiteral op, IList<PdfObject> operands)
        {
            parser.CheckMarkedContentStart(op, operands);

            parser.Process(op, operands, true);

            parser.CheckMarkedContentEnd(op);
        }

        /// <summary>
        /// Class that knows how to process path construction, path painting and path clipping operators.
        /// </summary>
        public static void ProcessPathConstructionOrPaintingOperator(OCGParser parser, PdfLiteral @operator, IList<PdfObject> operands)
        {
            parser.Process(@operator, operands, true);
        }

        /// <summary>
        /// Class that knows how to process text state operators.
        /// </summary>
        public static void ProcessTextOperator(OCGParser parser, PdfLiteral @operator, IList<PdfObject> operands)
        {
            parser.Process(@operator, operands, true);
        }

        /// <summary>
        /// Class that knows how to process XObject operators.
        /// </summary>
        public static void ProcessXObjectOperator(OCGParser parser, PdfLiteral @operator, IList<PdfObject> operands)
        {
            if (!parser.IsVisible(operands)) // already hidden by from xobjects to remove
            {
                // do not write to stream
            }
            else
            {
                // further identify if it is hidden by its parent, or filtered by user
                parser.Process(@operator, operands, true);
            }
        }


        private PdfOperatorHandler _handler;
        private PdfLiteral _operator;
        private IList<PdfObject> _operands;

        public PdfOperator(PdfLiteral op, IList<PdfObject> operands)
        {
            _operator = op;
            _operands = operands;

            if ( !_operators.TryGetValue(_operator.ToString(), out _handler) )
            {
                _handler = _operators[DEFAULTOPERATOR];
            }
        }

        public void Process(OCGParser parser)
        {
            _handler(parser, _operator, _operands);
        }

        /// <summary>
        /// Populates the operators variable.
        /// </summary>
        private static void PopulateOperators()
        {
            if (_operators != null) return; // one-time initialization

            _operators = new Dictionary<string, PdfOperatorHandler>();
            _operators[DEFAULTOPERATOR] = ProcessUnknownOperator;  // copy content simply

            // PathConstructionOrPaintingOperator
            _operators["m"] = ProcessPathConstructionOrPaintingOperator;
            _operators["l"] = ProcessPathConstructionOrPaintingOperator;
            _operators["c"] = ProcessPathConstructionOrPaintingOperator;
            _operators["v"] = ProcessPathConstructionOrPaintingOperator;
            _operators["y"] = ProcessPathConstructionOrPaintingOperator;
            _operators["h"] = ProcessPathConstructionOrPaintingOperator;
            _operators["re"] = ProcessPathConstructionOrPaintingOperator;
            _operators["S"] = ProcessPathConstructionOrPaintingOperator;
            _operators["s"] = ProcessPathConstructionOrPaintingOperator;
            _operators["f"] = ProcessPathConstructionOrPaintingOperator;
            _operators["F"] = ProcessPathConstructionOrPaintingOperator;
            _operators["f*"] = ProcessPathConstructionOrPaintingOperator;
            _operators["B"] = ProcessPathConstructionOrPaintingOperator;
            _operators["B*"] = ProcessPathConstructionOrPaintingOperator;
            _operators["b"] = ProcessPathConstructionOrPaintingOperator;
            _operators["b*"] = ProcessPathConstructionOrPaintingOperator;
            _operators["n"] = ProcessPathConstructionOrPaintingOperator;
            _operators["W"] = ProcessPathConstructionOrPaintingOperator;
            _operators["W*"] = ProcessPathConstructionOrPaintingOperator;

            // GraphicsOperator
            _operators["q"] = ProcessGraphicsOperator;
            _operators["Q"] = ProcessGraphicsOperator;
            _operators["w"] = ProcessGraphicsOperator;
            _operators["J"] = ProcessGraphicsOperator;
            _operators["j"] = ProcessGraphicsOperator;
            _operators["M"] = ProcessGraphicsOperator;
            _operators["d"] = ProcessGraphicsOperator;
            _operators["ri"] = ProcessGraphicsOperator;
            _operators["i"] = ProcessGraphicsOperator;
            _operators["gs"] = ProcessGraphicsOperator;
            _operators["cm"] = ProcessGraphicsOperator;
            _operators["g"] = ProcessGraphicsOperator;
            _operators["G"] = ProcessGraphicsOperator;
            _operators["rg"] = ProcessGraphicsOperator;
            _operators["RG"] = ProcessGraphicsOperator;
            _operators["k"] = ProcessGraphicsOperator;
            _operators["K"] = ProcessGraphicsOperator;
            _operators["cs"] = ProcessGraphicsOperator;
            _operators["CS"] = ProcessGraphicsOperator;
            _operators["sc"] = ProcessGraphicsOperator;
            _operators["SC"] = ProcessGraphicsOperator;
            _operators["scn"] = ProcessGraphicsOperator;
            _operators["SCN"] = ProcessGraphicsOperator;
            _operators["sh"] = ProcessGraphicsOperator;

            // XObjectOperator 
            _operators["Do"] = ProcessXObjectOperator;

            // InlineImageOperator
            _operators["BI"] = ProcessInlineImageOperator;
            _operators["EI"] = ProcessInlineImageOperator;

            // TextOperator
            _operators["BT"] = ProcessTextOperator;
            _operators["ID"] = ProcessTextOperator;
            _operators["ET"] = ProcessTextOperator;
            _operators["Tc"] = ProcessTextOperator;
            _operators["Tw"] = ProcessTextOperator;
            _operators["Tz"] = ProcessTextOperator;
            _operators["TL"] = ProcessTextOperator;
            _operators["Tf"] = ProcessTextOperator;
            _operators["Tr"] = ProcessTextOperator;
            _operators["Ts"] = ProcessTextOperator;
            _operators["Td"] = ProcessTextOperator;
            _operators["TD"] = ProcessTextOperator;
            _operators["Tm"] = ProcessTextOperator;
            _operators["T*"] = ProcessTextOperator;
            _operators["Tj"] = ProcessTextOperator;
            _operators["'"] = ProcessTextOperator;
            _operators["\""] = ProcessTextOperator;
            _operators["TJ"] = ProcessTextOperator;

            // MarkedContentOperator
            _operators["BMC"] = ProcessMarkedContentOperator;
            _operators["BDC"] = ProcessMarkedContentOperator;
            _operators["EMC"] = ProcessMarkedContentOperator;
        }
    }
}