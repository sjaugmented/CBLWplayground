using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AnchorMonitor : MonoBehaviour
{
    [SerializeField] GameObject sessionStartedCanvas;
    [SerializeField] GameObject anchorCreatedCanvas;
    [SerializeField] GameObject anchorSharedCanvas;
    [SerializeField] GameObject anchorRetrievedCanvas;
    [SerializeField] GameObject uhohCanvas;
    [SerializeField] GameObject errMessage;

    public bool AnchorCreated { get; set; }
    public bool AnchorShared { get; set; }
    public bool AnchorRetrieved { get; set; }

    private void Start()
    {
        sessionStartedCanvas.SetActive(false);
        anchorCreatedCanvas.SetActive(false);
        anchorSharedCanvas.SetActive(false);
        anchorRetrievedCanvas.SetActive(false);
        uhohCanvas.SetActive(false);
    }

    public void StartSession()
    {
        StartCoroutine("ShowCanvas", sessionStartedCanvas);
    }

    public void SetAnchorCreated()
    {
        AnchorCreated = true;
        StartCoroutine("ShowCanvas", anchorCreatedCanvas);
    }

    public void SetAnchorShared()
    {
        AnchorShared = true;
        StartCoroutine("ShowCanvas", anchorSharedCanvas);
    }
    
    public void SetAnchorRetrieved()
    {
        AnchorRetrieved = true;
        StartCoroutine("ShowCanvas", anchorRetrievedCanvas);
    }

    public void ShowUhoh(string error)
    {
        errMessage.GetComponent<TextMeshPro>().text = error;
        StartCoroutine("ShowCanvas", uhohCanvas);
    }

    IEnumerator ShowCanvas(GameObject canvas)
    {
        canvas.SetActive(true);
        yield return new WaitForSeconds(5);
        canvas.SetActive(false);
    }
}
