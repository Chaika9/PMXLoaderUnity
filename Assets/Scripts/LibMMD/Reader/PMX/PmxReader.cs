using System;
using System.IO;
using System.Text;
using LibMMD.Model;
using UnityEngine;

namespace LibMMD.Reader.PMX {
    public class PmxReader : ModelReader {
        protected override MmdModel ReadModel(BinaryReader reader) {
            PmxHeader header = ReadHeader(reader);
            
            // Check magic
            if (!"PMX ".Equals(header.Magic)) {
                throw new ModelFormatException("Invalid PMX file");
            }
            
            // Check version (only support 2.0)
            if (Math.Abs(header.Version - 2.0f) > 0.0001f) {
                throw new ModelFormatException("Unsupported PMX version");
            }
            
            var model = new MmdModel();
            PmxConfig config = ReadConfig(reader, model);
            ReadModelInfo(reader, model, config);
            return model;
        }
        
        private static PmxHeader ReadHeader(BinaryReader reader) {
            var header = new PmxHeader {
                Magic = ReaderUtil.ReadString(reader, Encoding.ASCII, 4),
                Version = reader.ReadSingle(),
                FileFlagSize = reader.ReadByte()
            };
            return header;
        }
        
        private static PmxConfig ReadConfig(BinaryReader reader, MmdModel model) {
            var config = new PmxConfig {
                IsUtf8Encoding = reader.ReadByte() != 0,
                AdditionalUVCount = reader.ReadByte(),
                VertexIndexSize = reader.ReadByte(),
                TextureIndexSize = reader.ReadByte(),
                MaterialIndexSize = reader.ReadByte(),
                BoneIndexSize = reader.ReadByte(),
                MorphIndexSize = reader.ReadByte(),
                RigidBodyIndexSize = reader.ReadByte()
            };
            
            // Set encoding
            config.Encoding = config.IsUtf8Encoding ? Encoding.UTF8 : Encoding.Unicode;
            
            model.AdditionalUVCount = config.AdditionalUVCount;
            return config;
        }
        
        private static void ReadModelInfo(BinaryReader reader, MmdModel model, PmxConfig config) {
            model.Name = ReaderUtil.ReadString(reader, config.Encoding);
            model.NameEnglish = ReaderUtil.ReadString(reader, config.Encoding);
            model.Comment = ReaderUtil.ReadString(reader, config.Encoding);
            model.CommentEnglish = ReaderUtil.ReadString(reader, config.Encoding);
        }
        
        private static void ReadVertices(BinaryReader reader, MmdModel model, PmxConfig config) {
            uint nbVertices = reader.ReadUInt32();
            model.Vertices = new Vertex[nbVertices];

            for (uint i = 0; i < nbVertices; i++) {
                model.Vertices[i] = ReadVertex(reader, config);
            }
        }

        private static Vertex ReadVertex(BinaryReader reader, PmxConfig config) {
            var vertex = new Vertex {
                Position = ReaderUtil.ReadVector3(reader),
                Normal = ReaderUtil.ReadVector3(reader),
                UvPosition = ReaderUtil.ReadVector2(reader)
            };
            
            // Additional UVs
            if (config.AdditionalUVCount > 0) {
                vertex.AdditionalUvPositions = new Vector4[config.AdditionalUVCount];
                for (int i = 0; i < config.AdditionalUVCount; i++) {
                    vertex.AdditionalUvPositions[i] = ReaderUtil.ReadVector4(reader);
                }
            }

            var skinningOperator = new SkinningOperator();
            var skinningType = (SkinningOperator.SkinningType) reader.ReadByte();
            skinningOperator.Type = skinningType;
            
            switch (skinningType) {
                case SkinningOperator.SkinningType.Bdef1:
                    skinningOperator.Param = ReadSkinningParamBdef1(reader, config);
                    break;
                case SkinningOperator.SkinningType.Bdef2:
                    break;
                case SkinningOperator.SkinningType.Qdef:
                case SkinningOperator.SkinningType.Bdef4:
                    break;
                case SkinningOperator.SkinningType.Sdef:
                    break;
                default:
                    throw new ModelFormatException("Invalid skinning type: " + skinningType);
            }

            return vertex;
        }
        
        private static SkinningOperator.SkinningParam ReadSkinningParamBdef1(BinaryReader reader, PmxConfig config) {
            var param = new SkinningOperator.Bdef1 {
                BoneIndex = ReaderUtil.ReadIndex(reader, config.BoneIndexSize)
            };
            return param;
        }

        private static void ReadTriangles(BinaryReader reader, MmdModel model) {
            throw new System.NotImplementedException();
        }
        
        private  static void ReadTextures(BinaryReader reader, MmdModel model) {
            throw new System.NotImplementedException();
        }
        
        private static void ReadParts(BinaryReader reader, MmdModel model) {
            throw new System.NotImplementedException();
        }
        
        private static void ReadBones(BinaryReader reader, MmdModel model) {
            throw new System.NotImplementedException();
        }
        
        private static void ReadMorphs(BinaryReader reader, MmdModel model) {
            throw new System.NotImplementedException();
        }
        
        private static void ReadEntries(BinaryReader reader, MmdModel model) {
            throw new System.NotImplementedException();
        }
        
        private static void ReadRigidBodies(BinaryReader reader, MmdModel model) {
            throw new System.NotImplementedException();
        }
        
        private static void ReadConstraints(BinaryReader reader, MmdModel model) {
            throw new System.NotImplementedException();
        }
    }
}