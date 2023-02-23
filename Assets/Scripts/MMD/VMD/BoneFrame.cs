using UnityEngine;

namespace MMD.VMD {
    public class BoneFrame {
        public string BoneName { get; set; }
        public uint FrameNumber { get; set; }
        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; }
        
        public Curve XCurve { get; set; }
        public Curve YCurve { get; set; }
        public Curve ZCurve { get; set; }
        public Curve RCurve { get; set; }
    }
}
