using UnityEngine;

namespace MatchBoard
{
	[System.Serializable]
	public class ArrayLayout
	{
		[System.Serializable]
		public struct rowData{
			public bool[] row;
		}

		public Grid grid;
		public rowData[] rows = new rowData[7]; //Grid of 7x7
	}
}
