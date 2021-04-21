using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LW.Ball
{
    public class BallPainter : MonoBehaviour
    {
        [SerializeField] List<GameObject> shells, rings;
        float h, s, v;

        List<Material> shellMats = new List<Material>();
        List<Material> ringMats = new List<Material>();

        Ball ball;
        Light light;

        void Start()
        {
            ball = GetComponent<Ball>();
            light = GetComponentInChildren<Light>();

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
            if (ball.CoreActive)
            {
                SetColor();
            }

            if (s > 0)
            {
                s -= 0.01f;
            }
            
            if (v > 0)
            {
                v -= 0.01f;
            }

            if (light.intensity > 0)
            {
                light.intensity -= 0.1f;
            }

            foreach (Material mat in shellMats)
            {
                if (ball.Handedness == Core.Hands.right)
                {
                    mat.color = ball.State == BallState.Still ? Color.HSVToRGB(0, 0.5f, 0.2f) : Color.HSVToRGB(0, 0.5f, 1);
                } 
                else
                {
                    mat.color = ball.State == BallState.Still ? Color.HSVToRGB(0.5f, 0.5f, 0.2f) : Color.HSVToRGB(0.5f, 0.5f, 1);

                }
            }

            foreach (Material mat in ringMats)
            {
                //mat.color = Color.HSVToRGB(h, s, v);
                //mat.EnableKeyword("_EMISSION");
                mat.SetColor("_EmissionColor", Color.HSVToRGB(h, s, v));
                mat.color = ball.NoteColor;
                //if (ball.State == BallState.Active)
                //{
                //    mat.EnableKeyword("_EMISSION");
                //    mat.SetColor("_EmissionColor", Color.HSVToRGB(h, s, v));
                //}
            }

            light.enabled = true;
            light.color = Color.HSVToRGB(h, s, 1);
        }

        public void SetColor()
        {
            Color.RGBToHSV(ball.NoteColor, out h, out s, out v);
            light.intensity = 10;
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

