using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LW.Ball
{
    public class RingPainter : MonoBehaviour
    {
        [SerializeField] List<GameObject> rings;
        public Color RingColor { get; set; }

        List<Material> ringMats = new List<Material>();

        void Start()
        {
            if (rings.Count > 0)
            {
                foreach(GameObject ring in rings)
                {
                    ringMats.Add(ring.GetComponent<Material>());
                }
            }

            RingColor = Color.HSVToRGB(0, 1, 0.8f);
        }

        void Update()
        {
            foreach(Material mat in ringMats)
            {
                mat.color = RingColor;
            }
        }
    }
}

// color values
// red = 0
// yellow = .15
// green = .29
// cyan = .5
// blue = .66, .58, 1
// baby blue = .66, 
// purple = .78
// pink = 0.86, 0.58, 1
// magneta = .89

