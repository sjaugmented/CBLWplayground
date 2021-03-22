using LW.HSL;
using UnityEngine;

namespace LW.SlingShot
{
    [RequireComponent(typeof(Rigidbody))]
    public class PebbleController : MonoBehaviour
    {
        public float Force { get; set; }

        float time = 0;
        float lifeSpan = 5;

        float hue;
        float sat;
        float val;

        ColorPicker colorPicker;

        void Start()
        {
            GetComponent<Rigidbody>().AddForce(transform.forward * Force);
            colorPicker = GameObject.FindGameObjectWithTag("ColorPicker").GetComponent<ColorPicker>();

            hue = colorPicker.hueFloat;
            sat = colorPicker.satFloat;
            val = colorPicker.valFloat;
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
            Debug.Log("collision with " + other.name); // TODO remove
            
            if (other.gameObject.CompareTag("Light"))
			{
                Material lightMaterial = other.gameObject.GetComponentInChildren<Renderer>().material;
                lightMaterial.color = GetComponent<PebbleColor>().StoredColor;

                other.GetComponent<LightHolo>().ChangeDMX(hue, sat, val);
                other.GetComponent<LightHolo>().ChangeOSC(hue, sat, val);
			}
        }
    }
}
