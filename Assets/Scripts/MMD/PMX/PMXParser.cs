using System;
using System.IO;
using System.Text;
using UnityEngine;

namespace MMD.PMX {
    public static class PMXParser {
        public static PMXFormat Load(string path) {
            if (!File.Exists(path)) {
                throw new FileNotFoundException("The model file is not found", path);
            }
            using (FileStream stream = File.OpenRead(path)) {
                return Parse(stream);
            }
        }

        private static PMXFormat Parse(Stream stream) {
            var reader = new BinaryReader(stream);
            var format = new PMXFormat();
            try {
                ParseHeader(reader, format);
                ParseVertices(reader, format);
                ParseFaces(reader, format);
                ParseTextures(reader, format);
                ParseMaterials(reader, format);
                ParseBones(reader, format);
                ParseMorphs(reader, format);
                ParseDisplayFrames(reader, format);
                ParseRigidBodies(reader, format);
                ParseRigidBodyJoints(reader, format);
            } finally {
                reader.Close();
                stream.Close();
            }
            return format;
        }

        private static void ParseHeader(BinaryReader reader, PMXFormat format) {
            byte[] magic = reader.ReadBytes(4);
            if (Encoding.ASCII.GetString(magic) != "PMX ") {
                throw new FormatException("The file is not a PMX file");
            }

            float version = reader.ReadSingle();
            if (version < 2.0f) {
                throw new NotSupportedException("PMX version 2.0 or higher is required");
            }

            format.Header = new PMXFormat.PMXHeader();

            // Number of flags (skip this for now)
            reader.ReadByte();

            format.Header.Encode = (PMXFormat.StringEncode)reader.ReadByte();
            format.Header.AdditionalUV = reader.ReadByte();
            format.Header.VertexIndexSize = (PMXFormat.IndexSize)reader.ReadByte();
            format.Header.TextureIndexSize = (PMXFormat.IndexSize)reader.ReadByte();
            format.Header.MaterialIndexSize = (PMXFormat.IndexSize)reader.ReadByte();
            format.Header.BoneIndexSize = (PMXFormat.IndexSize)reader.ReadByte();
            format.Header.MorphIndexSize = (PMXFormat.IndexSize)reader.ReadByte();
            format.Header.RigidbodyIndexSize = (PMXFormat.IndexSize)reader.ReadByte();

            // Read the model name
            format.Header.ModelName = ReadString(reader, format.Header.Encode);
            format.Header.ModelNameEnglish = ReadString(reader, format.Header.Encode);

            // Read the comment
            format.Header.Comment = ReadString(reader, format.Header.Encode);
            format.Header.CommentEnglish = ReadString(reader, format.Header.Encode);
        }

        private static void ParseVertices(BinaryReader reader, PMXFormat format) {
            uint nbVertices = reader.ReadUInt32();
            format.Vertices = new Vertex[nbVertices];

            for (uint i = 0; i < nbVertices; i++) {
                format.Vertices[i] = ReadVertex(reader, format);
            }
        }

        private static Vertex ReadVertex(BinaryReader reader, PMXFormat format) {
            var vertex = new Vertex {
                Position = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle()),
                Normal = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle()),
                UV = new Vector2(reader.ReadSingle(), reader.ReadSingle())
            };

            // Check if the vertex has additional UVs
            if (format.Header.AdditionalUV > 0) {
                // TODO: Read the additional UVs
                Debug.LogWarning("[PMX Parser] Additional UVs are not supported");
            }

            var weightType = (PMXFormat.WeightMethod)reader.ReadByte();
            switch (weightType) {
                case PMXFormat.WeightMethod.Bdef1:
                    vertex.BoneWeight = ReadBoneWeightBDEF1(reader, format);
                    break;
                case PMXFormat.WeightMethod.Bdef2:
                    vertex.BoneWeight = ReadBoneWeightBDEF2(reader, format);
                    break;
                case PMXFormat.WeightMethod.Qdef:
                case PMXFormat.WeightMethod.Bdef4:
                    vertex.BoneWeight = ReadBoneWeightBDEF4(reader, format);
                    break;
                case PMXFormat.WeightMethod.Sdef:
                    vertex.BoneWeight = ReadBoneWeightSDEF(reader, format);
                    break;
                default:
                    throw new FormatException("Unsupported weight type");
            }

            // Read the edge magnification
            vertex.EdgeMagnification = reader.ReadSingle();

