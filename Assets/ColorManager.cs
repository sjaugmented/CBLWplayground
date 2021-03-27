using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LW.HSL
{
    public class ColorManager : MonoBehaviour
    {
        public float Hue { get; set; }
        void Start()
        {
            Hue = Mathf.Infinity;
        }

        void Update()
        {

        }
    }
}

