using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LW.Runic
{
    public class NodeRingController : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            transform.LookAt(Camera.main.transform.position);
        }
    }
}
