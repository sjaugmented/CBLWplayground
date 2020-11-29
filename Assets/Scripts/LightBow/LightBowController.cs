using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LW.Core;

namespace LW.LightBow
{
    public class LightBowController : MonoBehaviour
    {
        [SerializeField] GameObject peaceIndicator;
        
        HandTracking handtracking;
        BowSights bowSights;

        // Start is called before the first frame update
        void Start()
        {
            handtracking = GameObject.FindGameObjectWithTag("Handtracking").GetComponent<HandTracking>();
            bowSights = GetComponent<BowSights>();
        }

        // Update is called once per frame
        void Update()
        {
            if (handtracking.rightPeace)
            {
                ActivateBow(bowSights.rightIndex, bowSights.rightMiddle, bowSights.GetRightSight(), handtracking.leftPalm.Position);
            }
            else if (handtracking.leftPeace)
            {
                ActivateBow(bowSights.leftIndex, bowSights.leftMiddle, bowSights.GetLeftSight(), handtracking.rightPalm.Position);
            }
            else
            {
                ClearExistingLines();
                peaceIndicator.SetActive(false);
            }
        }

        private void ActivateBow(Vector3 sightStart, Vector3 sightEnd, Vector3 arrowStart, Vector3 arrowEnd)
        {
            ClearExistingLines();
            peaceIndicator.SetActive(true);
            DrawLine(sightStart, sightEnd, Color.red);
            DrawLine(arrowStart, arrowEnd, Color.white);
        }

        private void DrawLine(Vector3 start, Vector3 end, Color color)
        {
            GameObject myLine = new GameObject();
            myLine.tag = "line";
            myLine.transform.position = start;
            myLine.AddComponent<LineRenderer>();
            LineRenderer lr = myLine.GetComponent<LineRenderer>();
            lr.material = new Material(Shader.Find("Particles/Alpha Blended Premultiply"));
            lr.startColor = color;
            lr.endColor = color;
            lr.startWidth = 0.1f;
            lr.endWidth = 0.1f;
            lr.SetPosition(0, start);
            lr.SetPosition(1, end);
        }

        private void ClearExistingLines()
        {
            GameObject[] lines = GameObject.FindGameObjectsWithTag("line");

            foreach(GameObject line in lines)
            {
                Destroy(line);
            }
        }
    }
}
