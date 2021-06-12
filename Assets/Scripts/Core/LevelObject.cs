using UnityEngine;

namespace LW.Core
{
    public class LevelObject : MonoBehaviour
    {
        public static LevelObject Instance;

        private void Start()
        {
            Instance = this;
        }
        void Update()
        {
            transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y - 1, Camera.main.transform.position.z);
            transform.rotation = new Quaternion(0, Camera.main.transform.rotation.y, 0, Camera.main.transform.rotation.w);
        }
    }
}


