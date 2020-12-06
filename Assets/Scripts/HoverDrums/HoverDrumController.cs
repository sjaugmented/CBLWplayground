using System.Collections;
using UnityEngine;
using LW.Core;
using static UnityEngine.ParticleSystem;

namespace LW.HoverDrums
{
    public class HoverDrumController : MonoBehaviour
    {
        [SerializeField] AudioClip castFX;
        [SerializeField] AudioClip touchFX;
        [SerializeField] ParticleSystem particles;

        public float force = 1;
        public string address1;
        public string address2;
        
        [Range(0, 1)] public HSV color;
        bool isTouched = false;

        Renderer renderer;
        HandTracking handtracking;
        EmissionModule emission;

        void Start()
        {
            renderer = GetComponentInChildren<Renderer>();
            handtracking = GameObject.FindGameObjectWithTag("Handtracking").GetComponent<HandTracking>();
            GetComponent<Rigidbody>().AddForce(transform.forward * force);

            GetComponent<AudioSource>().PlayOneShot(castFX);

            GameObject.FindGameObjectWithTag("OSC").GetComponent<OSC>().SetAddressHandler(address1 + "/receive", OnReceiveOSC);

            emission = particles.emission;
            emission.enabled = false;
            //particles.main.startColor = Color.HSVToRGB(color.Hue, color.Sat, color.Val);
        }

        void Update()
        {
            if (isTouched)
            {
                renderer.material.color = Color.clear;
            }
            else renderer.material.color = Color.HSVToRGB(color.Hue, color.Sat, color.Val);
        }

        public void SetDrumColor(HSV colorValue)
        {
            color = colorValue;
        }

        public void SetDrumAddress(int drumNum)
        {
            address1 = transform.GetChild(0).name + drumNum + "a".ToString();
            address2 = transform.GetChild(0).name + drumNum + "b".ToString();
        }

        public void Touched()
        {
            isTouched = true;
            GetComponent<AudioSource>().PlayOneShot(touchFX);
            // if touched with one finger
            if (!handtracking.rightPeace && !handtracking.leftPeace)
            {
                SendOSCMessage(address1);
            }
            else
            {
                SendOSCMessage(address2);
            }
        }

        public void NotTouched()
        {
            isTouched = false;
        }

        public void SendOSCMessage(string address, float value = 0.5f)
        {
            OscMessage message = new OscMessage();
            message.address = address;
            message.values.Add(value);
            GameObject.FindGameObjectWithTag("OSC").GetComponent<OSC>().Send(message);
            Debug.Log(this.gameObject.name + " sending OSC:" + message); // todo remove
        }

        void OnReceiveOSC(OscMessage message)
        {
            // pulse particles inside core
        }

        private IEnumerator PlayParticles()
        {
            emission.enabled = true;
            yield return new WaitForSeconds(0.3f);
            emission.enabled = false;
        }

        public string GetDrumAddress()
        {
            return address1;
        }
    }
}
