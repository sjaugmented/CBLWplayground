using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LW.Runic
{
    public class RuneBelt : MonoBehaviour
    {
        [SerializeField] RuneSlot[] runeSlots;

		[System.Serializable]
        private class RuneSlot
		{
			public GameObject prefab;
			
			public RuneType runeType;
            public int runeCount;
		}

		public GameObject GetRunePrefab(RuneType runeType)
		{
			return GetRuneSlot(runeType).prefab;
		}

		public int GetRuneSlots()
		{
			return runeSlots.Length;
		}

		public int GetCurrentRuneAmmo(RuneType runeType)
		{
			
			return GetRuneSlot(runeType).runeCount;
		}

		public void ReduceCurrentRuneAmmo(RuneType runeType)
		{
			GetRuneSlot(runeType).runeCount--;
		}

		public void ResetAllRuneAmmo(int maxColorVariants)
		{
			foreach(RuneSlot rune in runeSlots)
			{
				rune.runeCount = maxColorVariants;
			}
		}

		private RuneSlot GetRuneSlot(RuneType runeType)
		{
			foreach (RuneSlot rune in runeSlots)
			{
				if (rune.runeType == runeType)
				{
					return rune;
				}
			}
			return null;
		}
	}
}
