using UnityEngine;

namespace MMD.PMX {
    public class BDEF4Vertex : IBoneWeight {
        public PMXFormat.WeightMethod Method => PMXFormat.WeightMethod.Bdef4;

        public uint BoneIndex1 { get; set; }
        public uint BoneIndex2 { get; set; }
        public uint BoneIndex3 { get; set; }
        public uint BoneIndex4 { get; set; }

        public float BoneWeight1 { get; set; }
        public float BoneWeight2 { get; set; }
        public float BoneWeight3 { get; set; }
        public float BoneWeight4 { get; set; }

        public Vector3 C => Vector3.zero;
        public Vector3 R0 => Vector3.zero;
        public Vector3 R1 => Vector3.zero;
    }
}
