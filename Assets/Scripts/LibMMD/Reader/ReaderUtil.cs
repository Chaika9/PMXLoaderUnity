using System.IO;
using System.Text;

namespace LibMMD.Reader {
    public static class ReaderUtil {
        public static string ReadString(BinaryReader reader, Encoding encoding, int length) {
            if (length < 0) {
                throw new IOException("Invalid string length: " + length);
            }
            
            // Empty string
            if (length == 0) {
                return "";
            }
            
            byte[] bytes = reader.ReadBytes(length);
            string str = encoding.GetString(bytes);
            return str;
        }
        
        public static string ReadString(BinaryReader reader, Encoding encoding) {
            int length = reader.ReadInt32();
            return ReadString(reader, encoding, length);
        }
    }
}