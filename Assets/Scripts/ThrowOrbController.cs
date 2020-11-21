using System.Collections;
using UnityEngine;

public class ThrowOrbController : MonoBehaviour
{
    [SerializeField] float lifeSpan = 5f;
    public float force = 50;
    [SerializeField] Material slowMat;
    [SerializeField] Material medMat;
    [SerializeField] Material fastMat;

    Rigidbody rigidBody;
    Renderer thisRen;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        thisRen = GetComponent<Renderer>();

        StartCoroutine("SelfDestruct");
    }

    // Update is called once per frame
    void Update()
    {
        if (force >= 10 && force < 15)
        {
            thisRen.material = slowMat;
        }
        else if (force >= 15 && force < 20)
        {
            thisRen.material = medMat;
        }
        else if (force >= 20)
        {
            thisRen.material = fastMat;
        }
    }

    IEnumerator SelfDestruct()
    {
        yield return new WaitForSeconds(lifeSpan);
        Destroy(gameObject);
    }

    void FixedUpdate()
    {
        rigidBody.AddForce(transform.forward * force);
    }
}
