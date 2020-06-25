using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GestureToggleObj : MonoBehaviour
{
    bool gesturesToggled = false;
    bool modeToggled = false;

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
            if (!gesturesToggled)
            {
                Director director = FindObjectOfType<Director>();
                director.ToggleGestures();
                Debug.Log("collided!");
                gesturesToggled = true;
                StartCoroutine("ToggleDelay");
            }
            
        }

        if (other.CompareTag("Right Pointer"))
        {
            if (!modeToggled)
            {
                Director director = FindObjectOfType<Director>();
                director.ToggleMode();
                Debug.Log("collided!");
                modeToggled = true;
                StartCoroutine("ToggleDelay");
            }

        }
    }

    IEnumerator ToggleDelay()
    {
        if (gesturesToggled)
        {
            yield return new WaitForSeconds(0.5f);
            gesturesToggled = false;
        }

        if (modeToggled)
        {
            yield return new WaitForSeconds(0.5f);
            modeToggled = false;
        }

    }
}
