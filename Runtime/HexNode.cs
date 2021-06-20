using System.Collections;
using System.Collections.Generic;
using Priority_Queue;
using UnityEngine;

[System.Serializable]
public class HexNode : FastPriorityQueueNode
{
	public HexTile HexTile { get; set; }
}