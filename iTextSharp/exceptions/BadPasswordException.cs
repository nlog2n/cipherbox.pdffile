using System;
using System.IO;

namespace iTextSharp.text.pdf {

    public class BadPasswordException : IOException {
        public BadPasswordException(string message) : base(message) {
        }
    }
}
