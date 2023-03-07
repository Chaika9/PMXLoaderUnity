using UnityEngine;

namespace LibMMD.Unity3D {
    public class BasicDemoController : MonoBehaviour {
        [SerializeField] private string modelPath;

        private void Start() {
            if (string.IsNullOrEmpty(modelPath)) {
                Debug.LogError("Please fill your model path");
                return;
            }
            
            GameObject mmdGameObject = MmdGameObject.CreateGameObject("MmdGameObject");
            var mmdGameObjectComponent = mmdGameObject.GetComponent<MmdGameObject>();
            
            mmdGameObjectComponent.LoadModel(modelPath);
        }
    }
}
