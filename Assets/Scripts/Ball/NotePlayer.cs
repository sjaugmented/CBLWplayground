using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LW.Ball
{
    public class NotePlayer : MonoBehaviourPunCallbacks
    {
        [SerializeField] List<AudioClip> notes;

        Ball ball;
        AudioSource audio;
        void Start()
        {
            if (!photonView.IsMine)
            {
                return;
            }
            ball = GetComponent<Ball>();
            audio = GetComponent<AudioSource>();
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        public void PlayNote(int index)
        {
            if (!photonView.IsMine)
            {
                return;
            }

            //if (audio.isPlaying) { return; }
            audio.PlayOneShot(notes[index]);
        }
    }
}

