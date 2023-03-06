using UnityEngine;

namespace LibMMD.Material {
    public class MmdMaterial {
        public string Name { get; set; }
        public string NameEnglish { get; set; }
        
        public Color DiffuseColor { get; set; }
        public Color SpecularColor { get; set; }
        public Color AmbientColor { get; set; }
        
        public float Shininess { get; set; }
        
        public bool DrawDoubleFace { get; set; }
        public bool DrawGroundShadow { get; set; }
        public bool CastSelfShadow { get; set; }
        public bool DrawSelfShadow { get; set; }
        public bool DrawEdge { get; set; }
        
        public Color EdgeColor { get; set; }
        public float EdgeSize { get; set; }
        
        public string ToonTexturePath { get; set; }
        public string TexturePath { get; set; }
        public string SubTexturePath { get; set; }
        public SubTextureTypeFlags SubTextureType { get; set; }
        
        public string MetaInfo { get; set; }
        
        public enum SubTextureTypeFlags : byte {
            MatSubTexOff = 0,
            MatSubTexSph = 1,
            MatSubTexSpa = 2,
            MatSubTexSub = 3
        }
    }
}
