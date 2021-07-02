using MRTK.Tutorials.MultiUserCapabilities;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallAnchorManager : MonoBehaviour
{
    bool firstIn;
    bool shared, getted, createInitiated, shareInitiated, getInitiated;

    PhotonLobby lobby;
    AnchorMonitor monitor;
    AnchorModuleScript anchorModule;
    SharingModuleScript sharingModule;

    void Start()
    {
        lobby = FindObjectOfType<PhotonLobby>();
        monitor = FindObjectOfType<AnchorMonitor>();
        anchorModule = FindObjectOfType<AnchorModuleScript>();
        sharingModule = FindObjectOfType<SharingModuleScript>();
        

        firstIn = PhotonNetwork.CountOfPlayersInRooms == 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (lobby.Connected)
        {
            if (firstIn)
            {
                if (!shared)
                {
                    StartCoroutine("EstablishAnchor");
                    shared = true;
                }
                
                //if (!monitor.AnchorCreated && PhotonNetwork.CountOfPlayersInRooms == 0)
                //{
                //    if(!createInitiated)
                //    {
                //        var anchorObject = FindObjectOfType<TableAnchor>().gameObject;

                //        anchorModule.CreateAzureAnchor(anchorObject);
                //        createInitiated = true;
                //    }
                //}

                //if (monitor.AnchorCreated && !monitor.AnchorShared)
                //{
                //    if (!shareInitiated)
                //    {
                //        sharingModule.ShareAzureAnchor();
                //        shareInitiated = true;
                //    }
                //}
            }
            else
            {
                if (!getted)
                {
                    StartCoroutine("GetAnchor");
                    getted = true;
                }
                //if (!getInitiated)
                //{
                //    sharingModule.GetAzureAnchor();
                //    getInitiated = true;
                //}
            }
        }
    }

    IEnumerator EstablishAnchor()
    {
        var anchorObject = FindObjectOfType<TableAnchor>().gameObject;
        
        anchorModule.StartAzureSession(); 
        yield return new WaitForSeconds(10);
        anchorModule.CreateAzureAnchor(anchorObject);
        yield return new WaitForSeconds(10);
        sharingModule.ShareAzureAnchor();
    }

    IEnumerator GetAnchor()
    {
        anchorModule.StartAzureSession();
        yield return new WaitForSeconds(10);
        sharingModule.GetAzureAnchor();
    }
}
