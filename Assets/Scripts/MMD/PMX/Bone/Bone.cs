using UnityEngine;

namespace MMD.PMX {
    public class Bone {
        public string Name { get; set; }
        public string NameEnglish { get; set; }
        
        public Vector3 Position { get; set; }
        
        public uint ParentBoneIndex { get; set; }
        public int TransformLevel { get; set; }
        public PMXFormat.BoneFlag Flag { get; set; }
        
        public Vector3 ConnectionPosition { get; set; }
        public uint ConnectionBoneIndex { get; set; }
        
        public uint AdditionalParentBoneIndex { get; set; }
        public float AdditionalRate { get; set; }
        
        public Vector3 AxisDirection { get; set; }

        public Vector3 LocalXAxisDirection { get; set; }
        public Vector3 LocalZAxisDirection { get; set; }
        
        public uint ExternalParentBoneIndex { get; set; }
        
        public IK IK { get; set; }
    }
}