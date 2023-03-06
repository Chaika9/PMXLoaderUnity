using LibMMD.Material;

namespace LibMMD.Model {
    public class Part {
        public MmdMaterial Material { get; set; }
        public int BaseShift { get; set; }
        public int TriangleIndexCount { get; set; }
    }
}