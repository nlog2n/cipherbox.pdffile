namespace iTextSharp.text.pdf.languages
{
    /**
     * Implementation of the IndicLigaturizer for Gujarati.
     */
    public class GujaratiLigaturizer : IndicLigaturizer 
    {
        // Gujrati constants
        public static char GUJR_MATRA_AA = '\u0ABE';
        public static char GUJR_MATRA_I = '\u0ABF';
        public static char GUJR_MATRA_E = '\u0AC7';
        public static char GUJR_MATRA_AI = '\u0AC8';
        public static char GUJR_MATRA_HLR = '\u0AE2';
        public static char GUJR_MATRA_HLRR = '\u0AE3';
        public static char GUJR_LETTER_A = '\u0A85';
        public static char GUJR_LETTER_AU = '\u0A94';
        public static char GUJR_LETTER_KA = '\u0A95';
        public static char GUJR_LETTER_HA = '\u0AB9';
        public static char GUJR_HALANTA = '\u0ACD';

        /**
         * Constructor for the IndicLigaturizer for Gujarati.
         */

        public GujaratiLigaturizer() {
            langTable = new char[11];
            langTable[MATRA_AA] = GUJR_MATRA_AA;
            langTable[MATRA_I] = GUJR_MATRA_I;
            langTable[MATRA_E] = GUJR_MATRA_E;
            langTable[MATRA_AI] = GUJR_MATRA_AI;
            langTable[MATRA_HLR] = GUJR_MATRA_HLR;
            langTable[MATRA_HLRR] = GUJR_MATRA_HLRR;
            langTable[LETTER_A] = GUJR_LETTER_A;
            langTable[LETTER_AU] = GUJR_LETTER_AU;
            langTable[LETTER_KA] = GUJR_LETTER_KA;
            langTable[LETTER_HA] = GUJR_LETTER_HA;
            langTable[HALANTA] = GUJR_HALANTA;
        }
    }
}