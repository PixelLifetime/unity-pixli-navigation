using System.Collections;
using System.Collections.Generic;
using Priority_Queue;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "[Hex Grid Pathfinder]", menuName = "Pathfinding/[Hex Grid Pathfinder]")]
public class HexGridPathfinder : Pathfinder<HexGrid>
{
	//[SerializeField] private Material _material;

	//private List<GameObject> _gameObjects = new List<GameObject>();

	private float Heuristic(Vector3 a, Vector3 b) => Vector3.Distance(a, b);

	public override Vector3[] CalculatePath(Vector3 start, Vector3 end, HexGrid grid)
	{
		Vector3Int startCell = grid._Tilemap.WorldToCell(worldPosition: start);
		//Debug.Log("startCell: " + startCell);
		//Debug.Log("start: " + grid._Tilemap.CellToWorld(cellPosition: startCell));
		//Debug.Log("start center: " + grid._Tilemap.GetCellCenterWorld(position: startCell));
		Vector3Int endCell = grid._Tilemap.WorldToCell(worldPosition: end);

		Vector3 endCenter = grid._Tilemap.GetCellCenterWorld(position: endCell);

		GroundTile startTile = grid._Tilemap.GetTile<GroundTile>(position: startCell);
		if (startTile == null)
			return new Vector3[0];

		HexTile instantiatedHexTile = grid._Tilemap.GetInstantiatedObject(position: startCell).GetComponent<HexTile>();

		FastPriorityQueue<HexNode> frontier = new FastPriorityQueue<HexNode>(grid._Tilemap.size.x * grid._Tilemap.size.y * grid._Tilemap.size.z);

#if UNITY_EDITOR
		instantiatedHexTile._HexNode.Queue = null;
#endif
		frontier.Enqueue(instantiatedHexTile._HexNode, startTile.movementWeight);

		Dictionary<Vector3Int, Vector3Int> backtracking = new Dictionary<Vector3Int, Vector3Int>(grid._Tilemap.size.x);
		backtracking[startCell] = startCell;

		Dictionary<Vector3Int, float> cost = new Dictionary<Vector3Int, float>(grid._Tilemap.size.x);
		cost[startCell] = 0.0f;

		//for (int a = 0; a < this._gameObjects.Count; a++)
		//{
		//	Object.Destroy(this._gameObjects[a]);
		//}

		//this._gameObjects.Clear();

		Vector3Int currentCell;

		while (frontier.Count > 0)
		{
			Vector3 current = frontier.Dequeue().HexTile.transform.position;
			currentCell = grid._Tilemap.WorldToCell(worldPosition: current);

			if (currentCell == endCell)
				break;

			Vector3[] neighbours = grid.GetNeighbours(node: current);

			for (int a = 0; a < neighbours.Length; a++)
			{
				Vector3Int neighbourCell = grid._Tilemap.WorldToCell(worldPosition: neighbours[a]);
				GroundTile neighbourTile = grid._Tilemap.GetTile<GroundTile>(position: neighbourCell);

				if (neighbourTile == null || !neighbourTile.walkable)
					continue;

				float newCost = cost[currentCell] + neighbourTile.movementWeight;

				if (!backtracking.ContainsKey(neighbourCell) || newCost < cost[neighbourCell])
				{
					cost[neighbourCell] = newCost + this.Heuristic(a: endCenter, b: neighbours[a]);

					instantiatedHexTile = grid._Tilemap.GetInstantiatedObject(position: neighbourCell).GetComponent<HexTile>();

#if UNITY_EDITOR
					instantiatedHexTile._HexNode.Queue = null;
#endif
					if (frontier.Contains(instantiatedHexTile._HexNode))
						frontier.UpdatePriority(instantiatedHexTile._HexNode, newCost);
					else
						frontier.Enqueue(instantiatedHexTile._HexNode, newCost);

					backtracking[neighbourCell] = currentCell;

					//GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
					//go.transform.position = neighbours[a];
					//go.GetComponent<MeshRenderer>().sharedMaterial = this._material;

					//this._gameObjects.Add(go);
				}
			}
		}

		currentCell = endCell;

		List<Vector3> path = new List<Vector3>(32);

		while (currentCell != startCell)
		{
			path.Add(grid._Tilemap.GetCellCenterWorld(position: currentCell));

			if (!backtracking.TryGetValue(currentCell, out currentCell))
				return new Vector3[0];
		}

		path.Reverse();

		//for (int a = 0; a < path.Count; a++)
		//{
		//	Debug.Log($"path {a}: {path[a]}");
		//}

		return path.ToArray();
	}
}