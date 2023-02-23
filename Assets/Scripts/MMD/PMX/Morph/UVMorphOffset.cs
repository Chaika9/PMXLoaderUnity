using UnityEngine;

namespace MMD.PMX {
    public class UVMorphOffset : IMorphOffset {
        public uint VertexIndex { get; set; }
        public Vector4 UVOffset { get; set; }
    }
}
