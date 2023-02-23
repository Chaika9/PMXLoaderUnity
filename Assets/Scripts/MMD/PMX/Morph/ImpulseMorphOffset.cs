using UnityEngine;

namespace MMD.PMX {
    public class ImpulseMorphOffset : IMorphOffset {
        public uint RigidBodyIndex { get; set; }
        public byte LocalFlag { get; set; }
        public Vector3 Velocity { get; set; }
        public Vector3 RotationTorque { get; set; }
    }
}
