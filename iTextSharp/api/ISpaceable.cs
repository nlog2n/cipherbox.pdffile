using System;

namespace iTextSharp.text.api {

    /**
     * Objects implementing Spaceable allow setting spacing before and after.
     */
    public interface ISpaceable {

        /**
         * Sets the spacing before.
         *
         * @param   spacing     the new spacing
         */
        float SpacingBefore {get;set;}

        /**
         * Sets the spacing after.
         *
         * @param   spacing     the new spacing
         */
        float SpacingAfter {get;set;}
    }
}