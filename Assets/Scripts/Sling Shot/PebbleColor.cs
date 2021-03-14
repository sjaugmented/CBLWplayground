using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LW.HSL;

namespace LW.SlingShot
{
    public class PebbleColor : MonoBehaviour
    {
        [Tooltip("Instantiated projectile vs static HUD element")]
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
            
            StoredColor = colorPicker.PreviewColor;
            //material.color = StoredColor;
            //material.SetColor("_EmissionColor", StoredColor);

            Debug.Log("Pebble's StoredColor: " + StoredColor);
        }

        void Update()
        {
            if (!projectile)
            {
                material.color = colorPicker.PreviewColor;
                material.SetColor("_EmissionColor", colorPicker.PreviewColor);
            }
        }
    }
}


