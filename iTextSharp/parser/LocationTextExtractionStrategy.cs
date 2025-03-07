using System;
using System.Collections.Generic;
using System.Text;

namespace CipherBox.Pdf.Parser {


    /**
     * <b>Development preview</b> - this class (and all of the parser classes) are still experiencing
     * heavy development, and are subject to change both behavior and interface.
     * <br>
     * A text extraction renderer that keeps track of relative position of text on page
     * The resultant text will be relatively consistent with the physical layout that most
     * PDF files have on screen.
     * <br>
     * This renderer keeps track of the orientation and distance (both perpendicular
     * and parallel) to the unit vector of the orientation.  Text is ordered by
     * orientation, then perpendicular, then parallel distance.  Text with the same
     * perpendicular distance, but different parallel distance is treated as being on
     * the same line.
     * <br>
     * This renderer also uses a simple strategy based on the font metrics to determine if
     * a blank space should be inserted into the output.
     *
     * @since   5.0.2
     */
    public class LocationTextExtractionStrategy : ITextExtractionStrategy {

        /** set to true for debugging */
        public static bool DUMP_STATE = false;
        
        /** a summary of all found text */
        private List<TextChunk> locationalResult = new List<TextChunk>();

        /**
         * Creates a new text extraction renderer.
         */
        public LocationTextExtractionStrategy() {
        }

        /**
         * @see com.itextpdf.text.pdf.parser.RenderListener#beginTextBlock()
         */
        public virtual void BeginTextBlock(){
        }

        /**
         * @see com.itextpdf.text.pdf.parser.RenderListener#endTextBlock()
         */
        public virtual void EndTextBlock(){
        }

        /**
         * @param str
         * @return true if the string starts with a space character, false if the string is empty or starts with a non-space character
         */
        private bool StartsWithSpace(String str){
            if (str.Length == 0) return false;
            return str[0] == ' ';
        }
        
        /**
         * @param str
         * @return true if the string ends with a space character, false if the string is empty or ends with a non-space character
         */
        private bool EndsWithSpace(String str){
            if (str.Length == 0) return false;
            return str[str.Length-1] == ' ';
        }

        /**
         * Determines if a space character should be inserted between a previous chunk and the current chunk.
         * This method is exposed as a callback so subclasses can fine time the algorithm for determining whether a space should be inserted or not.
         * By default, this method will insert a space if the there is a gap of more than half the font space character width between the end of the
         * previous chunk and the beginning of the current chunk.  It will also indicate that a space is needed if the starting point of the new chunk 
         * appears *before* the end of the previous chunk (i.e. overlapping text).
         * @param chunk the new chunk being evaluated
         * @param previousChunk the chunk that appeared immediately before the current chunk
         * @return true if the two chunks represent different words (i.e. should have a space between them).  False otherwise.
         */
        protected bool IsChunkAtWordBoundary(TextChunk chunk, TextChunk previousChunk) {
            float dist = chunk.DistanceFromEndOf(previousChunk);
            if(dist < -chunk.CharSpaceWidth || dist > chunk.CharSpaceWidth / 2.0f)
                return true;

            return false;
        }

        /**
         * Returns the result so far.
         * @return  a String with the resulting text.
         */
        public virtual String GetResultantText(){

            if (DUMP_STATE) DumpState();
            
            locationalResult.Sort();

            StringBuilder sb = new StringBuilder();
            TextChunk lastChunk = null;
            foreach (TextChunk chunk in locationalResult) {

                if (lastChunk == null){
                    sb.Append(chunk.text);
                } else {
                    if (chunk.SameLine(lastChunk)){
                        // we only insert a blank space if the trailing character of the previous string wasn't a space, and the leading character of the current string isn't a space
                        if(IsChunkAtWordBoundary(chunk, lastChunk) && !StartsWithSpace(chunk.text) && !EndsWithSpace(lastChunk.text))
                            sb.Append(' ');

                        sb.Append(chunk.text);
                    } else {
                        sb.Append('\n');
                        sb.Append(chunk.text);
                    }
                }
                lastChunk = chunk;
            }

            return sb.ToString();

        }

        /** Used for debugging only */
        private void DumpState(){
            foreach (TextChunk location in locationalResult) {
                
                location.PrintDiagnostics();
                
                Console.Out.WriteLine();
            }
            
        }
        
        /**
         * 
         * @see com.itextpdf.text.pdf.parser.RenderListener#renderText(com.itextpdf.text.pdf.parser.TextRenderInfo)
         */
        public virtual void RenderText(TextRenderInfo renderInfo) {
            LineSegment segment = renderInfo.GetBaseline();
            if (renderInfo.GetRise() != 0)
            { // remove the rise from the baseline - we do this because the text from a super/subscript render operations should probably be considered as part of the baseline of the text the super/sub is relative to 
                Matrix riseOffsetTransform = new Matrix(0, -renderInfo.GetRise());
                segment = segment.TransformBy(riseOffsetTransform);
            }
            TextChunk location = new TextChunk(renderInfo.GetText(), segment.GetStartPoint(), segment.GetEndPoint(), renderInfo.GetSingleSpaceWidth());
            locationalResult.Add(location);        
        }
        


