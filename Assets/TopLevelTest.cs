using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopLevelTest : MonoBehaviour
{
    [SerializeField] Transform magicSel;
    [SerializeField] Transform precisionSel;
    [SerializeField] float scaleMultiplier = 2;
    [SerializeField] Material selectedMat;
    Vector3 defaultScale;
    Material defaultMat;

    Renderer magicRender;
    Renderer precisionRender;
 
    private void Start()
    {
        
        
        magicRender = magicSel.gameObject.GetComponent<Renderer>();
        precisionRender = precisionSel.gameObject.GetComponent<Renderer>();

        defaultScale = magicSel.localScale;
        defaultMat = magicRender.material;


    }

    public void MagicSelected()
    {
        magicSel.localScale = defaultScale * scaleMultiplier;
        magicRender.material = selectedMat;

        precisionSel.localScale = defaultScale;
        precisionRender.material = defaultMat;
    }

    public void PrecisionSelected()
    {
        magicSel.localScale = defaultScale;
        magicRender.material = defaultMat;

        precisionSel.localScale = defaultScale * scaleMultiplier;
        precisionRender.material = selectedMat;
    }

}
