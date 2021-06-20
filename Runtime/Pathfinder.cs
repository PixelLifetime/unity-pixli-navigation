using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Pathfinder<TGrid> : ScriptableObject
{
	public abstract Vector3[] CalculatePath(Vector3 start, Vector3 end, TGrid grid);
}