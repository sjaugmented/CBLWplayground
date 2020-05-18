using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementMenu : MonoBehaviour
{
    public List<Transform> elements = new List<Transform>();
    SpellManager spellManager;
    float defaultScale = 0.05784709f;
    [SerializeField] float selectedScale = 3;

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
            if (i == spellManager.GetElementID())
            {
                elements[i].localScale = new Vector3(defaultScale * selectedScale, defaultScale * selectedScale, defaultScale * selectedScale);
            }
            else elements[i].localScale = new Vector3(defaultScale, defaultScale, defaultScale);
        }
    }
}
