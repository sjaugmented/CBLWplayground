using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MagicController))]
public class OrbMaterialController : MonoBehaviour
{
    [Header("Master Orb Materials", order = 0)]
    [SerializeField] Material clearMat;

    [Header("Light Materials", order = 1)]
    public List<Material> lightMats = new List<Material>();

    [Header("Fire Materials")]
    public List<Material> fireMats = new List<Material>();

    [Header("Water Materials")]
    public List<Material> waterMats = new List<Material>();

    MagicController magic;

    void Awake()
    {
        magic = GetComponent<MagicController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (magic.enabled)
        {
            // determine color scheme based on current element
            if (magic.currState == MagicController.State.scaler)
            {
                // determine material based on staff angle
                if (magic.currEl == MagicController.Element.light)
                {
                    for (int i = 0; i < lightMats.Count; i++)
                    {
                        if (i == magic.staffIndex) ShowStaffAngle(lightMats[i]);
                    }
                }

                if (magic.currEl == MagicController.Element.fire)
                {
                    for (int i = 0; i < fireMats.Count; i++)
                    {
                        if (i == magic.staffIndex) ShowStaffAngle(fireMats[i]);
                    }
                }

                if (magic.currEl == MagicController.Element.water)
                {
                    for (int i = 0; i < waterMats.Count; i++)
                    {
                        if (i == magic.staffIndex) ShowStaffAngle(waterMats[i]);
                    }
                }

            }
            else
            {
                // make orb mat clear transparency
                ShowStaffAngle(clearMat);
            }
        }
    }

    private void ShowStaffAngle(Material colorMat)
    {
        Renderer orbRender = magic.masterOrbs[magic.elIndex].GetComponent<Renderer>();
        orbRender.material = colorMat;
    }
}
