using System.Collections;
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
            Color.RGBToHSV(ball.NoteColor, out float h, out float s, out float v);

            if (s > 0)
            {
                s -= 0.01f;
            }
            
            if (v > 0)
            {
                v -= 0.001f;
            }

            foreach (Material mat in shellMats)
            {
                mat.color = ball.State == BallState.Still ? Color.HSVToRGB(h, s, 0.2f) : Color.HSVToRGB(h, s, 1);
                //mat.color = Color.HSVToRGB(h, 0.2f, 0.2f);
            }

            foreach (Material mat in ringMats)
            {
                mat.color = ball.State == BallState.Still ? Color.HSVToRGB(h, 0.2f, v) : ball.NoteColor;
                //mat.color = ball.NoteColor;
                if (ball.State == BallState.Active)
                {
                    mat.EnableKeyword("_EMISSION");
                    mat.SetColor("_EmissionColor", ball.NoteColor);
                }
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

