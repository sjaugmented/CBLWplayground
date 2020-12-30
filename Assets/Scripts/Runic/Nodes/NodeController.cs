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
        
        public void Touched()
        {
            RuneController runeParent = UtilityFunctions.FindParentWithTag(gameObject, "Rune").GetComponent<RuneController>();
            string message = runeParent.address1 + "/node" + nodeIndex;

            runeParent.SendOSCMessage(message);

            StartCoroutine("ExplodeAndDeactivate");
		}

        public void NotTouched()
		{
            // do nothing
		}

        IEnumerator ExplodeAndDeactivate()
		{
            GetComponentInParent<NodeRingController>().Timer = 0;
            // explode collider
            GetComponent<MeshExploder>().Explode();
            GetComponent<MeshRenderer>().enabled = false;
            GetComponent<AudioSource>().PlayOneShot(touchFX);
            while (GetComponent<AudioSource>().isPlaying)
			{
                yield return new WaitForSeconds(1);
			}
            GetComponentInParent<NodeRingController>().Timer = Mathf.Infinity;
            GetComponent<MeshRenderer>().enabled = true;
        }
    }
}
