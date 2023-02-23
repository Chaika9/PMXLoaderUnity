using System.Collections.Generic;
using MMD.PMX;
using UnityEngine;

namespace MMD {
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class PMXRenderer : MonoBehaviour {
        [Header("Model")]
        [SerializeField] private string modelPath;
        [SerializeField] private string textureFolderPath;
        
        [Header("Materials")]
        [SerializeField] private Shader shader;
        
        [Header("Debug")]
        [SerializeField] private bool showBones;
        [SerializeField] private bool showBoneNames;
        [SerializeField] private bool showRigidBodiesBones;
        [SerializeField] private bool showJoints;
        [SerializeField] private bool showJointNames;
        
        private MeshFilter meshFilter;
        private MeshRenderer meshRenderer;

        public PMXModel Model { get; }

        public PMXRenderer() {
            Model = new PMXModel();
        }
        
        private void Awake() {
            meshFilter = GetComponent<MeshFilter>();
            meshRenderer = GetComponent<MeshRenderer>();

            if (shader == null) {
                shader = Shader.Find("Standard");
            }
        }

        private void Start() {
            Model.LoadModel(modelPath, textureFolderPath, transform.localScale);

            var mesh = new Mesh {
                vertices = Model.Vertices,
                normals = Model.Normals,
                uv = Model.UV,
                subMeshCount = Model.Materials.Length
            };

            for (int i = 0; i < Model.Materials.Length; i++) {
                mesh.SetTriangles(Model.Materials[i].Triangles, i);
            }

            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            mesh.RecalculateTangents();

            meshFilter.mesh = mesh;

            GenerateUnityMaterials();
        }
        
        private void GenerateUnityMaterials() {
            var materials = new List<UnityEngine.Material>();
            foreach (ModelMaterial modelMaterial in Model.Materials) {
                var material = new UnityEngine.Material(shader) {
                    name = modelMaterial.Name
                };
                
                if (modelMaterial.TextureIndex < Model.Textures.Length) {
                    material.SetTexture(Shader.PropertyToID("_MainTex"), Model.Textures[modelMaterial.TextureIndex]);
                }
                
                // Disable metallic and smoothness
                material.SetFloat(Shader.PropertyToID("_Metallic"), 0);
                material.SetFloat(Shader.PropertyToID("_Glossiness"), 0);
                
                materials.Add(material);
            }

            meshRenderer.materials = materials.ToArray();
        }
        
        #if UNITY_EDITOR
        
        private void OnDrawGizmos() {
            if (!Model.IsLoaded) {
                return;
            }

            if (showBones) {
                DrawBones();
            }
            if (showJoints) {
                DrawJoints();
            }
        }
        
        private void DrawBones() {
            // Calculate radius in function of scale
            float radius = 0.05f * transform.localScale.x;
            
            // Draw bones
            foreach (Bone bone in Model.Bones) {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(bone.Position, radius);
            }
                
            // Draw bone connections
            foreach (Bone bone in Model.Bones) {
                Gizmos.color = Color.green;
                    
                // Check if bone is out of bounds
                if (bone.ParentBoneIndex < Model.Bones.Length) {
                    Gizmos.DrawLine(Model.Bones[bone.ParentBoneIndex].Position, bone.Position);
                }
            }
            
            if (showBoneNames) {
                // Draw bone names
                foreach (Bone bone in Model.Bones) {
                    UnityEditor.Handles.Label(bone.Position, bone.Name);
                }
            }
            
            if (showRigidBodiesBones) {
                // Link rigid bodies to bones
                foreach (RigidBody rigidBody in Model.RigidBodies) {
                    Gizmos.color = Color.yellow;
                    if (rigidBody.BoneIndex < Model.Bones.Length) {
                        Gizmos.DrawSphere(Model.Bones[rigidBody.BoneIndex].Position, radius);
                    }
                }
            }
        }

        private void DrawJoints() {
            if (!showJoints) {
                return;
            }
            
            foreach (PMX.Joint joint in Model.Joints) {
                Gizmos.color = Color.blue;
                Gizmos.DrawSphere(joint.Position, 0.05f);
            }
            
            if (showJointNames) {
                // Show joint names
                foreach (PMX.Joint joint in Model.Joints) {
                    UnityEditor.Handles.Label(joint.Position, joint.Name);
                }
            }
        }

        #endif
    }
}
