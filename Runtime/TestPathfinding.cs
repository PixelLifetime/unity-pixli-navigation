using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPathfinding : MonoBehaviour
{
	[SerializeField] private Transform _start;
	public Transform _Start => this._start;

	[SerializeField] private Transform _end;
	public Transform _End => this._end;

	private List<GameObject> _gameObjects = new List<GameObject>();

	private void Update()
	{
		for (int a = 0; a < this._gameObjects.Count; a++)
		{
			Object.Destroy(this._gameObjects[a]);
		}

		this._gameObjects.Clear();

		Vector3[] path = HexGrid._Instance.CalculatePath(start: this._start.position, end: this._end.position);

		for (int a = 0; a < path.Length; a++)
		{
			GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			go.transform.position = path[a];

			this._gameObjects.Add(go);
		}
	}
}