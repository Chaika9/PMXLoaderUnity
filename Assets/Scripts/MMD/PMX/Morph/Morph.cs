namespace MMD.PMX {
    public class Morph {
        public string Name { get; set; }
        public string NameEnglish { get; set; }
        public PMXFormat.MorphPanel Panel { get; set; }
        public PMXFormat.MorphType Type { get; set; }
        public IMorphOffset[] Offsets { get; set; }
    }
}
