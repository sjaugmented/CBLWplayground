using System.Collections;
using UnityEngine;

namespace LW.HoverDrums
{
    public class HoverDrumController : MonoBehaviour
    {
        [SerializeField] AudioClip castFX;
        [SerializeField] AudioClip touchFX;
        [SerializeField] ParticleSystem particles;

        public float force = 1;
        public string address;
        
        [Range(0, 1)] public HSV color;
        bool isTouched = false;

        Renderer renderer;
        bool emission;

        void Start()
        {
            renderer = GetComponentInChildren<Renderer>();
            GetComponent<Rigidbody>().AddForce(transform.forward * force);

            GetComponent<AudioSource>().PlayOneShot(castFX);

            GameObject.FindGameObjectWithTag("OSC").GetComponent<OSC>().SetAddressHandler(address + "/receive", OnReceiveOSC);

            emission = particles.emission.enabled;
            emission = false;
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
            address = transform.GetChild(0).name + drumNum.ToString();
        }

        public void Touched()
        {
            isTouched = true;
            GetComponent<AudioSource>().PlayOneShot(touchFX);
            SendOSCMessage(address);
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
            emission = true;
            yield return new WaitForSeconds(0.3f);
            emission = false;
        }

        public string GetDrumAddress()
        {
            return address;
        }
    }
}
