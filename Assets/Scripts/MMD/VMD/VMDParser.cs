using System.IO;
using System.Text;
using UnityEngine;

namespace MMD.VMD {
    public static class VMDParser {
        public static VMDFormat Load(string path) {
            if (!File.Exists(path)) {
                throw new FileNotFoundException("The animation file is not found", path);
            }
            using (FileStream stream = File.OpenRead(path)) {
                return Parse(stream);
            }
        }
        
        private static VMDFormat Parse(Stream stream) {
            var reader = new BinaryReader(stream);
            var format = new VMDFormat();
            try {
                ParseHeader(reader, format);
                ParseMotion(reader, format);
            } finally {
                reader.Close();
                stream.Close();
            }
            return format;
        }
        
        private static void ParseHeader(BinaryReader reader, VMDFormat format) {
            string animationType = ReadString(reader, 30);
            if (animationType != "Vocaloid Motion Data 0002") {
                throw new InvalidDataException("The animation file is not a valid VMD file");
            }
            
            format.Header = new VMDFormat.VMDHeader();
            
            // Name of the model that this VMD is compatible with.
            format.Header.ModelReferenceName = ReadString(reader, 20);
        }
        
        private static void ParseMotion(BinaryReader reader, VMDFormat format) {
            // Bone frames
            format.BoneFrames = ReadBoneFrames(reader);
            // Morph frames
            format.MorphFrames = ReadMorphFrames(reader);
            // Camera frames
            format.CameraFrames = ReadCameraFrames(reader);
        }
        
        private static BoneFrame[] ReadBoneFrames(BinaryReader reader) {
            uint nbBoneFrames = reader.ReadUInt32();
            var frames = new BoneFrame[nbBoneFrames];
            for (int i = 0; i < nbBoneFrames; i++) {
                frames[i] = ReadBoneFrame(reader);
            }
            return frames;
        }
        
        private static BoneFrame ReadBoneFrame(BinaryReader reader) {
            var frame = new BoneFrame {
                // Bone name (limited to 15 characters)
                BoneName = ReadString(reader, 15),
                // Frame number (1 frame = 1/30 second)
                FrameNumber = reader.ReadUInt32(),
                // Position
                Position = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle()),
                // Rotation
                Rotation = new Quaternion(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle()),
                
                // Read curves
                XCurve = ReadCurve(reader),
                YCurve = ReadCurve(reader),
                ZCurve = ReadCurve(reader),
                RCurve = ReadCurve(reader)
            };
            return frame;
        }
        
        private static Curve ReadCurve(BinaryReader reader) {
            var curve = new Curve {
                AX = ReadBone(reader),
                AY = ReadBone(reader),
                BX = ReadBone(reader),
                BY = ReadBone(reader)
            };
            return curve;
        }
        
        private static MorphFrame[] ReadMorphFrames(BinaryReader reader) {
            uint nbMorphFrames = reader.ReadUInt32();
            var frames = new MorphFrame[nbMorphFrames];
            for (int i = 0; i < nbMorphFrames; i++) {
                frames[i] = ReadMorphFrame(reader);
            }
            return frames;
        }
        
        private static MorphFrame ReadMorphFrame(BinaryReader reader) {
            var frame = new MorphFrame {
                // Morph name (limited to 15 characters)
                MorphName = ReadString(reader, 15),
                // Frame number (1 frame = 1/30 second)
                FrameNumber = reader.ReadUInt32(),
                // Weight
                Weight = reader.ReadSingle()
            };
            return frame;
        }
        
        private static CameraFrame[] ReadCameraFrames(BinaryReader reader) {
            uint nbCameraFrames = reader.ReadUInt32();
            var frames = new CameraFrame[nbCameraFrames];
            for (int i = 0; i < nbCameraFrames; i++) {
                frames[i] = ReadCameraFrame(reader);
            }
            return frames;
        }
        
        private static CameraFrame ReadCameraFrame(BinaryReader reader) {
            var frame = new CameraFrame {
                // Frame number (1 frame = 1/30 second)
                FrameNumber = reader.ReadUInt32(),
                // Distance
                Distance = reader.ReadSingle(),
                // Position
                Position = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle()),
                // Rotation
                Rotation = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle()),
                // Curve
                Curve = new Curve {
                    AX = reader.ReadByte(),
                    AY = reader.ReadByte(),
                    BX = reader.ReadByte(),
                    BY = reader.ReadByte()
                }
            };

            // Skip 20 bytes
            reader.ReadBytes(20);
            
            frame.FOV = reader.ReadUInt32();
            frame.Orthographic = reader.ReadBoolean();
            return frame;
        }

        private static byte ReadBone(BinaryReader reader) {
            byte bone = reader.ReadByte();
            // Skip 3 bytes
            reader.ReadBytes(3);
            return bone;
        }
        
        private static string ReadString(BinaryReader reader, int count) {
            byte[] bytes = reader.ReadBytes(count);
            
            // Shift-JIS is a Japanese encoding. It is used by MMD.
            return Encoding.GetEncoding("shift_jis").GetString(bytes).Trim('\0');
        }
    }
}
