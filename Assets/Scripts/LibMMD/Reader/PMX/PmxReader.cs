using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using LibMMD.Material;
using LibMMD.Model;
using UnityEngine;

namespace LibMMD.Reader.PMX {
    public class PmxReader : ModelReader {
        private readonly static string[] GlobalToonNames = {
            "toon0.bmp",
            "toon01.bmp",
            "toon02.bmp",
            "toon03.bmp",
            "toon04.bmp",
            "toon05.bmp",
            "toon06.bmp",
            "toon07.bmp",
            "toon08.bmp",
            "toon09.bmp",
            "toon10.bmp"
        };
        
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
            string[] texturePaths = ReadTexturePaths(reader, config);
            ReadParts(reader, model, config, texturePaths);
            ReadBones(reader, model, config);
            ReadMorphs(reader, model, config);
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
        
        private  static string[] ReadTexturePaths(BinaryReader reader, PmxConfig config) {
            uint nbTextures = reader.ReadUInt32();
            string[] texturePaths = new string[nbTextures];
            
            for (uint i = 0; i < nbTextures; i++) {
                texturePaths[i] = ReaderUtil.ReadString(reader, config.Encoding);
            }
            return texturePaths;
        }
        
        private static void ReadParts(BinaryReader reader, MmdModel model, PmxConfig config, IReadOnlyList<string> texturePaths) {
            uint nbParts = reader.ReadUInt32();
            model.Parts = new Part[nbParts];
            
            int baseShift = 0;
            for (uint i = 0; i < nbParts; i++) {
                model.Parts[i] = ReadPart(reader, config, texturePaths, ref baseShift);
            }
        }
        
        private static Part ReadPart(BinaryReader reader, PmxConfig config, IReadOnlyList<string> texturePaths, ref int baseShift) {
            var part = new Part {
                Material = ReadMaterial(reader, config, texturePaths)
            };

            int triangleIndexCount = reader.ReadInt32();
            if (triangleIndexCount % 3 != 0) {
                throw new ModelParseException("Invalid number of triangles");
            }
            
            part.BaseShift = baseShift;
            part.TriangleIndexCount = triangleIndexCount;
            baseShift += triangleIndexCount;
            return part;
        }
        
        private static MmdMaterial ReadMaterial(BinaryReader reader, PmxConfig config, IReadOnlyList<string> texturePaths) {
            var material = new MmdMaterial {
                Name = ReaderUtil.ReadString(reader, config.Encoding),
                NameEnglish = ReaderUtil.ReadString(reader, config.Encoding),
                
                DiffuseColor = ReaderUtil.ReadColor(reader, true),
                SpecularColor = ReaderUtil.ReadColor(reader),
                Shininess = reader.ReadSingle(),
                AmbientColor = ReaderUtil.ReadColor(reader)
            };

            var drawFlag = (PmxMaterialDrawFlags) reader.ReadByte();
            material.DrawDoubleFace = (drawFlag & PmxMaterialDrawFlags.MaterialDrawDoubleFace) != 0;
            material.DrawGroundShadow = (drawFlag & PmxMaterialDrawFlags.MaterialDrawGroundShadow) != 0;
            material.CastSelfShadow = (drawFlag & PmxMaterialDrawFlags.MaterialCastSelfShadow) != 0;
            material.DrawSelfShadow = (drawFlag & PmxMaterialDrawFlags.MaterialDrawSelfShadow) != 0;
            material.DrawEdge = (drawFlag & PmxMaterialDrawFlags.MaterialDrawEdge) != 0;
            
            material.EdgeColor = ReaderUtil.ReadColor(reader, true);
            material.EdgeSize = reader.ReadSingle();
            
            int textureIndex = ReaderUtil.ReadIndex(reader, config.TextureIndexSize);
            // Check if the texture index is valid and set the texture path
            if (textureIndex < texturePaths.Count && textureIndex >= 0) {
                material.TexturePath = texturePaths[textureIndex];
            }
            
            int subTextureIndex = ReaderUtil.ReadIndex(reader, config.TextureIndexSize);
            // Check if the sub texture index is valid and set the sub texture path
            if (subTextureIndex < texturePaths.Count && subTextureIndex >= 0) {
                material.SubTexturePath = texturePaths[subTextureIndex];
            }
            material.SubTextureType = (MmdMaterial.SubTextureTypeFlags) reader.ReadByte();
            
            bool useGlobalToon = reader.ReadByte() != 0;
            if (useGlobalToon) {
                int globalToonTextureIndex = reader.ReadByte();
                if (globalToonTextureIndex < GlobalToonNames.Length - 1) {
                    material.ToonTexturePath = GlobalToonNames[globalToonTextureIndex + 1];
                }
            } else {
                int toonTextureIndex = ReaderUtil.ReadIndex(reader, config.TextureIndexSize);
                // Check if the toon texture index is valid and set the toon texture path
                if (toonTextureIndex < texturePaths.Count && toonTextureIndex >= 0) {
                    material.ToonTexturePath = texturePaths[toonTextureIndex];
                }
            }
            
            material.MetaInfo = ReaderUtil.ReadString(reader, config.Encoding);
            return material;
        }
        
