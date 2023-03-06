using System.Text;

namespace LibMMD.Reader.PMX {
    public class PmxConfig {
        public bool IsUtf8Encoding { get; set; }
        public Encoding Encoding { get; set; }
        public int AdditionalUVCount { get; set; }
        public int VertexIndexSize { get; set; }
        public int TextureIndexSize { get; set; }
        public int MaterialIndexSize { get; set; }
        public int BoneIndexSize { get; set; }
        public int MorphIndexSize { get; set; }
        public int RigidBodyIndexSize { get; set; }
    }
}