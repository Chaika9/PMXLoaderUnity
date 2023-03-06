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
            ReadVertices(reader, model, config);
            ReadTriangles(reader, model, config);
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
                    skinningOperator.Param = ReadSkinningParamBdef2(reader, config);
                    break;
                case SkinningOperator.SkinningType.Qdef:
                case SkinningOperator.SkinningType.Bdef4:
                    skinningOperator.Param = ReadSkinningParamBdef4(reader, config);
                    break;
                case SkinningOperator.SkinningType.Sdef:
                    skinningOperator.Param = ReadSkinningParamSdef(reader, config);
                    break;
                default:
                    throw new ModelParseException("Unknown skinning type");
            }
            
            vertex.SkinningOperator = skinningOperator;
            
            vertex.EdgeScale = reader.ReadSingle();
            return vertex;
        }
        
        private static SkinningOperator.SkinningParam ReadSkinningParamBdef1(BinaryReader reader, PmxConfig config) {
            var param = new SkinningOperator.Bdef1 {
                BoneIndices = ReaderUtil.ReadIndex(reader, config.BoneIndexSize)
            };
            return param;
        }
        
        private static SkinningOperator.SkinningParam ReadSkinningParamBdef2(BinaryReader reader, PmxConfig config) {
            var param = new SkinningOperator.Bdef2 {
                BoneIndices = new int[2]
            };
            
            param.BoneIndices[0] = ReaderUtil.ReadIndex(reader, config.BoneIndexSize);
            param.BoneIndices[1] = ReaderUtil.ReadIndex(reader, config.BoneIndexSize);
            param.BoneWeight = reader.ReadSingle();
            return param;
        }
        
        private static SkinningOperator.SkinningParam ReadSkinningParamBdef4(BinaryReader reader, PmxConfig config) {
            var param = new SkinningOperator.Bdef4 {
                BoneIndices = new int[4],
                BoneWeights = new float[4]
            };
            
            param.BoneIndices[0] = ReaderUtil.ReadIndex(reader, config.BoneIndexSize);
            param.BoneIndices[1] = ReaderUtil.ReadIndex(reader, config.BoneIndexSize);
            param.BoneIndices[2] = ReaderUtil.ReadIndex(reader, config.BoneIndexSize);
            param.BoneIndices[3] = ReaderUtil.ReadIndex(reader, config.BoneIndexSize);
            param.BoneWeights[0] = reader.ReadSingle();
            param.BoneWeights[1] = reader.ReadSingle();
            param.BoneWeights[2] = reader.ReadSingle();
            param.BoneWeights[3] = reader.ReadSingle();
            return param;
        }
        
        private static SkinningOperator.SkinningParam ReadSkinningParamSdef(BinaryReader reader, PmxConfig config) {
            var param = new SkinningOperator.Sdef {
                BoneIndices = new int[2]
            };
            
            param.BoneIndices[0] = ReaderUtil.ReadIndex(reader, config.BoneIndexSize);
            param.BoneIndices[1] = ReaderUtil.ReadIndex(reader, config.BoneIndexSize);
            param.BoneWeight = reader.ReadSingle();
            param.C = ReaderUtil.ReadVector3(reader);
            param.R0 = ReaderUtil.ReadVector3(reader);
            param.R1 = ReaderUtil.ReadVector3(reader);
            return param;
        }

        private static void ReadTriangles(BinaryReader reader, MmdModel model, PmxConfig config) {
            uint nbTriangles = reader.ReadUInt32();
            model.Triangles = new int[nbTriangles];
            
            if (nbTriangles % 3 != 0) {
                throw new ModelParseException("Invalid number of triangles");
            }
            
            for (uint i = 0; i < nbTriangles; i++) {
                model.Triangles[i] = ReaderUtil.ReadIndex(reader, config.VertexIndexSize);
            }
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