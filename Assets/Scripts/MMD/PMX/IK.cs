namespace MMD.PMX {
    public class IK {
        public uint BoneIndex { get; set; }
        public uint Iteration { get; set; } // Number of recursive operations
        public float AngleLimit { get; set; }
        public Link[] Links { get; set; }
    }
}