        private static void ReadBones(BinaryReader reader, MmdModel model, PmxConfig config) {
            uint nbBones = reader.ReadUInt32();
            model.Bones = new Bone[nbBones];
            
            for (uint i = 0; i < nbBones; i++) {
                model.Bones[i] = ReadBone(reader, config, nbBones);
            }
        }

        private static Bone ReadBone(BinaryReader reader, PmxConfig config, uint nbBones) {
            var bone = new Bone {
                Name = ReaderUtil.ReadString(reader, config.Encoding),
                NameEnglish = ReaderUtil.ReadString(reader, config.Encoding),
                Position = ReaderUtil.ReadVector3(reader)
            };
            
            int parentIndex = ReaderUtil.ReadIndex(reader, config.BoneIndexSize);
            if (parentIndex < nbBones && parentIndex >= 0) {
                bone.ParentIndex = parentIndex;
            } else {
                bone.ParentIndex = -1;
            }
            
            bone.TransformLevel = reader.ReadInt32();
            
            var boneFlags = (PmxBoneFlags) reader.ReadUInt16();
            bone.Rotatable = (boneFlags & PmxBoneFlags.BoneRotatable) != 0;
            bone.Movable = (boneFlags & PmxBoneFlags.BoneMovable) != 0;
            bone.Visible = (boneFlags & PmxBoneFlags.BoneVisible) != 0;
            bone.Controllable = (boneFlags & PmxBoneFlags.BoneControllable) != 0;
            bone.HasIk = (boneFlags & PmxBoneFlags.BoneHasIk) != 0;
            bone.AppendRotate = (boneFlags & PmxBoneFlags.BoneAcquireRotate) != 0;
            bone.AppendTranslate = (boneFlags & PmxBoneFlags.BoneAcquireTranslate) != 0;
            bone.RotAxisFixed = (boneFlags & PmxBoneFlags.BoneRotAxisFixed) != 0;
            bone.UseLocalAxis = (boneFlags & PmxBoneFlags.BoneUseLocalAxis) != 0;
            bone.PostPhysics = (boneFlags & PmxBoneFlags.BonePostPhysics) != 0;
            bone.ReceiveTransform = (boneFlags & PmxBoneFlags.BoneReceiveTransform) != 0;
            bone.ChildBoneValue.ChildUseIndex = (boneFlags & PmxBoneFlags.BoneChildUseIndex) != 0;

            if (bone.ChildBoneValue.ChildUseIndex) {
                bone.ChildBoneValue.Index = ReaderUtil.ReadIndex(reader, config.BoneIndexSize);
            } else {
                bone.ChildBoneValue.Offset = ReaderUtil.ReadVector3(reader);
            }
            
            if (bone.RotAxisFixed) {
                bone.RotAxis = ReaderUtil.ReadVector3(reader);
            }
            
            if (bone.AppendRotate || bone.AppendTranslate) {
                bone.AppendBoneValue.Index = ReaderUtil.ReadIndex(reader, config.BoneIndexSize);
                bone.AppendBoneValue.Ratio = reader.ReadSingle();
            }

            if (bone.UseLocalAxis) {
                Vector3 localX = ReaderUtil.ReadVector3(reader);
                Vector3 localZ = ReaderUtil.ReadVector3(reader);
                Vector3 localY = Vector3.Cross(localZ, localX);
                localZ = Vector3.Cross(localX, localY);
                
                localX.Normalize();
                localY.Normalize();
                localZ.Normalize();
                
                bone.LocalAxisValue.AxisX = localX;
                bone.LocalAxisValue.AxisY = localY;
                bone.LocalAxisValue.AxisZ = localZ;
            }

            if (bone.ReceiveTransform) {
                bone.ExportKey = reader.ReadInt32();
            }
            
            if (bone.HasIk) {
                bone.Ik = ReadIk(reader, config);
            }
            
            return bone;
        }
        
