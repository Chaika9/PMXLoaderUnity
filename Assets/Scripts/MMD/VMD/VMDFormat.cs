namespace MMD.VMD {
    public class VMDFormat {
        public VMDHeader Header { get; set; }
        
        public BoneFrame[] BoneFrames { get; set; }
        public MorphFrame[] MorphFrames { get; set; }
        public CameraFrame[] CameraFrames { get; set; }
        
        public class VMDHeader {
            public string ModelReferenceName { get; set; }
        }
    }
}
