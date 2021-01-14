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
        [SerializeField] float rateOfRotation = 0.5f;

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
            runeParent = UtilityFunctions.FindParentWithTag(gameObject, "Rune").GetComponent<RuneController>();

            GetComponent<Renderer>().material.color = runeParent.RuneMaterial.color;
        }

        void Update()
		{
            if (GetComponentInParent<NodeCompass>().Expanded) transform.Rotate(0, rateOfRotation, 0);
            if (GameObject.FindGameObjectWithTag("Director").GetComponent<RunicDirector>().Gaze) GetComponent<BoxCollider>().enabled = true;
            else GetComponent<BoxCollider>().enabled = false;
		}
        
        public void IsTouched()
        {
            if (runeParent.Manipulated) return;
            
            if (!Touched)
			{
                StartCoroutine("ExplodeAndDeactivate");

                string message = runeParent.addressBasic1 + "/node" + nodeIndex;

                runeParent.SendOSCMessage(message);
			}
		}

        public void NotTouched()
		{
            // do nothing
		}

        IEnumerator ExplodeAndDeactivate()
		{
            GetComponentInParent<NodeCompass>().Timer = 0;
            Touched = true;

            GetComponent<MeshExploder>().Explode();
            GetComponent<MeshRenderer>().enabled = false;
            GetComponent<AudioSource>().PlayOneShot(touchFX);
            yield return new WaitForSeconds(2f);
        }

        public void IsGazed()
		{
            if (runeParent.Manipulated) return;
            
            if (GameObject.FindGameObjectWithTag("Director").GetComponent<RunicDirector>().Gaze)
			{
                StartCoroutine("ExplodeAndDeactivate");

                RuneController runeParent = UtilityFunctions.FindParentWithTag(gameObject, "Rune").GetComponent<RuneController>();
                string message = runeParent.addressBasic1 + "/node" + nodeIndex;

                runeParent.SendOSCMessage(message);
            }
		}

        void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Rune"))
            {
                Collider ignoredCollider = collision.gameObject.GetComponent<Collider>();
                Physics.IgnoreCollision(ignoredCollider, GetComponent<Collider>());
            }

            Debug.Log(gameObject.name + " collided with " + collision.gameObject.name);
        }
    }
}
