using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudController : MonoBehaviour
{
    [Header("Hand Clouds")]
    [SerializeField] GameObject rightCloudParent;
    [SerializeField] GameObject leftCloudParent;
    public List<GameObject> rightClouds;
    public List<GameObject> leftClouds;

    HandTracking hands;
    MagicController magic;
    Director director;

    void Awake()
    {
        hands = FindObjectOfType<HandTracking>();
        magic = GetComponent<MagicController>();
        director = FindObjectOfType<Director>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (magic.enabled)
        {
            if (!director.readGestures || magic.orbActive == true)
            {
                rightCloudParent.SetActive(false);
                leftCloudParent.SetActive(false);
            }
            else
            {
                // right cloud
                if (hands.rightHand)
                {
                    rightCloudParent.SetActive(true);
                    for (int i = 0; i < rightClouds.Count; i++)
                    {
                        if (i == magic.elementID)
                        {
                            rightClouds[i].SetActive(true);
                        }
                        else rightClouds[i].SetActive(false);
                    }
                }
                else rightCloudParent.SetActive(false);


                // left cloud
                if (hands.leftHand)
                {
                    leftCloudParent.SetActive(true);

                    for (int i = 0; i < leftClouds.Count; i++)
                    {
                        if (i == magic.elementID)
                        {
                            leftClouds[i].SetActive(true);
                        }
                        else leftClouds[i].SetActive(false);
                    }
                }
                else leftCloudParent.SetActive(false);
            }
        } else
        {
            rightCloudParent.SetActive(false);
            leftCloudParent.SetActive(false);
        }
    }
        
}

    /*public void ProcessHandClouds()
    {
        if (!director.readGestures || magic.orbActive == true)
        {
            rightCloudParent.SetActive(false);
            leftCloudParent.SetActive(false);
        }
        else
        {
            // right cloud
            if (hands.rightHand)
            {
                rightCloudParent.SetActive(true);
                for (int i = 0; i < rightClouds.Count; i++)
                {
                    if (i == magic.elementID)
                    {
                        rightClouds[i].SetActive(true);
                    }
                    else rightClouds[i].SetActive(false);
                }
            }
            else rightCloudParent.SetActive(false);


            // left cloud
            if (hands.leftHand)
            {
                leftCloudParent.SetActive(true);

                for (int i = 0; i < leftClouds.Count; i++)
                {
                    if (i == magic.elementID)
                    {
                        leftClouds[i].SetActive(true);
                    }
                    else leftClouds[i].SetActive(false);
                }
            }
            else leftCloudParent.SetActive(false);
        }
    }*/
//}
