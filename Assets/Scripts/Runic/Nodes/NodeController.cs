using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LW.Utility;

namespace LW.Runic
{
    public class NodeController : MonoBehaviour
    {
        [SerializeField] int nodeIndex = 1;
        bool isTouched = false;

        void Start()
        {
            
        }

        void Update()
        {
            
        }

        public void Touched()
        {
			string parentAddress = UtilityFunctions.FindParentWithTag(gameObject, "Rune").GetComponent<RuneController>().address1;
			Debug.Log("touched " + parentAddress + "/node" + nodeIndex);

			//Debug.Log("touched node" + nodeID);
		}

        public void NotTouched()
		{

		}
    }
}
