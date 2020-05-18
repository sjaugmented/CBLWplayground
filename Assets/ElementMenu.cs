using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementMenu : MonoBehaviour
{
    public List<Transform> elements = new List<Transform>();
    SpellManager spellManager;
    float defaultScale = 0.05784709f;
    float selectedScale = 2;

    // Start is called before the first frame update
    void Start()
    {
        spellManager = FindObjectOfType<SpellManager>();
        
        foreach (Transform child in transform)
        {
            elements.Add(child);
        }
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < elements.Count; i++)
        {
            if (i == spellManager.elementID)
            {
                elements[i].localScale = new Vector3(selectedScale, selectedScale, selectedScale);
            }
            else elements[i].localScale = new Vector3(defaultScale, defaultScale, defaultScale);
        }
    }
}
