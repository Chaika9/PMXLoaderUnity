using UnityEngine;

namespace LibMMD.Model {
    public class SkinningOperator {
        public SkinningType Type { get; set; }
        public SkinningParam Param { get; set; }
        
        public enum SkinningType : byte {
            Bdef1 = 0,
            Bdef2 = 1,
            Bdef4 = 2,
            Sdef = 3,
            Qdef = 4
        }
        
        public abstract class SkinningParam {}
        
        public class Bdef1 : SkinningParam {
            public int BoneIndices { get; set; }
        }

        public class Bdef2 : SkinningParam {
            public int[] BoneIndices { get; set; }
            public float BoneWeight { get; set; }
        }
        
        public class Bdef4 : SkinningParam {
            public int[] BoneIndices { get; set; }
            public float[] BoneWeights { get; set; }
        }
        
        public class Sdef : SkinningParam {
            public int[] BoneIndices { get; set; }
            public float BoneWeight { get; set; }
            public Vector3 C { get; set; }
            public Vector3 R0 { get; set; }
            public Vector3 R1 { get; set; }
        }
    }
}