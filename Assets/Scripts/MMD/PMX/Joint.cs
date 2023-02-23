using UnityEngine;

namespace MMD.PMX {
    public class Joint {
        public string Name { get; set; }
        public string NameEnglish { get; set; }
        
        public PMXFormat.JointOperationType OperationType { get; set; }
        
        public uint RigidBodyAIndex { get; set; }
        public uint RigidBodyBIndex { get; set; }
        
        public Vector3 Position { get; set; }
        public Vector3 Rotation { get; set; }
        
        public Vector3 ConstrainPositionLower { get; set; }
        public Vector3 ConstrainPositionUpper { get; set; }
        
        public Vector3 ConstrainRotationLower { get; set; }
        public Vector3 ConstrainRotationUpper { get; set; }
        
        public Vector3 SpringPosition { get; set; }
        public Vector3 SpringRotation { get; set; }
    }
}
