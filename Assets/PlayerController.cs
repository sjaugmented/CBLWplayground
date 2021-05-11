using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LW.Ball
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] bool controller = false;
        public bool Controller { get { return controller; } set { controller = value; } }

        void Start()
        {
            Debug.Log("Player live");
        }

        void Update()
        {

        }
    }
}

