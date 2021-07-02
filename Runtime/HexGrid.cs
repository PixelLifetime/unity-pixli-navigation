using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace PixLi
{
	public class HexGrid : MonoBehaviourSingleton<HexGrid>
	{
		public static readonly Vector3[] Directions =
		{
		new Vector3(-0.5f, 0.0f, 0.75f), // NW
		new Vector3(0.5f, 0.0f, 0.75f), // NE
		new Vector3(1.0f, 0.0f, 0.0f), // E
		new Vector3(0.5f, 0.0f, -0.75f), // SE
		new Vector3(-0.5f, 0.0f, -0.75f), // SW
		new Vector3(-1.0f, 0.0f, 0.0f), // W
	};

		[SerializeField] private Tilemap _tilemap;
		public Tilemap _Tilemap => this._tilemap;

		public Vector3[] GetNeighbours(Vector3 node)
		{
			Vector3[] neighbours = new Vector3[Directions.Length];

			Vector3Int nodeCell = this._tilemap.WorldToCell(worldPosition: node);
			Vector3 nodeCenter = this._tilemap.GetCellCenterWorld(position: nodeCell);

			for (int a = 0; a < Directions.Length; a++)
			{
				neighbours[a] = nodeCenter + new Vector3(
					x: Directions[a].x * this._tilemap.cellSize.x,
					y: Directions[a].y * this._tilemap.cellSize.z,
					z: Directions[a].z * this._tilemap.cellSize.y
				);

				//Debug.Log($"neighbour {a}: {neighbours[a]}");
			}

			return neighbours;
		}

		[SerializeField] private AStarPathfinder _hexGridPathfinder;
		public AStarPathfinder _HexGridPathfinder => this._hexGridPathfinder;

		public Vector3[] CalculatePath(Vector3 start, Vector3 end) => this._hexGridPathfinder.CalculatePath(start: start, end: end, null);
	}
}