using UnityEngine;

namespace MMD.PMX {
    public class Material {
        public string Name { get; set; }
        public string NameEnglish { get; set; }

        public Color DiffuseColor { get; set; }
        public Color SpecularColor { get; set; }

        public float SpecularPower { get; set; }

        public Color AmbientColor { get; set; }

        public byte Flags { get; set; }

        public Color EdgeColor { get; set; }
        public float EdgeSize { get; set; }

        public uint TextureIndex { get; set; }
        public uint SphereTextureIndex { get; set; }

        public byte SphereMode { get; set; }
        public byte SharedToonFlag { get; set; }

        public int ToonTextureIndex { get; set; }

        public string Memo { get; set; }

        public int FaceCount { get; set; }
    }
}
    