
namespace CipherBox.Pdf
{
    public class PdfWaterMarkOption
    {
        public float  Rotation = 0f; // in degree
        public float  Opacity = 0.5f; // 0-1
        public int    PageStart = 1;    // page number
        public int    PageEnd = int.MaxValue; // page number
        public string Location = "Center"; // only support center. Top Center, Top Lef, Top Right, Bottom Center, Bottom Left, Bottom Right

        public string Text = null;
        public string TextFont = "HELVETICA";
        public float  TextFontSize = 30;
        public string TextColor = "RED";

        public string ImageFileName = null;
        public float  ImageScalePercentage = 100;  // range 0-100

        public PdfWaterMarkOption()
        {
        }

        public PdfWaterMarkOption(string wmtext, string wmpicfilename)
        {
            this.Text = wmtext;
            this.ImageFileName = wmpicfilename;
        }
    }
}