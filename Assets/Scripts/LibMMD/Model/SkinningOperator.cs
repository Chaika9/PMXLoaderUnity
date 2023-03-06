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
            public int BoneIndex { get; set; }
        }
    }
}