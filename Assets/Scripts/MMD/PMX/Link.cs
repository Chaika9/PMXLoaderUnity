using UnityEngine;

namespace MMD.PMX {
    public class Link {
        public uint BoneIndex { get; set; }
        public byte AngleLimit { get; set; }
        public Vector3 LowerLimit { get; set; }
        public Vector3 UpperLimit { get; set; }
    }
}
