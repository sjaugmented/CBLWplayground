using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LW.Runic
{
    public class RuneBelt : MonoBehaviour
    {
        //[SerializeField] List<GameObject> runePrefabs;
        [SerializeField] RuneSlot[] runeSlots;

		public int GetRuneSlots()
		{
			return runeSlots.Length;
		}

        [System.Serializable]
        private class RuneSlot
		{
			[SerializeField] GameObject prefab;
			public RuneType runeType;
            public int runeCount = 8;
		}

		//public int GetCurrentRuneCount()
		//{
		//	return runeCount;
		//}

		//public void ReduceCurrentRuneCount()
		//{
		//	runeCount--;
		//}
	}
}
