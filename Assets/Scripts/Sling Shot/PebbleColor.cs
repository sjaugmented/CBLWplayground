using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LW.HSL;

namespace LW.SlingShot
{
    public class PebbleColor : MonoBehaviour
    {
        [SerializeField] bool projectile = false;

        public Color StoredColor
		{
            get; set;
		}

        ColorPicker colorPicker;
        Material material;
        void Start()
        {
            colorPicker = GameObject.FindGameObjectWithTag("ColorPicker").GetComponent<ColorPicker>();
            material = GetComponentInChildren<Renderer>().material;
            StoredColor = colorPicker.ChosenColor;
            material.color = StoredColor;
            material.SetColor("_EmissionColor", StoredColor);
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


