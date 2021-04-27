using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LW.Ball
{
    public class NotePlayer : MonoBehaviour
    {
        [SerializeField] List<AudioClip> notes;

        Ball ball;
        AudioSource audio;
        void Start()
        {
            ball = GetComponent<Ball>();
            audio = GetComponent<AudioSource>();
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        public void PlayNote(int index)
        {
            //if (audio.isPlaying) { return; }

            audio.PlayOneShot(notes[index]);
        }
    }
}

