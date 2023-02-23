using UnityEngine;

namespace MMD.PMX {
    public class Vertex {
        public Vector3 Position { get; set; }
        public Vector3 Normal { get; set; }
        public Vector2 UV { get; set; }
        public Vector4[] AdditionalUV { get; set; }
        public IBoneWeight BoneWeight { get; set; }
        public float EdgeMagnification { get; set; }
    }
}
