using System.Collections.Generic;
using UnityEngine;

namespace LW.Ball
{
    public class BallPainter : MonoBehaviour
    {
        [SerializeField] List<GameObject> shells, rings;

        List<Material> shellMats = new List<Material>();
        List<Material> ringMats = new List<Material>();

        Ball ball;

        void Start()
        {
            ball = GetComponent<Ball>();
            if (shells.Count > 0)
            {
                foreach (GameObject shell in shells)
                {
                    shellMats.Add(shell.GetComponent<Renderer>().material);
                }
            }

            if (rings.Count > 0)
            {
                foreach (GameObject ring in rings)
                {
                    ringMats.Add(ring.GetComponent<Renderer>().material);
                }
            }
        }

        void Update()
        {
            foreach (Material mat in shellMats)
            {
                Color.RGBToHSV(ball.NoteColor, out float h, out _, out float v);
                mat.color = Color.HSVToRGB(h, 0.2f, 0.2f);
            }

            foreach (Material mat in ringMats)
            {
                mat.color = ball.State == BallState.Active ? Color.HSVToRGB(0, 0, 1) : ball.NoteColor;
                mat.SetColor("_EmissionColor", ball.NoteColor);
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

