using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PixLi
{
	public interface IPathfinder
	{
		Vector3[] CalculatePath(Vector3 start, Vector3 end, PolytopialSegmentsStructure polytopialSegmentsStructure);
	}

	public abstract class Pathfinder : ScriptableObject, IPathfinder
	{
		public abstract Vector3[] CalculatePath(Vector3 start, Vector3 end, PolytopialSegmentsStructure polytopialSegmentsStructure);
	}
}