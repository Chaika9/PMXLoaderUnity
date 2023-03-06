using System;

namespace LibMMD.Reader {
    public class ModelParseException : Exception {
        public ModelParseException(string message) : base(message) {}
    }
}
