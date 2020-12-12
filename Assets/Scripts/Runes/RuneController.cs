using System.Collections;
using UnityEngine;
using LW.Core;
using static UnityEngine.ParticleSystem;

namespace LW.Runic
{
    public class RuneController : MonoBehaviour
    {
        [SerializeField] AudioClip castFX;
        [SerializeField] AudioClip touchFX;
        [SerializeField] ParticleSystem particles;
        public bool oscTest = false;

        public float force = 1;
        public string address1;
        public string address2;
        
        [Range(0, 1)] public HSV color;
        bool isTouched = false;

        Renderer renderer;
        RunicDirector director;
        HandTracking handtracking;
        EmissionModule emission;

        void Start()
        {
            renderer = GetComponentInChildren<Renderer>();
            director = GameObject.FindGameObjectWithTag("Director").GetComponent<RunicDirector>();
            handtracking = GameObject.FindGameObjectWithTag("Handtracking").GetComponent<HandTracking>();
            GetComponent<Rigidbody>().AddForce(transform.forward * force);

            GetComponent<AudioSource>().PlayOneShot(castFX);

            GameObject.FindGameObjectWithTag("OSC").GetComponent<OSC>().SetAddressHandler(address1 + "/receive", OnReceiveOSC);
            GameObject.FindGameObjectWithTag("OSC").GetComponent<OSC>().SetAllMessageHandler(OnReceiveOSC);

            emission = particles.emission;
            emission.enabled = false;
        }

        void Update()
        {
            if (isTouched)
            {
                renderer.material.color = Color.white;
            }
            else renderer.material.color = Color.HSVToRGB(color.Hue, color.Sat, color.Val);

            if (oscTest) StartCoroutine("PlayParticles");
        }

        public void SetRuneAddressAndColor(int runeNum, HSV colorValue)
        {
            address1 = transform.GetChild(0).name + runeNum + "a".ToString();
            address2 = transform.GetChild(0).name + runeNum + "b".ToString();
            
            color = colorValue;
            MainModule particlesMain = particles.main;
            particlesMain.startColor = Color.HSVToRGB(color.Hue, color.Sat, color.Val);
        }

        public void Touched()
        {
            if (director.currentMode == RunicDirector.Mode.Touch)
			{
                GetComponent<AudioSource>().PlayOneShot(touchFX);
                // if touched with one finger
                if (!handtracking.rightPeace && !handtracking.leftPeace)
                {
                    isTouched = true;
                    SendOSCMessage(address1);
                }
                else
                {
                    StartCoroutine("TwoFingerTouchFlicker");
                    SendOSCMessage(address2);
                }
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
            Debug.Log("OSC received: " + message);
            StartCoroutine("PlayParticles");
        }

        private IEnumerator TwoFingerTouchFlicker()
		{
            isTouched = true;
            yield return new WaitForSeconds(0.05f);
            isTouched = false;
            yield return new WaitForSeconds(0.1f);
            isTouched = true;
            yield return new WaitForSeconds(0.1f);
            isTouched = false;
		}

        private IEnumerator PlayParticles()
        {
            Debug.Log("pulsing");
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
