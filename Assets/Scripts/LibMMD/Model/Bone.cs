using UnityEngine;

namespace LibMMD.Model {
    public class Bone {
        public Bone() {
            ChildBoneValue = new ChildBone();
            AppendBoneValue = new AppendBone();
            LocalAxisValue = new LocalAxis();
        }
        
        public string Name { get; set; }
        public string NameEnglish { get; set; }
        public Vector3 Position { get; set; }
        
        public int ParentIndex { get; set; }
        
        public int TransformLevel { get; set; }
        public bool Rotatable { get; set; }
        public bool Movable { get; set; }
        public bool Visible { get; set; }
        public bool Controllable { get; set; }
        public bool HasIk { get; set; }
        public bool AppendRotate { get; set; }
        public bool AppendTranslate { get; set; }
        public bool RotAxisFixed { get; set; }
        public bool UseLocalAxis { get; set; }
        public bool PostPhysics { get; set; }
        public bool ReceiveTransform { get; set; }
        public ChildBone ChildBoneValue { get; }
        public AppendBone AppendBoneValue { get; }
        public Vector3 RotAxis { get; set; }
        public LocalAxis LocalAxisValue { get; }
        public int ExportKey { get; set; }
        public IkInfo Ik { get; set; }

        public class ChildBone {
            public bool ChildUseIndex { get; set; }
            public object Index { get; set; }
            public Vector3 Offset { get; set; }
        }

        public class AppendBone {
            public int Index { get; set; }
            public float Ratio { get; set; }
        }

        public class LocalAxis {
            public Vector3 AxisX { get; set; }
            public Vector3 AxisY { get; set; }
            public Vector3 AxisZ { get; set; }
        }

        public class IkInfo {
            public int TargetIndex { get; set; }
            public int IterateLimit { get; set; }
            public float AngleLimit { get; set; }
            public IkLink[] Links { get; set; }
        }

        public class IkLink {
            public int LinkIndex { get; set; }
            public bool HasLimit { get; set; }
            public Vector3 MinLimit { get; set; }
            public Vector3 MaxLimit { get; set; }
        }
    }
}