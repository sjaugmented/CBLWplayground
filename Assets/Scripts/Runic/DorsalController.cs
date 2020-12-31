using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LW.Runic
{
    public class DorsalController : MonoBehaviour
    {
        [SerializeField] Material buildActiveColor;
        [SerializeField] Material buildInactiveColor;
        
        List<Renderer> childRend = new List<Renderer>();

        RunicDirector director;
        void Start()
        {
            director = GameObject.FindGameObjectWithTag("Director").GetComponent<RunicDirector>();

            foreach(Transform t in transform)
			{
                childRend.Add(t.GetComponent<Renderer>());
			}
        }

        void Update()
        {
   //         if (director.currentMode == RunicDirector.Mode.Touch)
			//{
   //             SetDorsalMaterial(buildActiveColor);
			//}
			//else
			//{
   //             SetDorsalMaterial(buildInactiveColor);
   //         }
        }

		private void SetDorsalMaterial(Material mat)
		{
			foreach (Renderer color in childRend)
			{
				color.material = mat;
			}
		}
	}
}
