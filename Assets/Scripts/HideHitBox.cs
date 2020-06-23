using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class HideHitBox : MonoBehaviour
{
    [SerializeField] Material gazedMaterial;
    Material defaultMaterial;
    float defaultAlpha;
    
    Renderer thisRenderer;
    bool visible = true;
    bool lookedAt = false;
    
    // Start is called before the first frame update
    void Start()
    {
        thisRenderer = GetComponent<Renderer>();
        defaultMaterial = thisRenderer.material;
        defaultAlpha = thisRenderer.material.color.a;
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

    public void LookedAt()
    {
        //this.transform.Rotate(0, 2, 0);
        thisRenderer.material = gazedMaterial;
        
    }

    public void LookedAway()
    {
        //this.transform.Rotate(0, 0, 0);
        thisRenderer.material = defaultMaterial;
    }
}
