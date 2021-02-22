using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LW.SlingShot
{
    public class LiveHud : MonoBehaviour
    {
        void Update()
        {
			transform.Rotate(0, 0, 0.2f);
            transform.LookAt(Camera.main.transform.position);
		}
    }

}
