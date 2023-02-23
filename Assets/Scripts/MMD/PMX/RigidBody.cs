using UnityEngine;

namespace MMD.PMX {
    public class RigidBody {
        public string Name { get; set; }
        public string NameEnglish { get; set; }
        public uint BoneIndex { get; set; }
        public byte GroupIndex { get; set; }
        
        public ushort IgnoreCollisionGroup { get; set; }
        
        public PMXFormat.RigidBodyShapeType ShapeType { get; set; }
        public Vector3 ShapeSize { get; set; }
        
        public Vector3 CollisionPosition { get; set; }
        public Vector3 CollisionRotation { get; set; }
        
        public float Weight { get; set; }
        
        public float PositionDamping { get; set; }
        public float RotationDamping { get; set; }
        
        public float Recoil { get; set; }
        public float Friction { get; set; }
        
        public PMXFormat.RigidBodyOperationType OperationType { get; set; }
    }
}
