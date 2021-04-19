using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LW.Ball
{
    public class ForceField : MonoBehaviour
    {
        public bool Caught { get; set; }

        BallDirector director;

        private void Start()
        {
            director = GameObject.FindGameObjectWithTag("Director").GetComponent<BallDirector>();
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

