using UnityEngine;

namespace LW.SlingShot
{
    [RequireComponent(typeof(Rigidbody))]
    public class PebbleController : MonoBehaviour
    {
        public float Force { get; set; }

        float time = 0;
        float lifeSpan = 5;

        void Start()
        {
            GetComponent<Rigidbody>().AddForce(transform.forward * Force);
        }

        void Update()
        {
            time += Time.deltaTime;

            if (time > lifeSpan)
            {
                Destroy(gameObject);
            }
        }
    }
}
