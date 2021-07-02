using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//using Debug = ?Uni

namespace PixLi
{
	public class TestPathfinding : MonoBehaviour
	{
		[SerializeField] private Transform _start;
		public Transform _Start => this._start;

		[SerializeField] private Transform _end;
		public Transform _End => this._end;

		private List<GameObject> _gameObjects = new List<GameObject>();

		[SerializeField] private Pathfinder _pathfinder;
		public Pathfinder _Pathfinder => this._pathfinder;

		[SerializeField] private PolytopialSegmentsStructure _polytopialSegmentsStructure;
		public PolytopialSegmentsStructure _PolytopialSegmentsStructure => this._polytopialSegmentsStructure;

		private void Update()
		{
			for (int a = 0; a < this._gameObjects.Count; a++)
			{
				Object.Destroy(this._gameObjects[a]);
			}

			this._gameObjects.Clear();

			Vector3[] path = this._pathfinder.CalculatePath(
				start: this._start.position,
				end: this._end.position,
				polytopialSegmentsStructure: this._polytopialSegmentsStructure
			);

			for (int a = 0; a < path.Length; a++)
			{
				GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
				go.transform.position = path[a];
				go.GetComponent<MeshRenderer>().sharedMaterial = null;

				this._gameObjects.Add(go);
			}
		}
	}
}