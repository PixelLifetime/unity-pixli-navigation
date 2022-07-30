using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

using Object = UnityEngine.Object;

namespace PixLi
{
	//TODO: Heuristics will make A* go into closed environments(CE) like house for example if destination point is behind that CE. You can run a flood fill algo in there, separate into environments and when searching for neighbour segment check if it's in the CE, if destination is not in that CE, there is no reason to enqueue these segments, so it will skip going inside and find another path. What defines CE is up to you, it could be 2+ walls etc.

	[CreateAssetMenu(fileName = "[AStar Pathfinder]", menuName = "[Pathfinding]/[AStar Pathfinder]")]
	public class AStarPathfinder : Pathfinder
	{
		//? One important thing to remember that heuristic should always be bigger than your max segment cost. So if you max cost of segment is 100, then you need to multiply heuristic by 100 to make it work. Otherwise it will 
		//TODO: Make heuristic and other calculations, that affect speed and optimization, modular.
		private float Heuristic(Segment a, Segment b)
		{
			//// Manheaten distance.
			//float x = Mathf.Abs(a.WorldPosition.x - b.WorldPosition.x);
			//float z = Mathf.Abs(a.WorldPosition.z - b.WorldPosition.z);

			//return x + z;

			//// Diagonal stuff.
			//float x = Mathf.Abs(a.WorldPosition.x - b.WorldPosition.x);
			//float z = Mathf.Abs(a.WorldPosition.z - b.WorldPosition.z);

			//if (x > z)
			//	return 14 * z + 10 * (x - z);

			//return 14 * x + 10 * (z - x);

			//return Mathf.Abs(((GridSegment)a).X - ((GridSegment)b).X) + Mathf.Abs(((GridSegment)a).Z - ((GridSegment)b).Z);

			return 0.0f;
		}

		private float Distance(Segment a, Segment b)
		{
			//float x = Mathf.Abs(a.WorldPosition.x - b.WorldPosition.x);
			//float z = Mathf.Abs(a.WorldPosition.z - b.WorldPosition.z);

			//if (x > z)
			//	return 14 * z + 10 * (x - z);

			//return 14 * x + 10 * (z - x);

			//return Mathf.Abs(((GridSegment)a).X - ((GridSegment)b).X) + Mathf.Abs(((GridSegment)a).Z - ((GridSegment)b).Z);

			return 0.0f;
		}

#if (SHAPES_URP || SHAPES_HDRP) && DEBUG_PATHFINDER
		[SerializeField] private PathfinderVisualizer _pathfinderVisualizer;
#endif

