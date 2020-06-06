using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class HideHitBox : MonoBehaviour
{
    Renderer thisRenderer;
    bool visible = true;
    
    // Start is called before the first frame update
    void Start()
    {
        thisRenderer = GetComponent<Renderer>();
    }

    public void HideBox()
    {
        if (visible)
        {
            thisRenderer.enabled = false;
            visible = false;
        }
        else
        {
            thisRenderer.enabled = true;
            visible = true;
        }
        
    }
}
