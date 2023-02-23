using System;

namespace MMD.PMX {
    public class PMXFormat {
        public PMXHeader Header { get; set; }
        public Vertex[] Vertices { get; set; }
        public Face[] Faces { get; set; }
        public string[] Textures { get; set; }
        public Material[] Materials { get; set; }
        public Bone[] Bones { get; set; }
        public Morph[] Morphs { get; set; }
        public DisplayFrame[] DisplayFrames { get; set; }
        public RigidBody[] RigidBodies { get; set; }
        public Joint[] Joints { get; set; }

        public enum StringEncode {
            UTF16Le = 0,
            UTF8 = 1
        }

        public enum IndexSize {
            Size1 = 1,
            Size2 = 2,
            Size4 = 4
        }

        public enum WeightMethod {
            Bdef1 = 0,
            Bdef2 = 1,
            Bdef4 = 2,
            Sdef = 3,
            Qdef = 4
        }

        [Flags]
        public enum BoneFlag {
            Connection = 1 << 0, // Connection destination (specify PMD child bone) display method (ON: specify by bone, OFF: specify by coordinate offset)
            Rotatable = 1 << 1,
            Movable = 1 << 2, 
            DisplayFlag = 1 << 3, 
            CanOperate = 1 << 4, 
            IK = 1 << 5, 
            AddLocal = 1 << 7, // Local grant | Grant target (ON: parent's local transformation amount, OFF: user transformation value/IK link/multiple grant)
            AddRotation = 1 << 8, 
            AddMove = 1 << 9, 
            FixedAxis = 1 << 10, 
            LocalAxis = 1 << 11, 
            PhysicsTransform = 1 << 12, 
            ExternalParentTransform = 1 << 13, 
        }
        
        public enum MorphPanel {
            Face = 0,
            LeftEye = 1,
            RightEye = 2,
            Mouth = 3,
            Other = 4
        }
        
        public enum MorphType {
            Group = 0,
            Vertex = 1,
            Bone = 2,
            UV = 3,
            AdditionalUV1 = 4,
            AdditionalUV2 = 5,
            AdditionalUV3 = 6,
            AdditionalUV4 = 7,
            Material = 8,
            Flip = 9,
            Impulse = 10
        }
        
        public enum DisplayFrameElementType {
            Bone = 0,
            Morph = 1
        }
        
        public enum RigidBodyShapeType {
            Sphere = 0,
            Box = 1,
            Capsule = 2
        }
        
        public enum RigidBodyOperationType {
            Static = 0,
            Dynamic = 1,
            DynamicAndPositionAdjust = 2
        }
        
        public enum JointOperationType {
            Spring6DOF = 0,
            P2P = 1,
            ConeTwist = 2,
            Slider = 3,
            Hinge = 4
        }

        public class PMXHeader {
            public string ModelName { get; set; }
            public string ModelNameEnglish { get; set; }
            public string Comment { get; set; }
            public string CommentEnglish { get; set; }

            public StringEncode Encode { get; set; }

            public byte AdditionalUV { get; set; }

            public IndexSize VertexIndexSize { get; set; }
            public IndexSize TextureIndexSize { get; set; }
            public IndexSize MaterialIndexSize { get; set; }
            public IndexSize BoneIndexSize { get; set; }
            public IndexSize MorphIndexSize { get; set; }
            public IndexSize RigidbodyIndexSize { get; set; }
        }
    }
}
