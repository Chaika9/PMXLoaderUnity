using System.IO;
using LibMMD.Model;

namespace LibMMD.Reader.PMX {
    public class PmxReader : ModelReader {
        protected override MmdModel ReadModel(BinaryReader reader) {
            throw new System.NotImplementedException();
        }
        
        private PmxReader ReadHeader(BinaryReader reader) {
            throw new System.NotImplementedException();
        }
        
        private PmxConfig ReadConfig(BinaryReader reader, MmdModel model) {
            throw new System.NotImplementedException();
        }
        
        private void ReadModelInfo(BinaryReader reader, MmdModel model) {
            throw new System.NotImplementedException();
        }
        
        private void ReadVertices(BinaryReader reader, MmdModel model) {
            throw new System.NotImplementedException();
        }
        
        private void ReadTriangles(BinaryReader reader, MmdModel model) {
            throw new System.NotImplementedException();
        }
        
        private void ReadTextures(BinaryReader reader, MmdModel model) {
            throw new System.NotImplementedException();
        }
        
        private void ReadParts(BinaryReader reader, MmdModel model) {
            throw new System.NotImplementedException();
        }
        
        private void ReadBones(BinaryReader reader, MmdModel model) {
            throw new System.NotImplementedException();
        }
        
        private void ReadMorphs(BinaryReader reader, MmdModel model) {
            throw new System.NotImplementedException();
        }
        
        private void ReadEntries(BinaryReader reader, MmdModel model) {
            throw new System.NotImplementedException();
        }
        
        private void ReadRigidBodies(BinaryReader reader, MmdModel model) {
            throw new System.NotImplementedException();
        }
        
        private void ReadConstraints(BinaryReader reader, MmdModel model) {
            throw new System.NotImplementedException();
        }
    }
}