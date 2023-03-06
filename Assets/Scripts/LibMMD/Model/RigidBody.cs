using UnityEngine;

namespace LibMMD.Model {
    public class RigidBody {
        public string Name { get; set; }
        public string NameEnglish { get; set; }
        public int AssociatedBoneIndex { get; set; }
        public byte CollisionGroup { get; set; }
        public ushort CollisionMask { get; set; }
        public RigidBodyShape Shape { get; set; }
        public Vector3 Dimemsions { get; set; }
        public Vector3 Position { get; set; }
        public Vector3 Rotation { get; set; }
        public float Mass { get; set; }
        public float TranslateDamp { get; set; }
        public float RotateDamp { get; set; }
        public float Restitution { get; set; }
        public float Friction { get; set; }
        public RigidBodyType Type { get; set; }

        public enum RigidBodyShape : byte {
            Sphere = 0x00,
            Box = 0x01,
            Capsule = 0x02
        }
        
        public enum RigidBodyType : byte {
            Kinematic = 0x00,
            Physics = 0x01,
            PhysicsStrict = 0x02,
            PhysicsGhost = 0x03
        }
    }
}