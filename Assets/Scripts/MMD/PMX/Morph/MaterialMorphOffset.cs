using UnityEngine;

namespace MMD.PMX {
    public enum OffsetMethod {
        Multiply,
        Add
    }
    
    public class MaterialMorphOffset : IMorphOffset {
        public uint MaterialIndex { get; set; }
        public OffsetMethod OffsetMethod { get; set; }
        public Color Diffuse { get; set; }
        public Color Specular { get; set; }
        public float SpecularPower { get; set; }
        public Color Ambient { get; set; }
        public Color EdgeColor { get; set; }
        public float EdgeSize { get; set; }
        public Color TextureCoefficient { get; set; }
        public Color SphereTextureCoefficient { get; set; }
        public Color ToonTextureCoefficient { get; set; }
    }
}