        private static Bone.IkInfo ReadIk(BinaryReader reader, PmxConfig config) {
            var ik = new Bone.IkInfo {
                TargetIndex = ReaderUtil.ReadIndex(reader, config.BoneIndexSize),
                IterateLimit = reader.ReadInt32(),
                AngleLimit = reader.ReadSingle()
            };
            
            uint nbLinks = reader.ReadUInt32();
            ik.Links = new Bone.IkLink[nbLinks];
            
            for (uint i = 0; i < nbLinks; i++) {
                ik.Links[i] = ReadIkLink(reader, config);
            }
            return ik;
        }
        
        private static Bone.IkLink ReadIkLink(BinaryReader reader, PmxConfig config) {
            var link = new Bone.IkLink {
                LinkIndex = ReaderUtil.ReadIndex(reader, config.BoneIndexSize),
                HasLimit = reader.ReadByte() != 0
            };
            
            if (link.HasLimit) {
                link.MaxLimit = ReaderUtil.ReadVector3(reader);
                link.MinLimit = ReaderUtil.ReadVector3(reader);
            }
            return link;
        }
        
        private static void ReadMorphs(BinaryReader reader, MmdModel model, PmxConfig config) {
            uint nbMorphs = reader.ReadUInt32();
            model.Morphs = new Morph[nbMorphs];
            for (uint i = 0; i < nbMorphs; i++) {
                model.Morphs[i] = ReadMorph(reader, model, config);
            }
        }
        
        private static Morph ReadMorph(BinaryReader reader, MmdModel model, PmxConfig config) {
            var morph = new Morph {
                Name = ReaderUtil.ReadString(reader, config.Encoding),
                NameEnglish = ReaderUtil.ReadString(reader, config.Encoding),
                Category = (Morph.MorphCategory) reader.ReadByte()
            };
            
            if (morph.Category == Morph.MorphCategory.Face) {
                // TODO: Implement
            }
            
            morph.Type = (Morph.MorphType) reader.ReadByte();
            
            uint nbMorphDatas = reader.ReadUInt32();
            morph.MorphDatas = new Morph.MorphData[nbMorphDatas];
            
            for (uint i = 0; i < nbMorphDatas; i++) {
                switch (morph.Type) {
                    case Morph.MorphType.Flip:
                    case Morph.MorphType.Group:
                        morph.MorphDatas[i] = ReadGroupMorph(reader, config);
                        break;
                    case Morph.MorphType.Vertex:
                        morph.MorphDatas[i] = ReadVertexMorph(reader, config);
                        break;
                    case Morph.MorphType.Bone:
                        morph.MorphDatas[i] = ReadBoneMorph(reader, config);
                        break;
                    case Morph.MorphType.UV:
                    case Morph.MorphType.AdditionalUV1:
                    case Morph.MorphType.AdditionalUV2:
                    case Morph.MorphType.AdditionalUV3:
                    case Morph.MorphType.AdditionalUV4:
                        morph.MorphDatas[i] = ReadUvMorph(reader, config);
                        break;
                    case Morph.MorphType.Material:
                        morph.MorphDatas[i] = ReadMaterialMorph(reader, model, config);
                        break;
                    case Morph.MorphType.Impulse:
                        throw new ModelParseException("Impulse morphs are not supported");
                    default:
                        throw new ModelParseException("Unsupported morph type: " + morph.Type);
                }
            }

            return morph;
        }
        
