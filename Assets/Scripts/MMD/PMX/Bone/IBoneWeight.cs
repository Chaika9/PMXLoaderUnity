using UnityEngine;

namespace MMD.PMX {
    public interface IBoneWeight {
        PMXFormat.WeightMethod Method { get; }

        public uint BoneIndex1 { get; }
        public uint BoneIndex2 { get; }
        public uint BoneIndex3 { get; }
        public uint BoneIndex4 { get; }

        public float BoneWeight1 { get; }
        public float BoneWeight2 { get; }
        public float BoneWeight3 { get; }
        public float BoneWeight4 { get; }

        public Vector3 C { get; }
        public Vector3 R0 { get; }
        public Vector3 R1 { get; }
    }
}
