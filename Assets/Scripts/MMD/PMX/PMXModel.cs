using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace MMD.PMX {
    public class PMXModel {
        private PMXFormat pmx;
        
        public string ModelName { get; private set; }
        public string ModelNameEnglish { get; private set; }
        
        public Vector3[] Vertices { get; private set; }
        public Vector3[] Normals { get; private set; }
        public Vector2[] UV { get; private set; }
        public Texture2D[] Textures { get; private set; }
        public ModelMaterial[] Materials { get; private set; }
        
        // Test
        public Bone[] Bones { get; private set; }
        public Morph[] Morphs { get; private set; }
        public RigidBody[] RigidBodies { get; private set; }
        public Joint[] Joints { get; private set; }
        
        public bool IsLoaded { get; private set; }

        public void LoadModel(string modelPath, string textureFolderPath, Vector3 scale) {
            IsLoaded = false;
            pmx = PMXParser.Load(modelPath);
            
            ModelName = pmx.Header.ModelName;
            ModelNameEnglish = pmx.Header.ModelNameEnglish;

            Vertices = pmx.Vertices.Select(v => v.Position).ToArray();
            Normals = pmx.Vertices.Select(v => v.Normal).ToArray();
            UV = pmx.Vertices
                .Select(v => v.UV)
                // Flip UVs (Unity uses a different coordinate system)
                .Select(v => new Vector2(v.x, 1 - v.y))
                .ToArray();
            
            // Test
            Bones = pmx.Bones;
            Morphs = pmx.Morphs;
            RigidBodies = pmx.RigidBodies;
            Joints = pmx.Joints;
            
            // Check Bones out of bounds
            foreach (Bone t in Bones) {
                if (t.ParentBoneIndex >= Bones.Length) {
                    Debug.LogWarning("[PMX Parser] Bone parent index out of bounds: " + t.ParentBoneIndex);
                }
            }

            // Scale bones by model scale
            foreach (Bone t in Bones) {
                t.Position = Vector3.Scale(t.Position, scale);
            }
            
            // Scale Joints by model scale
            foreach (Joint t in Joints) {
                t.Position = Vector3.Scale(t.Position, scale);
            }

            // Check if the texture folder path is null or empty. If it is, set it to an empty string.
            if (string.IsNullOrEmpty(textureFolderPath)) {
                textureFolderPath = "";
            }
            
            LoadTextures(textureFolderPath);
            LoadMaterials();
            
            IsLoaded = true;
        }

        private void LoadTextures(string textureFolderPath) {
            Textures = new Texture2D[pmx.Textures.Length];

            int textureIndex = 0;
            foreach (string pmxTexturePath in pmx.Textures) {
                string texturePath = Path.Combine(textureFolderPath, pmxTexturePath);
                
                if (texturePath.EndsWith(".tga")) {
                    texturePath = texturePath.Replace(".tga", ".png");
                }

                if (!File.Exists(texturePath)) {
                    Debug.LogWarning("[PMX Parser] Texture not found: " + texturePath);
                    continue;
                }

                var texture = new Texture2D(1, 1);
                texture.LoadImage(File.ReadAllBytes(texturePath));

                Textures[textureIndex] = texture;
                textureIndex++;
            }
        }

        private void LoadMaterials() {
            Materials = new ModelMaterial[pmx.Materials.Length];

            int faceIndex = 0;
            int materialIndex = 0;
            foreach (Material material in pmx.Materials) {
                var modelMaterial = new ModelMaterial {
                    Name = material.Name,
                    TextureIndex = material.TextureIndex
                };

                var faces = new List<uint>();

                // Load faces from VertexIndex1, VertexIndex2 and VertexIndex3
                for (int j = 0; j < material.FaceCount / 3; j++) {
                    Face face = pmx.Faces[faceIndex];

                    // Check if the indices are out of bounds.
                    if (face.VertexIndex1 >= pmx.Vertices.Length) {
                        Debug.LogWarning("[PMX Parser] Vertex index 1 out of bounds: " + face.VertexIndex1);
                        continue;
                    }

                    if (face.VertexIndex2 >= pmx.Vertices.Length) {
                        Debug.LogWarning("[PMX Parser] Vertex index 2 out of bounds: " + face.VertexIndex2);
                        continue;
                    }

                    if (face.VertexIndex3 >= pmx.Vertices.Length) {
                        Debug.LogWarning("[PMX Parser] Vertex index 3 out of bounds: " + face.VertexIndex3);
                        continue;
                    }

                    faces.Add(face.VertexIndex1);
                    faces.Add(face.VertexIndex2);
                    faces.Add(face.VertexIndex3);
                    faceIndex++;
                }

                modelMaterial.Triangles = faces.Select(v => (int)v).ToArray();

                Materials[materialIndex] = modelMaterial;
                materialIndex++;
            }
        }
    }
}
