using UnityEngine;

namespace LW.LightBow
{
    [RequireComponent(typeof(Rigidbody))]
    public class ArrowController : MonoBehaviour
    {
        float force;

        // Start is called before the first frame update
        void Start()
        {
            GetComponent<Rigidbody>().AddForce(transform.forward * force);
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
