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

        void OnTriggerEnter(Collider other)
        {
            Debug.Log("collision with " + other.name);
            
            if (other.gameObject.CompareTag("Light"))
			{
                Material lightMaterial = other.gameObject.GetComponent<Renderer>().material;
                lightMaterial.color = GetComponent<PebbleColor>().StoredColor;
                Debug.Log("///Saved Color info///");
                Debug.Log(GetComponent<PebbleColor>().StoredColor);
                Debug.Log("///Setting Color info///");
                Debug.Log(lightMaterial.color);
			}
        }
    }
}
