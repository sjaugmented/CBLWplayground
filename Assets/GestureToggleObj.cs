using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GestureToggleObj : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }private void OnTriggerEnter(Collider other)
    {
        
        if (other.CompareTag("Left Pointer"))
        {
            Director director = FindObjectOfType<Director>();
            director.ToggleGestures();
            Debug.Log("collided!");
        }

        if (other.CompareTag("Right Pointer"))
        {
            Director director = FindObjectOfType<Director>();
            director.ToggleMode();
            Debug.Log("collided!");
        }
    }
}
