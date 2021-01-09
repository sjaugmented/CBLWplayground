using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LW.Utility;

namespace LW.Runic
{
    public class NodeController : MonoBehaviour
    {
        [SerializeField] int nodeIndex = 1;
        [SerializeField] AudioClip touchFX;

        bool touched = false;
        float gazeTimer = 0;
        public bool Touched
		{
            get { return touched; }
            set { touched = value; }
		}

        RuneController runeParent;

        void Start()
		{
            runeParent = runeParent = UtilityFunctions.FindParentWithTag(gameObject, "Rune").GetComponent<RuneController>();

            GetComponent<Renderer>().material.color = runeParent.RuneMaterial.color;
        }
        
        public void IsTouched()
        {
            if (!Touched)
			{
                StartCoroutine("ExplodeAndDeactivate");

                string message = runeParent.address1 + "/node" + nodeIndex;

                runeParent.SendOSCMessage(message);
			}
		}

        public void NotTouched()
		{
            // do nothing
		}

        IEnumerator ExplodeAndDeactivate()
		{
            GetComponentInParent<NodeRingController>().Timer = 0;
            Touched = true;

            GetComponent<MeshExploder>().Explode();
            GetComponent<MeshRenderer>().enabled = false;
            GetComponent<AudioSource>().PlayOneShot(touchFX);
            yield return new WaitForSeconds(2f);
            //Touched = false;
            //GetComponentInParent<NodeRingController>().Timer = Mathf.Infinity;
        }

        public void IsGazed()
		{
            if (GameObject.FindGameObjectWithTag("Director").GetComponent<RunicDirector>().Gaze)
			{
                StartCoroutine("ExplodeAndDeactivate");

                RuneController runeParent = UtilityFunctions.FindParentWithTag(gameObject, "Rune").GetComponent<RuneController>();
                string message = runeParent.address1 + "/node" + nodeIndex;

                runeParent.SendOSCMessage(message);
            }
		}
    }
}
