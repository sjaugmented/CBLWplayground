using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LW.Ball
{
    public class ForceField : MonoBehaviour
    {
        public bool Caught { get; set; }

        BallCaster caster;

        private void Start()
        {
            caster = GameObject.FindGameObjectWithTag("Caster").GetComponent<BallCaster>();
        }

        //private void Update()
        //{
        //    GetComponent<CapsuleCollider>().enabled = !Caught;
        //    Debug.Log("Caught = " + Caught);
        //}
        
        //private void OnTriggerEnter(Collider other)
        //{
        //    Caught = true;
        //}

    }
}

