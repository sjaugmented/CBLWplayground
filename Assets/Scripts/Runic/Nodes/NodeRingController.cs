using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LW.Runic
{
    public class NodeRingController : MonoBehaviour
    {
        [SerializeField] float duration = 1;

        void Update()
        {
            transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);
        }
    }
}
