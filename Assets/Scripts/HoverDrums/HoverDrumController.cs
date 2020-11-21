using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LW.HoverDrums
{
    public class HoverDrumController : MonoBehaviour
    {
        [SerializeField] Color color;
        void Start()
        {
        
        }

        void Update()
        {
            GetComponentInChildren<Renderer>().material.color = color;
        }
    }

}
