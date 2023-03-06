using UnityEngine;

namespace LibMMD.Model {
    public class Vertex {
        public Vector3 Position { get; set; }
        public Vector3 Normal { get; set; }
        public Vector2 UvPosition { get; set; }
        public Vector4[] AdditionalUvPositions { get; set; }
    }
}