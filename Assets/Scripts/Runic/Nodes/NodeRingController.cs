using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LW.Runic
{
    public class NodeRingController : MonoBehaviour
    {
        [SerializeField] AudioClip expandFX;
        [SerializeField] float inactiveScale = 0.40757f;
        [SerializeField] float duration = 1f;
        [SerializeField] float growFactor = 1;

        float timer = Mathf.Infinity;
		public float Timer { get => timer; set => timer = value; }

        public List<NodeTouch> touchNodes = new List<NodeTouch>(); // TODO private

        void Start()
		{
            for (int i = 0; i < transform.childCount; i++)
			{
                touchNodes.Add(transform.GetChild(i).GetComponent<NodeTouch>());
			}

            DeactivateTouchNodes();
		}

		void Update()
        {
            timer += Time.deltaTime;
            transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);
            if (timer > duration) DeactivateNodeRing();
        }

        public void ActivateNodeRing()
		{
            gameObject.transform.localScale = new Vector3(inactiveScale, inactiveScale, inactiveScale);
            gameObject.SetActive(true);
            timer = 0;
            StartCoroutine("ExpandRing");
		}

        public void DeactivateNodeRing()
		{
            DeactivateTouchNodes();

            gameObject.SetActive(false);
		}

        void ActivateTouchNodes()
		{
            foreach (NodeTouch node in touchNodes)
            {
                node.enabled = true;
            }
        }

        void DeactivateTouchNodes()
		{
            foreach (NodeTouch node in touchNodes)
            {
                node.enabled = false;
            }
        }

        IEnumerator ExpandRing()
		{
            float timer = 0;
            GetComponent<AudioSource>().PlayOneShot(expandFX);
            while (transform.localScale.x < 1)
            {
                timer += Time.deltaTime;
                transform.localScale += new Vector3(1, 1, 1) * Time.deltaTime * growFactor;
                yield return null;
            }

            yield return new WaitForSeconds(0.2f);

            ActivateTouchNodes();
        }
    }
}
