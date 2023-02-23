using UnityEngine;

namespace MMD.VMD {
    public class CameraFrame {
        public uint FrameNumber { get; set; }
        public float Distance { get; set; }
        public Vector3 Position { get; set; }
        public Vector3 Rotation { get; set; }
        public Curve Curve { get; set; }
        public uint FOV { get; set; }
        public bool Orthographic { get; set; }
    }
}
