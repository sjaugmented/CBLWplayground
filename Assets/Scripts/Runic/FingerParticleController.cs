using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

namespace LW.Runic
{
    public class FingerParticleController : MonoBehaviour
    {
        RunicDirector director;
        EmissionModule emission;
        void Start()
        {
            director = GameObject.FindGameObjectWithTag("Director").GetComponent<RunicDirector>();
            emission = GetComponentInChildren<ParticleSystem>().emission;
        }

        void Update()
        {
			//if (director.currentMode == RunicDirector.Mode.Node) emission.enabled = true;
			//else emission.enabled = false;
		}
    }

}