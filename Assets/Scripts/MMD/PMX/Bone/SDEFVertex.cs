using UnityEngine;

namespace MMD.PMX {
    public class SDEFVertex : IBoneWeight {
        public PMXFormat.WeightMethod Method => PMXFormat.WeightMethod.Sdef;

        public uint BoneIndex1 { get; set; }
        public uint BoneIndex2 { get; set; }
        public uint BoneIndex3 { get; set; }
        public uint BoneIndex4 { get; set; }

        public float BoneWeight1 { get; set; }
        public float BoneWeight2 => 1.0f - BoneWeight1;
        public float BoneWeight3 => 0.0f;
        public float BoneWeight4 => 0.0f;

        public Vector3 C { get; set; }
        public Vector3 R0 { get; set; }
        public Vector3 R1 { get; set; }
    }
}
