namespace LibMMD.Model {
    public class MmdModel {
        public string Name { get; set; }
        public string NameEnglish { get; set; }
        public string Comment { get; set; }
        public string CommentEnglish { get; set; }
        public Vertex[] Vertices { get; set; }
        public int AdditionalUVCount { get; set; }
        public int[] Triangles { get; set; }
        public Part[] Parts { get; set; }
        public Bone[] Bones { get; set; }
        public Morph[] Morphs { get; set; }
        public RigidBody[] RigidBodies { get; set; }
        public Constraint[] Constraints { get; set; }
    }
}