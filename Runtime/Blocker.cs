using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PixLi
{
	public class Blocker : MonoBehaviour
	{
		[SerializeField] private ExperimentalSegmentCostRelation _experimentalSegmentCostRelation;
		public ExperimentalSegmentCostRelation _ExperimentalSegmentCostRelation => this._experimentalSegmentCostRelation;

		[SerializeField] private float _cost = 1.0f;
		public float _Cost => this._cost;

		private void Awake()
		{
			this._experimentalSegmentCostRelation._SegmentPositions.Add(new ExperimentalSegmentCostRelation.Data(this.transform.position, this._cost));
		}

#if UNITY_EDITOR
		private void OnDrawGizmos()
		{
			Gizmos.color = Color.black;
			Gizmos.DrawCube(this.transform.position, Vector3.one);
		}
#endif
	}
}