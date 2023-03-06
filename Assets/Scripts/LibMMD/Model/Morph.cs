

using UnityEngine;

namespace LibMMD.Model {
    public class Morph {
        public string Name { get; set; }
        public string NameEnglish { get; set; }
        public MorphCategory Category { get; set; }
        public MorphType Type { get; set; }
        public MorphData[] MorphDatas { get; set; }

        public enum MorphCategory : byte {
            Face = 0,
            LeftEye = 1,
            RightEye = 2,
            Mouth = 3,
            Other = 4
        }
        
        public enum MorphType : byte {
            Group = 0,
            Vertex = 1,
            Bone = 2,
            UV = 3,
            AdditionalUV1 = 4,
            AdditionalUV2 = 5,
            AdditionalUV3 = 6,
            AdditionalUV4 = 7,
            Material = 8,
            Flip = 9,
            Impulse = 10
        }
        
        public abstract class MorphData {}
        
        public class GroupMorph : MorphData {
            public int MorphIndex { get; set; }
            public float MorphRate { get; set; }
        }
        
        public class VertexMorph : MorphData {
            public int VertexIndex { get; set; }
            public Vector3 Offset { get; set; }
        }
        
        public class BoneMorph : MorphData {
            public int BoneIndex { get; set; }
            public Vector3 Translation { get; set; }
            public Quaternion Rotation { get; set; }
        }
        
        public class UvMorph : MorphData {
            public int VertexIndex { get; set; }
            public Vector4 Offset { get; set; }
        }
        
        public class MaterialMorph : MorphData {
            public enum MaterialMorphMethod : byte {
                Multiply = 0x00,
                Add = 0x01
            }
            
            public int MaterialIndex { get; set; }
            public bool Global { get; set; }
            public MaterialMorphMethod Method { get; set; }
            public Color Diffuse { get; set; }
            public Color Specular { get; set; }
            public float Shininess { get; set; }
            public Color Ambient { get; set; }
            public Color EdgeColor { get; set; }
            public float EdgeSize { get; set; }
            public Vector4 Texture { get; set; }
            public Vector4 SubTexture { get; set; }
            public Vector4 ToonTexture { get; set; }
        }
    }
}