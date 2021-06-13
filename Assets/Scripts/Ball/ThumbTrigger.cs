using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LW.Ball
{
    public class ThumbTrigger : MonoBehaviourPunCallbacks
    {
        [SerializeField] bool leftThumb = false;

        public bool Triggered { get; set; }

        private void OnTriggerEnter(Collider other)
        {
            if (!photonView.IsMine)
            {
                return;
            }
            if (leftThumb)
            {
                if (other.CompareTag("Right Index"))
                {
                    Triggered = true;
                }
            }
            else
            {
                if (other.CompareTag("Left Index"))
                {
                    Triggered = true;
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            Triggered = false;
        }
    }
}