        /**
         * Represents a chunk of text, it's orientation, and location relative to the orientation vector
         */
        protected class TextChunk : IComparable<TextChunk>{
            /** the text of the chunk */
            internal String text;
            /** the starting location of the chunk */
            internal Vector startLocation;
            /** the ending location of the chunk */
            internal Vector endLocation;
            /** unit vector in the orientation of the chunk */
            internal Vector orientationVector;
            /** the orientation as a scalar for quick sorting */
            internal int orientationMagnitude;
            /** perpendicular distance to the orientation unit vector (i.e. the Y position in an unrotated coordinate system)
             * we round to the nearest integer to handle the fuzziness of comparing floats */
            internal int distPerpendicular;
            /** distance of the start of the chunk parallel to the orientation unit vector (i.e. the X position in an unrotated coordinate system) */
            internal float distParallelStart;
            /** distance of the end of the chunk parallel to the orientation unit vector (i.e. the X position in an unrotated coordinate system) */
            internal float distParallelEnd;
            /** the width of a single space character in the font of the chunk */
            internal float charSpaceWidth;
            
            public TextChunk(String str, Vector startLocation, Vector endLocation, float charSpaceWidth) {
                this.text = str;
                this.startLocation = startLocation;
                this.endLocation = endLocation;
                this.charSpaceWidth = charSpaceWidth;
                
                Vector oVector = endLocation.Subtract(startLocation);
                if (oVector.Length == 0) {
                    oVector = new Vector(1, 0, 0);
                }
                orientationVector = oVector.Normalize();
                orientationMagnitude = (int)(Math.Atan2(orientationVector[Vector.I2], orientationVector[Vector.I1])*1000);

                // see http://mathworld.wolfram.com/Point-LineDistance2-Dimensional.html
                // the two vectors we are crossing are in the same plane, so the result will be purely
                // in the z-axis (out of plane) direction, so we just take the I3 component of the result
                Vector origin = new Vector(0,0,1);
                distPerpendicular = (int)(startLocation.Subtract(origin)).Cross(orientationVector)[Vector.I3];

                distParallelStart = orientationVector.Dot(startLocation);
                distParallelEnd = orientationVector.Dot(endLocation);
            }


            /**
             * @return the text captured by this chunk
             */
            public String Text {
                get { return text; }
            }

            /**
             * @return the width of a single space character as rendered by this chunk
             */
            public float CharSpaceWidth {
                get { return charSpaceWidth; }
            }


            public void PrintDiagnostics(){
                Console.Out.WriteLine("Text (@" + startLocation + " -> " + endLocation + "): " + text);
                Console.Out.WriteLine("orientationMagnitude: " + orientationMagnitude);
                Console.Out.WriteLine("distPerpendicular: " + distPerpendicular);
                Console.Out.WriteLine("distParallel: " + distParallelStart);
            }
            
            /**
             * @param as the location to compare to
             * @return true is this location is on the the same line as the other
             */
            public bool SameLine(TextChunk a){
                if (orientationMagnitude != a.orientationMagnitude) return false;
                if (distPerpendicular != a.distPerpendicular) return false;
                return true;
            }

            /**
             * Computes the distance between the end of 'other' and the beginning of this chunk
             * in the direction of this chunk's orientation vector.  Note that it's a bad idea
             * to call this for chunks that aren't on the same line and orientation, but we don't
             * explicitly check for that condition for performance reasons.
             * @param other
             * @return the number of spaces between the end of 'other' and the beginning of this chunk
             */
            public float DistanceFromEndOf(TextChunk other){
                float distance = distParallelStart - other.distParallelEnd;
                return distance;
            }
            
            /**
             * Compares based on orientation, perpendicular distance, then parallel distance
             * @see java.lang.Comparable#compareTo(java.lang.Object)
             */
            public int CompareTo(TextChunk rhs) {
                if (this == rhs) return 0; // not really needed, but just in case
                
                int rslt;
                rslt = CompareInts(orientationMagnitude, rhs.orientationMagnitude);
                if (rslt != 0) return rslt;

                rslt = CompareInts(distPerpendicular, rhs.distPerpendicular);
                if (rslt != 0) return rslt;

                // note: it's never safe to check floating point numbers for equality, and if two chunks
                // are truly right on top of each other, which one comes first or second just doesn't matter
                // so we arbitrarily choose this way.
                rslt = distParallelStart < rhs.distParallelStart ? -1 : 1;

                return rslt;
            }

            /**
             *
             * @param int1
             * @param int2
             * @return comparison of the two integers
             */
            private static int CompareInts(int int1, int int2){
                return int1 == int2 ? 0 : int1 < int2 ? -1 : 1;
            }

            
        }

        /**
         * no-op method - this renderer isn't interested in image events
         * @see com.itextpdf.text.pdf.parser.RenderListener#renderImage(com.itextpdf.text.pdf.parser.ImageRenderInfo)
         * @since 5.0.1
         */
        public virtual void RenderImage(ImageRenderInfo renderInfo) {
            // do nothing
        }
    }
}