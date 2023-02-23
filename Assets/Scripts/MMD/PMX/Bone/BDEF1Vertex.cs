using UnityEngine;

namespace MMD.PMX {
    public class BDEF1Vertex : IBoneWeight {
        public PMXFormat.WeightMethod Method => PMXFormat.WeightMethod.Bdef1;

        public uint BoneIndex1 { get; set; }
        public uint BoneIndex2 => 0;
        public uint BoneIndex3 => 0;
        public uint BoneIndex4 => 0;

        public float BoneWeight1 => 1.0f;
        public float BoneWeight2 => 0.0f;
        public float BoneWeight3 => 0.0f;
        public float BoneWeight4 => 0.0f;

        public Vector3 C => Vector3.zero;
        public Vector3 R0 => Vector3.zero;
        public Vector3 R1 => Vector3.zero;
    }
}