		//TODO: A* doesn't necessary needs to work with segments. Make it more modular.
		public Vector3[] CalculatePath(Segment startSegment, Segment endSegment, PolytopialSegmentsStructure polytopialSegmentsStructure, out float cost, float maxCost)
		{
#if (SHAPES_URP || SHAPES_HDRP) && DEBUG_PATHFINDER
			this._pathfinderVisualizer = polytopialSegmentsStructure.PathfinderVisualizer;
#endif

			//Debug.Log("startSegment: " + startSegment.LocalPosition);
			//Debug.Log("endSegment: " + endSegment.LocalPosition);

			if (startSegment == null)
			{
				cost = 0;
				return new Vector3[0];
			}

			//TODO: Maybe not create these each function call? HUH?!
			PriorityQueue<Segment> frontier = new PriorityQueue<Segment>(
				capacity: polytopialSegmentsStructure._RelativeBounds.size.x * polytopialSegmentsStructure._RelativeBounds.size.y * polytopialSegmentsStructure._RelativeBounds.size.z
			);

			frontier.Enqueue(startSegment, 0.0f, 0.0f);

			Dictionary<int, Segment> backtracking = new Dictionary<int, Segment>(frontier._Capacity)
			{
				[startSegment.Id] = startSegment
			};

			//Dictionary<int, int> segmentsCost = new Dictionary<int, int>(frontier._Capacity)
			//{
			//	[startSegment.Id] = 0
			//};

			Dictionary<int, float> costMemoization = new Dictionary<int, float>(frontier._Capacity)
			{
				[startSegment.Id] = 0.0f
			};

#if (SHAPES_URP || SHAPES_HDRP) && DEBUG_PATHFINDER
			List<PathfinderVisualizer.Data> visualizerData = new List<PathfinderVisualizer.Data>(64);
#endif

			Segment current;

			while (frontier.Count_ > 0)
			{
				current = frontier.Dequeue();

				//? Dunno if it's better to compare references. But I guess ids should be faster.
				if (current.Id == endSegment.Id)
					break;

				Segment[] neighbourSegments = polytopialSegmentsStructure.GetNeighbours(segment: current);
				//Debug.Log($"LEN LEN: {neighbourSegments.Length}");

				for (int a = 0; a < neighbourSegments.Length; a++)
				{
					Segment next = neighbourSegments[a];

					//? You would add weight/movement cost of neighbour tile, like mud, water, grass, sand, road etc...
					//float newCost = cost[current.Id];

					//int newSegmentsCost = segmentsCost[current.Id] + polytopialSegmentsStructure._SegmentCostMap.GetCost(next);
					float newCost = costMemoization[current.Id] + polytopialSegmentsStructure._SegmentCostMap.GetCost(next) + this.Distance(current, next);

					//!? This check is questionable, need profiling and benchmarks.
					if (next.Id == endSegment.Id)
					{
						costMemoization[next.Id] = newCost;
						backtracking[next.Id] = current;
						goto BacktrackingProcess;
					}

					if (next.GetMemoizationValue())
						continue;

					//Debug.Log($"Current: {current};\nCurrent cost: {cost[current.Id]};\nNeighbour: {next};\nNew cost to neighbour segment: {newCost};\nHeuristic: {this.Heuristic(a: endSegment, b: next)};");

					if (!costMemoization.ContainsKey(next.Id) || newCost < costMemoization[next.Id]) //!? #1
					{
						//if (cost.ContainsKey(next.Id))
						//	Debug.LogError($"Neighbour: {next}, Old cost: {cost[next.Id]};");

						costMemoization[next.Id] = newCost;

						//? Heuristic adds to priority because it determines if this neighbour segment is closer to the end segment and in case cost was lowered it should be checked before other segments that have higher cost.
						//? The lower the priority is the better chances for this segment. Because we have `#1` check it's either when (segment hasn't ever been visited) or (cost is lower now, so we update its priority).

						float heuristic = this.Heuristic(a: endSegment, b: next);

						//Debug.Log($"Neighbour: {next}, With heuristic (priority): {priority}");

						if (frontier.Contains(item: next))
							frontier.Update(item: next, priority: newCost + heuristic);
						else
							frontier.Enqueue(item: next, priority: newCost + heuristic, heuristic: heuristic);

						//? If cost was lower we are reasigning the backtracking node to improve path and making neighbour segment match this new lower cost.
						backtracking[next.Id] = current;

#if (SHAPES_URP || SHAPES_HDRP) && DEBUG_PATHFINDER
						visualizerData.Add(new PathfinderVisualizer.Data(next.WorldPosition, Quaternion.identity, newCost, this.Heuristic(a: endSegment, b: next)));
#endif
					}
				}
			}
			BacktrackingProcess:

			////? Don't take into path the last node. Start and last can be returned separately, this improves time in certain cituations if last segment is hard to reach. But optimization is questionable, needs benchmarking.
			if (!backtracking.ContainsKey(endSegment.Id))
			{
				cost = 0;
				return new Vector3[0];
			}

			current = endSegment;

			cost = costMemoization[current.Id];

			List<Vector3> path = new List<Vector3>(frontier._Capacity);

			while (current.Id != startSegment.Id)
			{
				if (costMemoization[current.Id] - Mathf.Epsilon < maxCost)
					path.Add(current.WorldPosition);

				if (!backtracking.TryGetValue(current.Id, out current))
				{
					cost = 0;
					return new Vector3[0];
				}
			}

			// Add start segment.
			path.Add(current.WorldPosition);

			path.Reverse();

			//for (int a = 0; a < path.Count; a++)
			//{
			//	Debug.Log($"path {a}: {path[a]}");
			//}

#if (SHAPES_URP || SHAPES_HDRP) && DEBUG_PATHFINDER
			this._pathfinderVisualizer.Visualize(data: visualizerData.ToArray());
#endif

			return path.ToArray();
		}

		public override Vector3[] CalculatePath(Segment startSegment, Segment endSegment, PolytopialSegmentsStructure polytopialSegmentsStructure, out float cost) => this.CalculatePath(
			startSegment: startSegment,
			endSegment: endSegment,
			polytopialSegmentsStructure: polytopialSegmentsStructure,
			out cost,
			maxCost: float.MaxValue
		);

		public override Vector3[] CalculatePath(Segment startSegment, Segment endSegment, PolytopialSegmentsStructure polytopialSegmentsStructure, float maxCost) => this.CalculatePath(
			startSegment: startSegment,
			endSegment: endSegment,
			polytopialSegmentsStructure: polytopialSegmentsStructure,
			out float cost,
			maxCost: maxCost
		);
	}
}