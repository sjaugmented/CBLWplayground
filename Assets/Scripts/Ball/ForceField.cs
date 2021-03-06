using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LW.Ball
{
    public class ForceField : MonoBehaviour
    {
        public bool Caught { get; set; }
        private void OnTriggerEnter(Collider other)
        {
            Caught = true;
        }

        private void Update()
        {
            GetComponent<CapsuleCollider>().enabled = !Caught;
        }
    }
}

