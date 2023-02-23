using UnityEngine;

namespace MMD.PMX {
    public class VertexMorphOffset : IMorphOffset {
        public uint VertexIndex { get; set; }
        public Vector3 PositionOffset { get; set; }
    }
}