        private static Morph.GroupMorph ReadGroupMorph(BinaryReader reader, PmxConfig config) {
            var morphData = new Morph.GroupMorph {
                MorphIndex = ReaderUtil.ReadIndex(reader, config.MorphIndexSize),
                MorphRate = reader.ReadSingle()
            };
            return morphData;
        }
        
        private static Morph.VertexMorph ReadVertexMorph(BinaryReader reader, PmxConfig config) {
            var morphData = new Morph.VertexMorph {
                VertexIndex = ReaderUtil.ReadIndex(reader, config.VertexIndexSize),
                Offset = ReaderUtil.ReadVector3(reader)
            };
            return morphData;
        }
        
        private static Morph.BoneMorph ReadBoneMorph(BinaryReader reader, PmxConfig config) {
            var morphData = new Morph.BoneMorph {
                BoneIndex = ReaderUtil.ReadIndex(reader, config.BoneIndexSize),
                Translation = ReaderUtil.ReadVector3(reader),
                Rotation = ReaderUtil.ReadQuaternion(reader)
            };
            return morphData;
        }
        
        private static Morph.UvMorph ReadUvMorph(BinaryReader reader, PmxConfig config) {
            var morphData = new Morph.UvMorph {
                VertexIndex = ReaderUtil.ReadIndex(reader, config.VertexIndexSize),
                Offset = ReaderUtil.ReadVector4(reader)
            };
            return morphData;
        }
        
        private static Morph.MaterialMorph ReadMaterialMorph(BinaryReader reader, MmdModel model, PmxConfig config) {
            var morphData = new Morph.MaterialMorph();
            int index = ReaderUtil.ReadIndex(reader, config.MaterialIndexSize);
            if (index < model.Parts.Length && index > 0) {
                morphData.MaterialIndex = index;
                morphData.Global = false;
            } else {
                morphData.MaterialIndex = 0;
                morphData.Global = true;
            }
            
            morphData.Method = (Morph.MaterialMorph.MaterialMorphMethod) reader.ReadByte();
            morphData.Diffuse = ReaderUtil.ReadColor(reader, true);
            morphData.Specular = ReaderUtil.ReadColor(reader);
            morphData.Shininess = reader.ReadSingle();
            morphData.Ambient = ReaderUtil.ReadColor(reader);
            morphData.EdgeColor = ReaderUtil.ReadColor(reader, true);
            morphData.EdgeSize = reader.ReadSingle();
            morphData.Texture = ReaderUtil.ReadVector4(reader);
            morphData.SubTexture = ReaderUtil.ReadVector4(reader);
            morphData.ToonTexture = ReaderUtil.ReadVector4(reader);
            return morphData;
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
        
        [Flags]
        private enum PmxMaterialDrawFlags {
            MaterialDrawDoubleFace = 1 << 0,
            MaterialDrawGroundShadow = 1 << 1,
            MaterialCastSelfShadow = 1 << 2,
            MaterialDrawSelfShadow = 1 << 3,
            MaterialDrawEdge = 1 << 4
        }
        
        [Flags]
        private enum PmxBoneFlags {
            BoneChildUseIndex = 1 << 0,
            BoneRotatable = 1 << 1,
            BoneMovable = 1 << 2, 
            BoneVisible = 1 << 3, 
            BoneControllable = 1 << 4, 
            BoneHasIk = 1 << 5,
            BoneAcquireRotate = 1 << 8, 
            BoneAcquireTranslate = 1 << 9, 
            BoneRotAxisFixed = 1 << 10, 
            BoneUseLocalAxis = 1 << 11, 
            BonePostPhysics = 1 << 12, 
            BoneReceiveTransform = 1 << 13, 
        }
    }
}