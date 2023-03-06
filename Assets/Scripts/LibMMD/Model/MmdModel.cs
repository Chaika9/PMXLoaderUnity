﻿namespace LibMMD.Model {
    public class MmdModel {
        public string Name { get; set; }
        public string NameEnglish { get; set; }
        public string Comment { get; set; }
        public string CommentEnglish { get; set; }
        public Vertex[] Vertices { get; set; }
    }
}