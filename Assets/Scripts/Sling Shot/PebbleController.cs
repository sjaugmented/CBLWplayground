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

        void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Light"))
			{
                Material lightMaterial = collision.gameObject.GetComponent<Renderer>().material;
                lightMaterial.color = GetComponent<Renderer>().material.color;
                Debug.Log("///Color info///");
                Debug.Log(lightMaterial.color);
			}
        }
    }
}
