using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LW.HSL;

namespace LW.SlingShot
{
    public class PebbleColor : MonoBehaviour
    {
        [SerializeField] bool projectile = false;

        ColorPicker colorPicker;
        Material material;
        void Start()
        {
            colorPicker = GameObject.FindGameObjectWithTag("ColorPicker").GetComponent<ColorPicker>();
            material = GetComponentInChildren<Renderer>().material;
            material.color = colorPicker.ChosenColor;
            material.SetColor("_EmissionColor", colorPicker.ChosenColor);
        }

        void Update()
        {
            if (!projectile)
			{
                material.color = colorPicker.ChosenColor;
                material.SetColor("_EmissionColor", colorPicker.ChosenColor);
            }
        }
    }
}


