using System.Collections;
using UnityEngine;
using LW.Core;

public class ThrowController : MonoBehaviour
{
    HandTracking handTracker;

    [SerializeField] float castThreshold = 10;

    [SerializeField] GameObject throwOrb;

    // Start is called before the first frame update
    void Start()
    {
        handTracker = FindObjectOfType<HandTracking>();
    }

    float previousDist;
    public float magnifier = 10;
    public float velocity;

    bool canCast = true;

    // Update is called once per frame
    void Update()
    {
        if (handTracker.rightThrower)
        {
            float currentDist = Vector3.Distance(Camera.main.transform.position, handTracker.rightPalm.Position);

            velocity = (currentDist - previousDist) / Time.deltaTime * magnifier;

            previousDist = Vector3.Distance(Camera.main.transform.position, handTracker.rightPalm.Position);

        }
        else canCast = true;

        if (velocity >= castThreshold)
        {
            if (canCast)
            {
                GameObject spellOrb = Instantiate(throwOrb, handTracker.rightPalm.Position, Camera.main.transform.rotation);
                StartCoroutine("CastDelay");
                //spellOrb.transform.localScale = new Vector3(0.05784709f, 0.05784709f, 0.05784709f);
                ThrowOrbController spellController = spellOrb.GetComponent<ThrowOrbController>();
                spellController.force = velocity;
            }

        }

    }

    IEnumerator CastDelay()
    {
        canCast = false;
        yield return new WaitForSeconds(1);
        canCast = true;
    }
}
