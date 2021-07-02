using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PixLi
{
	[CreateAssetMenu(fileName = "[ExperimentalSegmentCostRelation]", menuName = "[Pathfinding]/[ExperimentalSegmentCostRelation]")]
	public class ExperimentalSegmentCostRelation : ScriptableObject
	{
		[SerializeField] private PolytopialSegmentsStructure _polytopialSegmentsStructure;
		public PolytopialSegmentsStructure _PolytopialSegmentsStructure => this._polytopialSegmentsStructure;

		private Dictionary<int, float> _segmentId_cost;

		[SerializeField] private List<Data> _segmentPositions;
		public List<Data> _SegmentPositions => this._segmentPositions;

		public float GetCost(Segment segment) => this._segmentId_cost.TryGetValue(segment.Id, out float cost) ? cost : 0.0f;

		public void Initialize(PolytopialSegmentsStructure polytopialSegmentsStructure)
		{
			this._polytopialSegmentsStructure = polytopialSegmentsStructure;

			//Debug.Log(this._polytopialSegmentsStructure);

			this._segmentId_cost = new Dictionary<int, float>();

			for (int a = 0; a < this._segmentPositions.Count; a++)
			{
				Segment segment = this._polytopialSegmentsStructure.GetSegment(this._segmentPositions[a].Position);

				//Debug.Log(segment);

				this._segmentId_cost[segment.Id] = this._segmentPositions[a].Cost;
			}
		}

		[Serializable]
		public struct Data
		{
			public Vector3 Position;
			public float Cost;

			public Data(Vector3 position, float cost)
			{
				this.Position = position;
				this.Cost = cost;
			}
		}
	}
}