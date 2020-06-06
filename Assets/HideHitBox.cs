using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideHitBox : MonoBehaviour
{
    Renderer thisRenderer;
    
    // Start is called before the first frame update
    void Start()
    {
        thisRenderer = GetComponent<Renderer>();
    }

    public void HideBox()
    {
        thisRenderer.enabled = !thisRenderer.enabled;
    }
}
