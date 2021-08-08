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
		public interface IPriorityQueue<T>
		{
			int _Capacity { get; }

			int Count_ { get; }

			void Enqueue(T item, float priority, float heuristic);
			T Dequeue();

			void Update(T item, float priority);
			bool Contains(T item);
		}

		//public class ListPriorityQueue<T> : IPriorityQueue<T>
		//	where T : class
		//{
		//	private List<Data> _items = new List<Data>(256);

		//	public int _Capacity => this._items.Capacity;

		//	public int Count_ => this._items.Count;

		//	public void Enqueue(T item, float priority)
		//	{
		//		this._items.Add(new Data(item, priority));

		//		this._items.Sort((a, b) => a.Priority < b.Priority ? -1 : 1);
		//	}

		//	public T Dequeue()
		//	{
		//		T item = this._items[0].Item;

		//		this._items.RemoveAt(0);

		//		return item;
		//	}

		//	public void Update(T item, float priority)
		//	{
		//		int index = this._items.FindIndex(data => data.Item == item);
		//		this._items[index] = new Data(this._items[index].Item, priority);

		//		this._items.Sort((a, b) => a.Priority < b.Priority ? -1 : 1);
		//	}

		//	public bool Contains(T item) => this._items.FindIndex(data => data.Item == item) > 0;

		//	public struct Data
		//	{
		//		public T Item { get; set; }
		//		public float Priority { get; set; }

		//		public Data(T item, float priority) : this()
		//		{
		//			this.Item = item;
		//			this.Priority = priority;
		//		}
		//	}
		//}

		public class PriorityQueue<T> : IPriorityQueue<T>
			where T : class, PriorityQueue<T>.IItem
		{
			private float[] _heuristics;
			private float[] _priorities;
			private T[] _items;
			public int _Capacity => this._items.Length;

			public int Count_ { get; private set; }

			private void Swap(T a, T b)
			{
				////? DBUG
				//float aPriority = this._priorities[a.Index];
				//float bPriority = this._priorities[b.Index];

				int aIndex = a.Index;
				a.Index = b.Index;
				b.Index = aIndex;

				this._items[a.Index] = a;
				this._items[b.Index] = b;

				//this._priorities[a.Index] = aPriority;
				//this._priorities[b.Index] = bPriority;

				float aPriority = this._priorities[b.Index];
				this._priorities[b.Index] = this._priorities[a.Index];
				this._priorities[a.Index] = aPriority;

				float aHeuristic = this._heuristics[b.Index];
				this._heuristics[b.Index] = this._heuristics[a.Index];
				this._heuristics[a.Index] = aHeuristic;
			}

			private void SortUp(T item)
			{
				int parentItemIndex = (item.Index - 1) / 2;

				while (true)
				{
					T parentItem = this._items[parentItemIndex];

					int priorityComparisonResult = this._priorities[item.Index].CompareTo(this._priorities[parentItem.Index]);

					if (priorityComparisonResult < 0 || (priorityComparisonResult == 0 && this._heuristics[item.Index] < this._heuristics[parentItem.Index]))
						this.Swap(a: item, b: parentItem);
					else
						break;

					parentItemIndex = (item.Index - 1) / 2;
				}
			}

			public void Enqueue(T item, float priority, float heuristic)
			{
				item.Index = this.Count_;
				this._items[item.Index] = item;
				this._priorities[item.Index] = priority;
				this._heuristics[item.Index] = heuristic;

				this.SortUp(item: item);

				++this.Count_;
			}

			public void SortDown(T item)
			{
				while (true)
				{
					int leftChildIndex = item.Index * 2 + 1;
					int rightChildIndex = item.Index * 2 + 2;

					int swapIndex = 0;

					if (leftChildIndex < this.Count_)
					{
						swapIndex = leftChildIndex;

						int priorityComparisonResult = this._priorities[leftChildIndex].CompareTo(this._priorities[rightChildIndex]);

						if (rightChildIndex < this.Count_)
						{
							if (priorityComparisonResult > 0 || (priorityComparisonResult == 0 && this._heuristics[leftChildIndex] > this._heuristics[rightChildIndex]))
							{
								swapIndex = rightChildIndex;
							}
						}

						priorityComparisonResult = this._priorities[item.Index].CompareTo(this._priorities[swapIndex]);

						if (priorityComparisonResult > 0 || (priorityComparisonResult == 0 && this._heuristics[item.Index] > this._heuristics[swapIndex]))
							this.Swap(a: item, b: this._items[swapIndex]);
						else
							return;
					}
					else
						return;
				}
			}

			public T Dequeue()
			{
				T topItem = this._items[0];
				this._items[0] = null;

				--this.Count_;

				if (this.Count_ > 0)
				{
					// Put last item in the top place.
					this._items[0] = this._items[this.Count_];
					this._items[0].Index = 0;

					this._priorities[0] = this._priorities[this.Count_];
					this._heuristics[0] = this._heuristics[this.Count_];

					this.SortDown(this._items[0]);
				}

				return topItem;
			}

			//? Doesn't really update. We only have a scenario of sort up in pathfinding.
			public void Update(T item, float priority)
			{
				this._priorities[item.Index] = priority;

				this.SortUp(item: item);
			}

			public bool Contains(T item) => object.Equals(this._items[item.Index], item);

			public PriorityQueue(int capacity)
			{
				this._items = new T[capacity];
				this._priorities = new float[this._items.Length];
				this._heuristics = new float[this._items.Length];
			}

			public interface IItem
			{
				int Index { get; set; }
			}

			//public struct Data
			//{
			//	T Item { get; set; }
			//	float Priority { get; set; }
			//}
		}

		//? One important thing to remember that heuristic should always be bigger than your max segment cost. So if you max cost of segment is 100, then you need to multiply heuristic by 100 to make it work. Otherwise it will 
		//TODO: Make heuristic and other calculations, that affect speed and optimization, modular.
		private float Heuristic(Segment a, Segment b)
		{
			// Manheaten distance.
			//float x = Mathf.Abs(a.WorldPosition.x - b.WorldPosition.x);
			//float z = Mathf.Abs(a.WorldPosition.z - b.WorldPosition.z);

			//return x + z;

			// Diagonal stuff.
			float x = Mathf.Abs(a.WorldPosition.x - b.WorldPosition.x);
			float z = Mathf.Abs(a.WorldPosition.z - b.WorldPosition.z);

			if (x > z)
				return 14 * z + 10 * (x - z);

			return 14 * x + 10 * (z - x);
		}

		private float Distance(Segment a, Segment b)
		{
			float x = Mathf.Abs(a.WorldPosition.x - b.WorldPosition.x);
			float z = Mathf.Abs(a.WorldPosition.z - b.WorldPosition.z);

			if (x > z)
				return 14 * z + 10 * (x - z);

			return 14 * x + 10 * (z - x);
		}

