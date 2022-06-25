using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlockBuilder.Runtime.Core;
using MugCup_BlockBuilder.Runtime;
using MugCup_BlockBuilder.Runtime.Core;

namespace BlockBuilder.Runtime.Scriptable
{
	[CreateAssetMenu(fileName = "AssetCollection", menuName = "ScriptableObjects/AssetCollectionObject", order = 4)]
	public class AssetCollection : ScriptableObject
	{
		public Block DefualtBlock;
		public GameObject PathPointerVisualizer;
	}
	
}
