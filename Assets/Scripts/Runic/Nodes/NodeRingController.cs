using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LW.Runic
{
    public class NodeRingController : MonoBehaviour
    {
        [SerializeField] float duration = 1f;
        float timer = Mathf.Infinity;

        void Update()
        {
            timer += Time.deltaTime;
            transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);
            if (timer > duration) DeactivateNodes();
        }

        public void ActivateNodes()
		{
            gameObject.SetActive(true);
            timer = 0;
		}

        public void DeactivateNodes()
		{
            gameObject.SetActive(false);
		}
    }
}