#if SHAPES_URP || SHAPES_HDRP
		[SerializeField] private PathfinderVisualizer _pathfinderVisualizer;
#endif

		[SerializeField] private ExperimentalSegmentCostRelation _experimentalSegmentCostRelation;

		//TODO: A* doesn't necessary needs to work with segments. Make it more modular.
		public override Vector3[] CalculatePath(Vector3 start, Vector3 end, PolytopialSegmentsStructure polytopialSegmentsStructure)
		{
#if SHAPES_URP || SHAPES_HDRP
			this._pathfinderVisualizer = polytopialSegmentsStructure.PathfinderVisualizer;
#endif
			this._experimentalSegmentCostRelation.Initialize(polytopialSegmentsStructure);

			Segment startSegment = polytopialSegmentsStructure.GetSegment(position: start);
			//Debug.Log("startSegment: " + startSegment);

			Segment endSegment = polytopialSegmentsStructure.GetSegment(position: end);
			//Debug.Log("endSegment: " + endSegment);

			if (startSegment == null)
				return new Vector3[0];

			//TODO: Maybe not create these each function call? HUH?!
			PriorityQueue<Segment> frontier = new PriorityQueue<Segment>(
				capacity: polytopialSegmentsStructure._RelativeBounds.size.x * polytopialSegmentsStructure._RelativeBounds.size.y * polytopialSegmentsStructure._RelativeBounds.size.z
			);

			frontier.Enqueue(startSegment, 0.0f, 0.0f);

			Dictionary<int, Segment> backtracking = new Dictionary<int, Segment>(frontier._Capacity)
			{
				[startSegment.Id] = startSegment
			};

			Dictionary<int, float> cost = new Dictionary<int, float>(frontier._Capacity)
			{
				[startSegment.Id] = 0.0f
			};

#if SHAPES_URP || SHAPES_HDRP
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

					//!? This check is questionable, need profiling and benchmarks.
					if (next.Id == endSegment.Id)
					{
						backtracking[next.Id] = current;
						goto BacktrackingProcess;
					}

					//? You would add weight/movement cost of neighbour tile, like mud, water, grass, sand, road etc...
					//float newCost = cost[current.Id];

					float newCost = cost[current.Id] + this.Distance(current, next) + this._experimentalSegmentCostRelation.GetCost(next);

					//Debug.Log($"Current: {current};\nCurrent cost: {cost[current.Id]};\nNeighbour: {next};\nNew cost to neighbour segment: {newCost};\nHeuristic: {this.Heuristic(a: endSegment, b: next)};");

					if (!cost.ContainsKey(next.Id) || newCost < cost[next.Id]) //!? #1
					{
						//if (cost.ContainsKey(next.Id))
						//	Debug.LogError($"Neighbour: {next}, Old cost: {cost[next.Id]};");

						cost[next.Id] = newCost;

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

#if SHAPES_URP || SHAPES_HDRP
						visualizerData.Add(new PathfinderVisualizer.Data(next.WorldPosition, Quaternion.identity, newCost, this.Heuristic(a: endSegment, b: next)));
#endif
					}
				}
			}
			BacktrackingProcess:

			////? Don't take into path the last node. Start and last can be returned separately, this improves time in certain cituations if last segment is hard to reach. But optimization is questionable, needs benchmarking.
			//if (!backtracking.TryGetValue(endSegment.Id, out current))
			//	return new Vector3[0];

			current = endSegment;

			List<Vector3> path = new List<Vector3>(frontier._Capacity);

			while (current.Id != startSegment.Id)
			{
				path.Add(current.WorldPosition);

				if (!backtracking.TryGetValue(current.Id, out current))
					return new Vector3[0];
			}

			path.Reverse();

			//for (int a = 0; a < path.Count; a++)
			//{
			//	Debug.Log($"path {a}: {path[a]}");
			//}

#if SHAPES_URP || SHAPES_HDRP
			this._pathfinderVisualizer.Visualize(data: visualizerData.ToArray());
#endif

			return path.ToArray();
		}
	}
}