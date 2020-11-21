using UnityEngine;

public class MasterOrbRotater : MonoBehaviour
{
    public float xRotation = 1f;
    public float yRotation = 1f;
    public float zRotation = 1f;

    public bool scalerActive;

    MagicController spellManager;

    // Start is called before the first frame update
    void Start()
    {
        spellManager = FindObjectOfType<MagicController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (spellManager.fromOrbScaler) transform.Rotate(xRotation, yRotation, zRotation);
        else transform.forward = Camera.main.transform.forward;
    }
}