            return vertex;
        }

        private static IBoneWeight ReadBoneWeightBDEF1(BinaryReader reader, PMXFormat format) {
            var boneWeight = new BDEF1Vertex {
                // Read the bone index
                BoneIndex1 = ReadUInt(reader, format.Header.BoneIndexSize)
            };
            return boneWeight;
        }

        private static IBoneWeight ReadBoneWeightBDEF2(BinaryReader reader, PMXFormat format) {
            var boneWeight = new BDEF2Vertex {
                // Read the bone indices
                BoneIndex1 = ReadUInt(reader, format.Header.BoneIndexSize),
                BoneIndex2 = ReadUInt(reader, format.Header.BoneIndexSize),

                // Read the bone weights
                BoneWeight1 = reader.ReadSingle()
            };
            return boneWeight;
        }

        private static IBoneWeight ReadBoneWeightBDEF4(BinaryReader reader, PMXFormat format) {
            var boneWeight = new BDEF4Vertex {
                // Read the bone indices
                BoneIndex1 = ReadUInt(reader, format.Header.BoneIndexSize),
                BoneIndex2 = ReadUInt(reader, format.Header.BoneIndexSize),
                BoneIndex3 = ReadUInt(reader, format.Header.BoneIndexSize),
                BoneIndex4 = ReadUInt(reader, format.Header.BoneIndexSize),

                // Read the bone weights
                BoneWeight1 = reader.ReadSingle(),
                BoneWeight2 = reader.ReadSingle(),
                BoneWeight3 = reader.ReadSingle(),
                BoneWeight4 = reader.ReadSingle()
            };
            return boneWeight;
        }

        private static IBoneWeight ReadBoneWeightSDEF(BinaryReader reader, PMXFormat format) {
            var boneWeight = new SDEFVertex {
                // Read the bone indices
                BoneIndex1 = ReadUInt(reader, format.Header.BoneIndexSize),
                BoneIndex2 = ReadUInt(reader, format.Header.BoneIndexSize),

                // Read the bone weights
                BoneWeight1 = reader.ReadSingle(),

                // Read the C value
                C = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle()),

                // Read the R0 value
                R0 = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle()),

                // Read the R1 value
                R1 = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle())
            };
            return boneWeight;
        }

        private static void ParseFaces(BinaryReader reader, PMXFormat format) {
            uint nbFaces = reader.ReadUInt32();

            // Divide by 3 because each face is composed of 3 vertices
            format.Faces = new Face[nbFaces / 3];

            for (uint i = 0; i < format.Faces.Length; i++) {
                var face = new Face {
                    // Read the vertex indices
                    VertexIndex1 = ReadUInt(reader, format.Header.VertexIndexSize),
                    VertexIndex2 = ReadUInt(reader, format.Header.VertexIndexSize),
                    VertexIndex3 = ReadUInt(reader, format.Header.VertexIndexSize)
                };
                format.Faces[i] = face;
            }
        }

        private static void ParseTextures(BinaryReader reader, PMXFormat format) {
            uint nbTextures = reader.ReadUInt32();
            format.Textures = new string[nbTextures];

            for (uint i = 0; i < nbTextures; i++) {
                string texture = ReadString(reader, format.Header.Encode);
                format.Textures[i] = texture;
            }
        }

        private static void ParseMaterials(BinaryReader reader, PMXFormat format) {
            uint nbMaterials = reader.ReadUInt32();
            format.Materials = new Material[nbMaterials];

            for (uint i = 0; i < nbMaterials; i++) {
                format.Materials[i] = ReadMaterial(reader, format);
            }
        }

        private static Material ReadMaterial(BinaryReader reader, PMXFormat format) {
            var material = new Material {
                // Read the material name
                Name = ReadString(reader, format.Header.Encode),
                // Read the material name (English)
                NameEnglish = ReadString(reader, format.Header.Encode),
                // Read the diffuse color
                DiffuseColor = new Color(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle()),
                // Read the specular color
                SpecularColor = new Color(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle()),
                // Read the specular power
                SpecularPower = reader.ReadSingle(),
                // Read the ambient color
                AmbientColor = new Color(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle()),
                // Read the flag
                Flags = reader.ReadByte(),
                // Read the edge color
                EdgeColor = new Color(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle()),
                // Read the edge size
                EdgeSize = reader.ReadSingle(),
                // Read the texture index
                TextureIndex = ReadUInt(reader, format.Header.TextureIndexSize),
                // Read the sphere texture index
                SphereTextureIndex = ReadUInt(reader, format.Header.TextureIndexSize),
                // Read the sphere mode
                SphereMode = reader.ReadByte(),
                // Read the shared toon flag
                SharedToonFlag = reader.ReadByte()
            };

            if (material.SharedToonFlag == 1) {
                // Read the toon texture index
                material.ToonTextureIndex = reader.ReadByte();
            } else {
                // Read the toon texture index
                material.ToonTextureIndex = reader.ReadByte();
            }

            // Read the memo
            material.Memo = ReadString(reader, format.Header.Encode);

            // Read the face count
            material.FaceCount = reader.ReadInt32();

            return material;
        }

        private static void ParseBones(BinaryReader reader, PMXFormat format) {
            uint nbBones = reader.ReadUInt32();
            format.Bones = new Bone[nbBones];

            for (uint i = 0; i < nbBones; i++) {
                format.Bones[i] = ReadBone(reader, format);
            }
        }

        private static Bone ReadBone(BinaryReader reader, PMXFormat format) {
            var bone = new Bone {
                // Read the bone name
                Name = ReadString(reader, format.Header.Encode),
                // Read the bone name (English)
                NameEnglish = ReadString(reader, format.Header.Encode),
                // Read the bone position
                Position = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle()),
                // Read the parent bone index
                ParentBoneIndex = ReadUInt(reader, format.Header.BoneIndexSize),
                // Read the transformation level
                TransformLevel = reader.ReadInt32(),
                // Read the bone flags
                Flag = (PMXFormat.BoneFlag) reader.ReadUInt16()
            };

            if ((PMXFormat.BoneFlag.Connection & bone.Flag) == 0) {
                bone.ConnectionPosition = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
            } else {
                bone.ConnectionBoneIndex = ReadUInt(reader, format.Header.BoneIndexSize);
            }

            if ((bone.Flag & (PMXFormat.BoneFlag.AddRotation | PMXFormat.BoneFlag.AddMove)) != 0) {
                bone.AdditionalParentBoneIndex = ReadUInt(reader, format.Header.BoneIndexSize);
                bone.AdditionalRate = reader.ReadSingle();
            }
            
            if ((bone.Flag & PMXFormat.BoneFlag.FixedAxis) != 0) {
                bone.AxisDirection = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
            }
            
            if ((bone.Flag & PMXFormat.BoneFlag.LocalAxis) != 0) {
                bone.LocalXAxisDirection = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                bone.LocalZAxisDirection = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
            }
            
            if ((bone.Flag & PMXFormat.BoneFlag.ExternalParentTransform) != 0) {
                bone.ExternalParentBoneIndex = reader.ReadUInt32();
            }
            
            if ((bone.Flag & PMXFormat.BoneFlag.IK) != 0) {
                bone.IK = ParseIK(reader, format);
            }

            return bone;
        }
        
        private static IK ParseIK(BinaryReader reader, PMXFormat format) {
            var ik = new IK {
                BoneIndex = ReadUInt(reader, format.Header.BoneIndexSize),
                Iteration = reader.ReadUInt32(),
                AngleLimit = reader.ReadSingle(),
            };
            
            uint nbLinks = reader.ReadUInt32();
            ik.Links = new Link[nbLinks];
            for (uint i = 0; i < nbLinks; i++) {
                ik.Links[i] = ParseLink(reader, format);
            }
            return ik;
        }
        
        private static Link ParseLink(BinaryReader reader, PMXFormat format) {
            var link = new Link {
                // Read the bone index
                BoneIndex = ReadUInt(reader, format.Header.BoneIndexSize),
                // Read the angle limit
                AngleLimit = reader.ReadByte()
            };
            
            if (link.AngleLimit == 1) {
                link.LowerLimit = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                link.UpperLimit = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
            }
            return link;
        }
        
        private static void ParseMorphs(BinaryReader reader, PMXFormat format) {
            uint nbMorphs = reader.ReadUInt32();
            format.Morphs = new Morph[nbMorphs];

            for (uint i = 0; i < nbMorphs; i++) {
                format.Morphs[i] = ReadMorph(reader, format);
            }
        }
        
        private static Morph ReadMorph(BinaryReader reader, PMXFormat format) {
            var morph = new Morph {
                // Read the morph name
                Name = ReadString(reader, format.Header.Encode),
                // Read the morph name (English)
                NameEnglish = ReadString(reader, format.Header.Encode),
                // Read the morph panel (category)
                Panel = (PMXFormat.MorphPanel) reader.ReadByte(),
                // Read the morph type
                Type = (PMXFormat.MorphType) reader.ReadByte(),
            };
            
            uint nbMorphOffset = reader.ReadUInt32();
            morph.Offsets = new IMorphOffset[nbMorphOffset];
            for (uint i = 0; i < nbMorphOffset; i++) {
                switch (morph.Type) {
                    case PMXFormat.MorphType.Group:
                    case PMXFormat.MorphType.Flip:
                        morph.Offsets[i] = ReadGroupMorphOffset(reader, format);
                        break;
                    case PMXFormat.MorphType.Vertex:
                        morph.Offsets[i] = ReadVertexMorphOffset(reader, format);
                        break;
                    case PMXFormat.MorphType.Bone:
                        morph.Offsets[i] = ReadBoneMorphOffset(reader, format);
                        break;
                    case PMXFormat.MorphType.UV:
                    case PMXFormat.MorphType.AdditionalUV1:
                    case PMXFormat.MorphType.AdditionalUV2:
                    case PMXFormat.MorphType.AdditionalUV3:
                    case PMXFormat.MorphType.AdditionalUV4:
                        morph.Offsets[i] = ReadUVMorphOffset(reader, format);
                        break;
                    case PMXFormat.MorphType.Material:
                        morph.Offsets[i] = ReadMaterialMorphOffset(reader, format);
                        break;
                    case PMXFormat.MorphType.Impulse:
                        morph.Offsets[i] = ReadImpulseMorphOffset(reader, format);
                        break;
                    default:
                        throw new FormatException("Unknown morph type");
                }
            }

            return morph;
        }
        
        private static GroupMorphOffset ReadGroupMorphOffset(BinaryReader reader, PMXFormat format) {
            var offset = new GroupMorphOffset {
                MorphIndex = ReadUInt(reader, format.Header.MorphIndexSize),
                MorphRate = reader.ReadSingle()
            };
            return offset;
        }
        
        private static VertexMorphOffset ReadVertexMorphOffset(BinaryReader reader, PMXFormat format) {
            var offset = new VertexMorphOffset {
                VertexIndex = ReadUInt(reader, format.Header.VertexIndexSize),
                PositionOffset = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle()),
            };
            return offset;
        }
        
        private static BoneMorphOffset ReadBoneMorphOffset(BinaryReader reader, PMXFormat format) {
            var offset = new BoneMorphOffset {
                BoneIndex = ReadUInt(reader, format.Header.BoneIndexSize),
                Translation = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle()),
                Rotation = new Quaternion(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle()),
            };
            return offset;
        }
        
        private static UVMorphOffset ReadUVMorphOffset(BinaryReader reader, PMXFormat format) {
            var offset = new UVMorphOffset {
                VertexIndex = ReadUInt(reader, format.Header.VertexIndexSize),
                UVOffset = new Vector4(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle()),
            };
            return offset;
        }
        
        private static MaterialMorphOffset ReadMaterialMorphOffset(BinaryReader reader, PMXFormat format) {
            var offset = new MaterialMorphOffset {
                MaterialIndex = ReadUInt(reader, format.Header.MaterialIndexSize),
                OffsetMethod = (OffsetMethod) reader.ReadByte(),
                Diffuse = new Color(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle()),
                Specular = new Color(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle()),
                SpecularPower = reader.ReadSingle(),
                Ambient = new Color(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle()),
                EdgeColor = new Color(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle()),
                EdgeSize = reader.ReadSingle(),
                TextureCoefficient = new Color(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle()),
                SphereTextureCoefficient = new Color(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle()),
                ToonTextureCoefficient = new Color(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle()),
            };
            return offset;
        }
        
        private static ImpulseMorphOffset ReadImpulseMorphOffset(BinaryReader reader, PMXFormat format) {
            var offset = new ImpulseMorphOffset {
                RigidBodyIndex = ReadUInt(reader, format.Header.MorphIndexSize),
                LocalFlag = reader.ReadByte(),
                Velocity = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle()),
                RotationTorque = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle()),
            };
            return offset;
        }
        
        private static void ParseDisplayFrames(BinaryReader reader, PMXFormat format) {
            uint nbDisplayFrame = reader.ReadUInt32();
            format.DisplayFrames = new DisplayFrame[nbDisplayFrame];
            for (uint i = 0; i < nbDisplayFrame; i++) {
                format.DisplayFrames[i] = ReadDisplayFrame(reader, format);
            }
        }
        
        private static DisplayFrame ReadDisplayFrame(BinaryReader reader, PMXFormat format) {
            var frame = new DisplayFrame {
                Name = ReadString(reader, format.Header.Encode),
                NameEnglish = ReadString(reader, format.Header.Encode),
                SpecialFlag = reader.ReadByte(),
            };
            
            uint nbElement = reader.ReadUInt32();
            frame.Elements = new DisplayFrameElement[nbElement];
            for (uint i = 0; i < nbElement; i++) {
                frame.Elements[i] = ReadDisplayFrameElement(reader, format);
            }
            return frame;
        }
        
        private static DisplayFrameElement ReadDisplayFrameElement(BinaryReader reader, PMXFormat format) {
            var element = new DisplayFrameElement {
                Type = (PMXFormat.DisplayFrameElementType) reader.ReadByte(),
            };
            
            // Determine the index size 
            PMXFormat.IndexSize indexSize = element.Type switch {
                PMXFormat.DisplayFrameElementType.Bone => format.Header.BoneIndexSize,
                PMXFormat.DisplayFrameElementType.Morph => format.Header.MorphIndexSize,
                _ => throw new InvalidOperationException()
            };

            element.Index = ReadUInt(reader, indexSize);
            return element;
        }
        
        private static void ParseRigidBodies(BinaryReader reader, PMXFormat format) {
            uint nbRigidBody = reader.ReadUInt32();
            format.RigidBodies = new RigidBody[nbRigidBody];
            for (uint i = 0; i < nbRigidBody; i++) {
                format.RigidBodies[i] = ReadRigidBody(reader, format);
            }
        }
        
        private static RigidBody ReadRigidBody(BinaryReader reader, PMXFormat format) {
            var body = new RigidBody {
                Name = ReadString(reader, format.Header.Encode),
                NameEnglish = ReadString(reader, format.Header.Encode),
                BoneIndex = ReadUInt(reader, format.Header.BoneIndexSize),
                GroupIndex = reader.ReadByte(),
                IgnoreCollisionGroup = reader.ReadUInt16(),
                ShapeType = (PMXFormat.RigidBodyShapeType) reader.ReadByte(),
                ShapeSize = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle()),
                CollisionPosition = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle()),
                CollisionRotation = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle()),
                Weight = reader.ReadSingle(),
                PositionDamping = reader.ReadSingle(),
                RotationDamping = reader.ReadSingle(),
                Recoil = reader.ReadSingle(),
                Friction = reader.ReadSingle(),
                OperationType = (PMXFormat.RigidBodyOperationType) reader.ReadByte()
            };
            return body;
        }
        
        private static void ParseRigidBodyJoints(BinaryReader reader, PMXFormat format) {
            uint nbJoint = reader.ReadUInt32();
            format.Joints = new Joint[nbJoint];
            for (uint i = 0; i < nbJoint; i++) {
                format.Joints[i] = ReadJoint(reader, format);
            }
        }
        
        private static Joint ReadJoint(BinaryReader reader, PMXFormat format) {
            var joint = new Joint {
                Name = ReadString(reader, format.Header.Encode),
                NameEnglish = ReadString(reader, format.Header.Encode)
            };
            
            joint.OperationType = (PMXFormat.JointOperationType) reader.ReadByte();
            switch (joint.OperationType) {
                case PMXFormat.JointOperationType.Spring6DOF:
                    joint.RigidBodyAIndex = ReadUInt(reader, format.Header.RigidbodyIndexSize);
                    joint.RigidBodyBIndex = ReadUInt(reader, format.Header.RigidbodyIndexSize);
                    joint.Position = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                    joint.Rotation = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                    joint.ConstrainPositionLower = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                    joint.ConstrainPositionUpper = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                    joint.ConstrainRotationLower = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                    joint.ConstrainRotationUpper = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                    joint.SpringPosition = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                    joint.SpringRotation = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                    break;
                default:
                    throw new FormatException("Unsupported joint type.");
            }
            return joint;
        }

        private static string ReadString(BinaryReader reader, PMXFormat.StringEncode encode) {
            int length = reader.ReadInt32();
            byte[] bytes = reader.ReadBytes(length);
            return encode switch {
                PMXFormat.StringEncode.UTF16Le => Encoding.Unicode.GetString(bytes),
                PMXFormat.StringEncode.UTF8 => Encoding.UTF8.GetString(bytes),
                _ => throw new InvalidOperationException()
            };
        }

        private static uint ReadUInt(BinaryReader reader, PMXFormat.IndexSize indexSize) {
            return indexSize switch {
                PMXFormat.IndexSize.Size1 => reader.ReadByte(),
                PMXFormat.IndexSize.Size2 => reader.ReadUInt16(),
                PMXFormat.IndexSize.Size4 => reader.ReadUInt32(),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}
