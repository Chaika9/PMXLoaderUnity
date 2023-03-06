using System;

namespace LibMMD.Reader {
    public class ModelFormatException : Exception {
        public ModelFormatException(string message) : base(message) { }
    }
}