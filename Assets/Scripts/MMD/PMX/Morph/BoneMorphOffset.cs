using UnityEngine;

namespace MMD.PMX {
    public class BoneMorphOffset : IMorphOffset {
        public uint BoneIndex { get; set; }
        public Vector3 Translation { get; set; }
        public Quaternion Rotation { get; set; }
    }
}
