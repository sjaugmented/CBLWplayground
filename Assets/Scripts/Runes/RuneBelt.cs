using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LW.Runic
{
    public class RuneBelt : MonoBehaviour
    {
		[SerializeField] int maxRuneVariants = 8;
        [SerializeField] RuneSlot[] runeSlots;

		[System.Serializable]
        private class RuneSlot
		{
			[SerializeField] GameObject prefab;
			
			public RuneType runeType;
            public int runeCount;
		}

		void Start()
		{
			ResetAllRuneAmmo();
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

		public void ResetAllRuneAmmo()
		{
			foreach(RuneSlot rune in runeSlots)
			{
				rune.runeCount = maxRuneVariants;
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
