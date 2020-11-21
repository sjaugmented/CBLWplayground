using System.Collections.Generic;
using UnityEngine;

public class ElementMenu : MonoBehaviour
{
    [Tooltip("Populates ")]
    public List<Transform> elements = new List<Transform>();
    public List<Transform> hovers = new List<Transform>();
    MagicController spellManager;
    float defaultScale = 0.08408988f;
    [SerializeField] float selectedScale = 3;

    // Start is called before the first frame update
    void Start()
    {
        spellManager = FindObjectOfType<MagicController>();

        foreach (Transform child in transform)
        {
            if (child.CompareTag("Element Selector")) elements.Add(child);
            if (child.CompareTag("Hover Selector")) hovers.Add(child);
        }
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < elements.Count; i++)
        {
            if (i == spellManager.elIndex)
            {
                elements[i].localScale = new Vector3(defaultScale * selectedScale, defaultScale * selectedScale, defaultScale * selectedScale);
                elements[i].Rotate(10, 1, -1);
            }
            else elements[i].localScale = new Vector3(defaultScale, defaultScale, defaultScale);
        }

        if (!spellManager.hoverOrb)
        {
            hovers[0].localScale = new Vector3(defaultScale * selectedScale, defaultScale * selectedScale, defaultScale * selectedScale);
            hovers[0].Rotate(10, 1, -1);

            hovers[1].localScale = new Vector3(defaultScale, defaultScale, defaultScale);
        }
        else
        {
            hovers[0].localScale = new Vector3(defaultScale, defaultScale, defaultScale);

            hovers[1].localScale = new Vector3(defaultScale * selectedScale, defaultScale * selectedScale, defaultScale * selectedScale);
            hovers[1].Rotate(10, 1, -1);
        }
    }
}
