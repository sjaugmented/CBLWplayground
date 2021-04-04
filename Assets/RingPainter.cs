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

        Ball ball;

        void Start()
        {
            ball = GetComponent<Ball>();
            if (rings.Count > 0)
            {
                foreach(GameObject ring in rings)
                {
                    ringMats.Add(ring.GetComponent<Material>());
                }
            }

            RingColor = Color.HSVToRGB(0, 0, 1);
        }

        void Update()
        {
            foreach(Material mat in ringMats)
            {
                mat.color = ball.State == BallState.Still ? RingColor : Color.HSVToRGB(0, 0, 1);
            }
        }
    }
}

// color values
// red = 0 //
// light red = 0, .58, 1
// yellow = .15 //
// green = .29 //
// cyan = .5 //
// blue = .66 //
// baby blue = .66, .58, 1//
// purple = .78
// pink = 0.86, 0.58, 1//
// magneta = .89 //

