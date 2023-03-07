using LibMMD.Model;
using LibMMD.Reader;
using UnityEngine;

namespace LibMMD.Unity3D {
    [RequireComponent(typeof(MeshFilter), typeof(SkinnedMeshRenderer))]
    public class MmdGameObject : MonoBehaviour {
        private MmdModel _model;
        
        public static GameObject CreateGameObject(string name) {
            var go = new GameObject(name);
            go.AddComponent<MmdGameObject>();
            var skinnedMeshRenderer = go.GetComponent<SkinnedMeshRenderer>();
            skinnedMeshRenderer.quality = SkinQuality.Bone4;
            return go;
        }
        
        public void LoadModel(string path) {
            Debug.LogFormat("Start load model {0}", path);
            try {
                _model = ModelReader.LoadModel(path);
            } catch (System.Exception e) {
                Debug.LogErrorFormat("Load model {0} failed: {1}", path, e);
            }
            
            Debug.LogFormat("Load model finished {0}", path);
        }
    }
}
