namespace LibMMD.Reader.PMX {
    public class PmxHeader {
        public string Magic { get; set; }
        public float Version { get; set; }
        public byte FileFlagSize { get; set; }
    }
}