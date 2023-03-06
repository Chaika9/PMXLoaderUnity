using System.IO;
using System.Text;
using UnityEngine;

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
        
        public static Vector4 ReadVector4(BinaryReader reader) {
            float x = reader.ReadSingle();
            float y = reader.ReadSingle();
            float z = reader.ReadSingle();
            float w = reader.ReadSingle();
            return new Vector4(x, y, z, w);
        }
        
        public static Vector3 ReadVector3(BinaryReader reader) {
            float x = reader.ReadSingle();
            float y = reader.ReadSingle();
            float z = reader.ReadSingle();
            return new Vector3(x, y, z);
        }
        
        public static Vector2 ReadVector2(BinaryReader reader) {
            float x = reader.ReadSingle();
            float y = reader.ReadSingle();
            return new Vector2(x, y);
        }

        public static int ReadIndex(BinaryReader reader, int configBoneIndexSize) {
            return configBoneIndexSize switch {
                1 => reader.ReadByte(),
                2 => reader.ReadUInt16(),
                4 => reader.ReadInt32(),
                _ => throw new IOException("Invalid index size: " + configBoneIndexSize)
            };
        }
        public static Color ReadColor(BinaryReader reader, bool hasAlpha = false) {
            float r = reader.ReadSingle();
            float g = reader.ReadSingle();
            float b = reader.ReadSingle();
            float a = hasAlpha ? reader.ReadSingle() : 1.0f;
            return new Color(r, g, b, a);
        }
    }
}