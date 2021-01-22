using UnityEngine;

namespace LW.SlingShot
{
    [RequireComponent(typeof(Rigidbody))]
    public class ArrowController : MonoBehaviour
    {
        public float force { get; set; }

        float time = 0;
        float lifeSpan = 5;

        void Start()
        {
            GetComponent<Rigidbody>().AddForce(transform.forward * force);
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
