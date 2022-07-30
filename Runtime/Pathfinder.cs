using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PixLi
{
	public interface IPathfinder
	{
		Vector3[] CalculatePath(Vector3 start, Vector3 end, PolytopialSegmentsStructure polytopialSegmentsStructure, out float cost);
		Vector3[] CalculatePath(Vector3 start, Vector3 end, PolytopialSegmentsStructure polytopialSegmentsStructure);//, int maxCost);
	}

	public abstract class Pathfinder : ScriptableObject, IPathfinder
	{
		public abstract Vector3[] CalculatePath(Segment startSegment, Segment endSegment, PolytopialSegmentsStructure polytopialSegmentsStructure, out float cost);

		public Vector3[] CalculatePath(Vector3 start, Vector3 end, PolytopialSegmentsStructure polytopialSegmentsStructure, out float cost) => this.CalculatePath(
			startSegment: polytopialSegmentsStructure.GetSegment(position: start),
			endSegment: polytopialSegmentsStructure.GetSegment(position: end),
			polytopialSegmentsStructure: polytopialSegmentsStructure,
			out cost
		);

		public Vector3[] CalculatePath(Vector3 start, Vector3 end, PolytopialSegmentsStructure polytopialSegmentsStructure) => this.CalculatePath(
			startSegment: polytopialSegmentsStructure.GetSegment(position: start),
			endSegment: polytopialSegmentsStructure.GetSegment(position: end),
			polytopialSegmentsStructure: polytopialSegmentsStructure,
			out float cost
		);

		public abstract Vector3[] CalculatePath(Segment startSegment, Segment endSegment, PolytopialSegmentsStructure polytopialSegmentsStructure, float maxCost);

		public Vector3[] CalculatePath(Vector3 start, Vector3 end, PolytopialSegmentsStructure polytopialSegmentsStructure, float maxCost) => this.CalculatePath(
			startSegment: polytopialSegmentsStructure.GetSegment(position: start),
			endSegment: polytopialSegmentsStructure.GetSegment(position: end),
			polytopialSegmentsStructure: polytopialSegmentsStructure,
			maxCost: maxCost
		);
	}
}