using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    

    public void LoadMagic()
    {
        SceneManager.LoadScene(0);
    }

    public void LoadPrecision()
    {
        SceneManager.LoadScene(1);
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
