using System;

namespace iTextSharp.text.api {

    /**
     * Objects implementing Indentable allow to set indentation left and right.
     */
    public interface IIndentable {

        /**
         * Sets the indentation on the left side.
         *
         * @param   indentation     the new indentation
         */
        float IndentationLeft {get;set;}

        /**
         * Sets the indentation on the right side.
         *
         * @param   indentation     the new indentation
         */
        float IndentationRight {get;set;}
    }
}