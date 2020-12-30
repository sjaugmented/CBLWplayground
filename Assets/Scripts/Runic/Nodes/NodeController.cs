using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LW.Utility;

namespace LW.Runic
{
    public class NodeController : MonoBehaviour
    {
        [SerializeField] int nodeIndex = 1;

        public void Touched()
        {
			RuneController runeParent = UtilityFunctions.FindParentWithTag(gameObject, "Rune").GetComponent<RuneController>();
            string message = runeParent.address1 + "/node" + nodeIndex;

            runeParent.SendOSCMessage(message);

            GetComponentInParent<NodeRingController>().Timer = Mathf.Infinity;
		}

        public void NotTouched()
		{
            // do nothing
		}
    }
}
