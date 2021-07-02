using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LW.Ball
{
    public class BallPainter : MonoBehaviourPunCallbacks, IPunObservable
    {
        [SerializeField] List<GameObject> shells, rings;
        float h, s, v;

        List<Material> shellMats = new List<Material>();
        List<Material> ringMats = new List<Material>();

        Ball ball;
        Light light;

        #region Photon

        float r, g, b, a;

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                // We own this ball so send the others our data
                stream.SendNext(h);
                stream.SendNext(s);
                stream.SendNext(v);
                stream.SendNext(r);
                stream.SendNext(g);
                stream.SendNext(b);
                stream.SendNext(a);
            }
            else
            {
                // Network ball, receive data
                h = (float)stream.ReceiveNext();
                s = (float)stream.ReceiveNext();
                v = (float)stream.ReceiveNext();
                r = (float)stream.ReceiveNext();
                g = (float)stream.ReceiveNext();
                b = (float)stream.ReceiveNext();
                a = (float)stream.ReceiveNext();
            }
        }

        #endregion

        void Start()
        {
            ball = GetComponent<Ball>();

            if (GetComponentInChildren<Light>())
            {
                light = GetComponentInChildren<Light>();
            }

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
            if (photonView.IsMine)
            {
                r = ball.NoteColor.r;
                g = ball.NoteColor.r;
                b = ball.NoteColor.b;
                a = ball.NoteColor.a;
            }

            if (ball.CoreActive)
            {
                //Debug.Log("CoreActive");
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

            if (light != null)
            {
                if (light.intensity > 0)
                {
                    //Debug.Log("dimming light");
                    light.intensity -= 0.1f;
                }

                //Debug.Log("setting light color");
                light.color = Color.HSVToRGB(h, s, 1);
            }

            if (shellMats.Count > 0)
            {
                foreach (Material mat in shellMats)
                {
                    if (ball.Handedness == Core.Hands.right)
                    {
                        mat.color = ball.State == BallState.Still ? Color.HSVToRGB(0, 0.5f, 1f) : Color.HSVToRGB(0, 0.5f, 0.2f);
                    } 
                    else
                    {
                        mat.color = ball.State == BallState.Still ? Color.HSVToRGB(0.5f, 0.5f, 1f) : Color.HSVToRGB(0.5f, 0.5f, 0.2f);

                    }
                }
            }

            if (ringMats.Count > 0)
            {
                foreach (Material mat in ringMats)
                {
                    mat.SetColor("_EmissionColor", Color.HSVToRGB(h, s, v));
                    mat.color = new Color(r, g, b, a);
                }
            }

            
        }

        public void SetColor()
        {
            if (photonView.IsMine)
            {
                Color.RGBToHSV(ball.NoteColor, out h, out s, out v);
            }
            //Debug.Log("Light up 10!");
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

