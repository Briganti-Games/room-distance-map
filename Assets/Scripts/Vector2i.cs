
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Profiling;

[System.Serializable]
public struct Vector2i : IEquatable<Vector2i>, IComparable<Vector2i>
{
	public delegate bool IsValidTileDelegate(Vector2i tile);

	public int x;
	public int y;

	public static readonly Vector2i MaxValue = new Vector2i(int.MaxValue, int.MaxValue);
	public static readonly Vector2i MinValue = new Vector2i(int.MinValue, int.MinValue);

	public static readonly Vector2i DirLeft = new Vector2i(-1, 0);
	public static readonly Vector2i DirRight = new Vector2i(1, 0);
	public static readonly Vector2i DirDown = new Vector2i(0, -1);
	public static readonly Vector2i DirUp = new Vector2i(0, 1);

	public static readonly Vector2i[] AllDirections = new Vector2i[] { new Vector2i(1, 0), new Vector2i(1, 1), new Vector2i(0, 1), new Vector2i(-1, 1), new Vector2i(-1, 0), new Vector2i(-1, -1), new Vector2i(0, -1), new Vector2i(1, -1) };
	public static readonly Vector2i[] OrthogonalDirections = new Vector2i[] { DirRight, DirUp, DirLeft, DirDown };

	/*public Vector2i() {
		x = 0;
		y = 0;
	}*/

	public static Vector2i zero
	{
		get
		{
			return new Vector2i(0, 0);
		}
	}

	public static Vector2i one
	{
		get
		{
			return new Vector2i(1, 1);
		}
	}

	public int this[int index]
	{
		get => index == 0 ? x : y;
		set
		{
			if (index == 0) x = value;
			else y = value;
		}
	}

	public Vector2i(int[] arr)
	{
		if (arr.Length < 2) throw new Exception($"Array passed to Vector2i must contain at least 2 elements.");
		x = arr[0];
		y = arr[1];
	}

	public Vector2i(string s)
	{
		string[] split = s.Split(',');
		x = int.Parse(split[0]);
		y = int.Parse(split[1]);
	}

	public Vector2i(Vector3 p)
	{
		x = Mathf.RoundToInt(p.x);
		y = Mathf.RoundToInt(p.y);
	}

	public Vector2i(Vector2 p)
	{
		x = Mathf.RoundToInt(p.x);
		y = Mathf.RoundToInt(p.y);
	}

	public Vector2i(int x, int y)
	{
		this.x = x; this.y = y;
	}

	public Vector2i(float x, float y)
	{
		this.x = Mathf.RoundToInt(x);
		this.y = Mathf.RoundToInt(y);
	}

	public Vector2i Left => new Vector2i(x - 1, y);
	public Vector2i Right => new Vector2i(x + 1, y);
	public Vector2i Top => new Vector2i(x, y + 1);
	public Vector2i Bottom => new Vector2i(x, y - 1);

	public override string ToString()
	{
		return x + "," + y;
	}

	public static implicit operator Vector2(Vector2i v)
	{
		return new Vector2(v.x, v.y);
	}

	public static implicit operator Vector3(Vector2i v)
	{
		return new Vector3(v.x, v.y, 0.0f);
	}

	public static Vector2i operator -(Vector2i a, Vector2i b)
	{
		return new Vector2i(a.x - b.x, a.y - b.y);
	}

	public static Vector2i operator +(Vector2i a, Vector2i b)
	{
		return new Vector2i(a.x + b.x, a.y + b.y);
	}
	public static Vector2i operator *(Vector2i a, Vector2i b)
	{
		return new Vector2i(a.x * b.x, a.y * b.y);
	}

	public static Vector2i operator /(Vector2i a, Vector2i b)
	{
		return new Vector2i(a.x / b.x, a.y / b.y);
	}

	public static Vector2i operator *(Vector2i a, int v)
	{
		return new Vector2i(a.x * v, a.y * v);
	}

	public static Vector2i operator /(Vector2i a, int v)
	{
		return new Vector2i(a.x / v, a.y / v);
	}
	public static Vector2i operator -(Vector2i a, int b)
	{
		return new Vector2i(a.x - b, a.y - b);
	}

	public static Vector2i operator +(Vector2i a, int b)
	{
		return new Vector2i(a.x + b, a.y + b);
	}

	public static Vector2i operator *(int a, Vector2i v)
	{
		return new Vector2i(a * v.x, a * v.y);
	}

	public static Vector2i operator /(int a, Vector2i v)
	{
		return new Vector2i(a / v.x, a / v.y);
	}
	public static Vector2i operator -(int a, Vector2i v)
	{
		return new Vector2i(a - v.x, a - v.y);
	}

	public static Vector2i operator +(int a, Vector2i v)
	{
		return new Vector2i(a + v.x, a + v.y);
	}

	public static Vector2i operator -(Vector2i p)
	{
		return new Vector2i(-p.x, -p.y);
	}


	public static Vector2 operator *(Vector2i a, float v)
	{
		return new Vector2(a.x * v, a.y * v);
	}

	public static Vector2 operator /(Vector2i a, float v)
	{
		return new Vector2(a.x / v, a.y / v);
	}
	public static Vector2 operator -(Vector2i a, float b)
	{
		return new Vector2(a.x - b, a.y - b);
	}

	public static Vector2 operator +(Vector2i a, float b)
	{
		return new Vector2(a.x + b, a.y + b);
	}

	public static bool operator ==(Vector2i a, Vector2i b)
	{
		return a.x == b.x && a.y == b.y;
	}

	public static bool operator !=(Vector2i a, Vector2i b)
	{
		return a.x != b.x || a.y != b.y;
	}

	public static Vector2i Min(Vector2i a, Vector2i b)
	{
		return new Vector2i(Mathf.Min(a.x, b.x), Mathf.Min(a.y, b.y));
	}

	public static Vector2i Min(IEnumerable<Vector2i> values)
	{
		if (values.Count() == 0) throw new ArgumentException("Cannot get bottom left of empty list.");
		return values.Aggregate(values.First(), (min, next) => Vector2i.Min(min, next));
	}

	public static Vector2i Max(Vector2i a, Vector2i b)
	{
		return new Vector2i(Mathf.Max(a.x, b.x), Mathf.Max(a.y, b.y));
	}

	public static Vector2i Max(IEnumerable<Vector2i> values)
	{
		if (values.Count() == 0) throw new ArgumentException("Cannot get bottom left of empty list.");
		return values.Aggregate(values.First(), (min, next) => Vector2i.Max(min, next));
	}

	public static Vector2i Clamp(Vector2i v, Vector2i min, Vector2i max)
	{
		return Vector2i.Max(Vector2i.Min(v, max), min);
	}

	public static Vector2i Floor(Vector2 a)
	{
		return new Vector2i(Mathf.FloorToInt(a.x), Mathf.FloorToInt(a.y));
	}

	public static Vector2i Round(Vector2 a)
	{
		return new Vector2i(Mathf.RoundToInt(a.x), Mathf.RoundToInt(a.y));
	}

	public static Vector2i Ceil(Vector2 a)
	{
		return new Vector2i(Mathf.CeilToInt(a.x), Mathf.CeilToInt(a.y));
	}

	public Vector2i Clamp(Vector2i min, Vector2i max)
	{
		return Vector2i.Min(max, Vector2i.Max(min, this));
	}

	public static Vector2i Abs(Vector2i a)
	{
		return a.Abs();
	}

	public Vector2i Abs()
	{
		return new Vector2i(Mathf.Abs(x), Mathf.Abs(y));
	}
	
	public Vector2i Rotate90Degrees(bool positiveDirection = true)
	{
		if (positiveDirection)
		{
			return new Vector2i(-y, x);
		}
		else
		{
			return new Vector2i(y, -x);
		}
	}

	public Vector2i RotateMinus90Degrees()
	{
		return new Vector2i(y, -x);
	}

	public Vector2i Rotate90DegreesTimes(int nTimes, bool positiveDirection = true)
	{
		Vector2i p = this;
		for (int i = 0; i < nTimes; ++i)
		{
			p = p.Rotate90Degrees(positiveDirection);
		}
		return p;
	}

	public static void FromToNonAlloc(Vector2i from, Vector2i to, List<Vector2i> tiles)
	{
		tiles.Clear();
		Vector2i bottomLeft = Vector2i.Min(from, to);
		Vector2i topRight = Vector2i.Max(from, to);
		for (int x = bottomLeft.x; x <= topRight.x; ++x)
		{
			for (int y = bottomLeft.y; y <= topRight.y; ++y)
			{
				tiles.Add(new Vector2i(x, y));
			}
		}
	}

	public static IEnumerable<Vector2i> FromTo(Vector2i from, Vector2i to)
	{
		var _tiles = new List<Vector2i>();
		FromToNonAlloc(from, to, _tiles);
		foreach (Vector2i tile in _tiles) yield return tile;
	}

	public static void FromToMaxExclusiveNonAlloc(Vector2i from, Vector2i to, List<Vector2i> tiles)
	{
		tiles.Clear();
		Vector2i bottomLeft = Vector2i.Min(from, to);
		Vector2i topRight = Vector2i.Max(from, to);
		for (int x = bottomLeft.x; x < topRight.x; ++x)
		{
			for (int y = bottomLeft.y; y < topRight.y; ++y)
			{
				tiles.Add(new Vector2i(x, y));
			}
		}
	}

	public static IEnumerable<Vector2i> FromToMaxExclusive(Vector2i from, Vector2i to)
	{
		var _tiles = new List<Vector2i>();
		FromToMaxExclusiveNonAlloc(from, to, _tiles);
		foreach (Vector2i tile in _tiles) yield return tile;
	}

	public IEnumerable<Vector2i> GetAllNeighbours()
	{
		// return the neighbours counter-clockwise, this might be convenient for someone
		yield return new Vector2i(x + 1, y + 0);
		yield return new Vector2i(x + 1, y + 1);
		yield return new Vector2i(x + 0, y + 1);
		yield return new Vector2i(x - 1, y + 1);
		yield return new Vector2i(x - 1, y + 0);
		yield return new Vector2i(x - 1, y - 1);
		yield return new Vector2i(x + 0, y - 1);
		yield return new Vector2i(x + 1, y - 1);
	}

	public IEnumerable<Vector2i> GetOrthogonalNeighbours()
	{
		// return the neighbours counter-clockwise, this might be convenient for someone
		yield return Right;
		yield return Top;
		yield return Left;
		yield return Bottom;
	}

	public static IEnumerable<Vector2i> GetAllSurroundingTiles(IEnumerable<Vector2i> tiles)
	{
		List<Vector2i> surroundingTiles = new List<Vector2i>();
		foreach (var tile in tiles)
		{
			var neighbours = tile.GetAllNeighbours();
			foreach (var neighbour in neighbours)
			{
				if (!tiles.Contains(neighbour) && !surroundingTiles.Contains(neighbour))
				{
					surroundingTiles.Add(neighbour);
				}
			}
		}

		return surroundingTiles;
	}

	public static IEnumerable<Vector2i> GetAllOrthogonalSurroundingTiles(IEnumerable<Vector2i> tiles)
	{
		List<Vector2i> surroundingTiles = new List<Vector2i>();
		foreach (var tile in tiles)
		{
			var neighbours = tile.GetOrthogonalNeighbours();
			foreach (var neighbour in neighbours)
			{
				if (!tiles.Contains(neighbour) && !surroundingTiles.Contains(neighbour))
				{
					surroundingTiles.Add(neighbour);
				}
			}
		}

		return surroundingTiles;
	}

	public static IEnumerable<Vector2i> GetConcaveCorners(IEnumerable<Vector2i> tiles)
	{
		List<Vector2i> concaveCorners = new List<Vector2i>();

		void InvestigateCorner(Vector2i corner)
		{
			if (concaveCorners.Contains(corner)) return;

			Vector2i bottomLeft = new Vector2i(corner.x - 1, corner.y - 1);
			Vector2i bottomRight = new Vector2i(corner.x - 0, corner.y - 1);
			Vector2i topLeft = new Vector2i(corner.x - 1, corner.y - 0);
			Vector2i topRight = new Vector2i(corner.x - 0, corner.y - 0);

			int nTilesInRoom = 0;
			if (tiles.Contains(bottomLeft)) ++nTilesInRoom;
			if (tiles.Contains(bottomRight)) ++nTilesInRoom;
			if (tiles.Contains(topLeft)) ++nTilesInRoom;
			if (tiles.Contains(topRight)) ++nTilesInRoom;

			// concave!
			if (nTilesInRoom == 1)
			{
				concaveCorners.Add(corner);
			}
		}


		foreach (Vector2i tile in tiles)
		{
			InvestigateCorner(new Vector2i(tile.x + 0, tile.y + 0));
			InvestigateCorner(new Vector2i(tile.x + 1, tile.y + 0));
			InvestigateCorner(new Vector2i(tile.x + 0, tile.y + 1));
			InvestigateCorner(new Vector2i(tile.x + 1, tile.y + 1));
		}

		return concaveCorners;
	}

	public IEnumerable<Vector2i> GetTileCorners()
	{
		yield return new Vector2i(x, y);
		yield return new Vector2i(x, y + 1);
		yield return new Vector2i(x + 1, y);
		yield return new Vector2i(x + 1, y + 1);
	}

	public void GetTileCornersNonAlloc(IList<Vector2i> corners)
	{
		corners[0] = new Vector2i(x, y);
		corners[1] = new Vector2i(x, y + 1);
		corners[2] = new Vector2i(x + 1, y);
		corners[3] = new Vector2i(x + 1, y + 1);
	}

	public static bool IsIdenticalSet(List<Vector2i> list1, List<Vector2i> list2)
	{
		if (list1.Count != list2.Count) return false;
		for (int i = 0; i < list1.Count; ++i)
		{
			if (!list1.Contains(list2[i]) || !list2.Contains(list1[i])) return false;
		}
		return true;
	}

	public static bool IsContiguous(List<Vector2i> tiles)
	{
		HashSet<Vector2i> foundTiles = FindReachableTilesFrom(tiles[0], tiles);
		return foundTiles.Count == tiles.Count;
	}

	private static HashSet<Vector2i> FindReachableTilesFrom(Vector2i startTile, List<Vector2i> allTilesList)
	{
		HashSet<Vector2i> foundTiles = new HashSet<Vector2i>();
		HashSet<Vector2i> allTiles = new HashSet<Vector2i>(allTilesList);
		foundTiles.Add(startTile);
		List<Vector2i> searchTiles = new List<Vector2i>(foundTiles);

		void InvestigateTile(Vector2i tile)
		{
			Profiler.BeginSample("Check Tile");
			bool validTile = !foundTiles.Contains(tile) && allTiles.Contains(tile);
			Profiler.EndSample();
			if (validTile)
			{
				searchTiles.Add(tile);
				foundTiles.Add(tile);
			}
		}

		while (searchTiles.Count > 0)
		{
			Vector2i tile = searchTiles[searchTiles.Count - 1];
			searchTiles.RemoveAt(searchTiles.Count - 1);

			// look at the neighbours
			InvestigateTile(tile.Left);
			InvestigateTile(tile.Right);
			InvestigateTile(tile.Bottom);
			InvestigateTile(tile.Top);
		}

		return foundTiles;
	}

	public static List<List<Vector2i>> GetSeparateTiles(List<Vector2i> tiles)
	{
		List<List<Vector2i>> tileSets = new List<List<Vector2i>>();

		List<Vector2i> remainingTiles = new List<Vector2i>(tiles);

		while (remainingTiles.Count > 0)
		{
			var reachedTiles = FindReachableTilesFrom(remainingTiles[0], remainingTiles);
			remainingTiles.RemoveAll(tile => reachedTiles.Contains(tile));
			tileSets.Add(reachedTiles.ToList());
		}

		return tileSets;
	}

	public static IEnumerable<Vector2i> FloodFill(Vector2i startTile, IsValidTileDelegate IsValidTile)
	{
		// use flood fill to find all connected below-zero tiles with or without floors
		HashSet<Vector2i> visitedTiles = new HashSet<Vector2i>();
		HashSet<Vector2i> tiles = new HashSet<Vector2i>();
		if (!IsValidTile(startTile)) return tiles;
		Queue<Vector2i> remainingTiles = new Queue<Vector2i>();
		remainingTiles.Enqueue(startTile);
		visitedTiles.Add(startTile);
		while (remainingTiles.Count > 0)
		{
			Vector2i tile = remainingTiles.Dequeue();
			tiles.Add(tile);

			// add all below-zero neighbours that are not in a normal room
			foreach (Vector2i candidate in tile.GetAllNeighbours())
			{
				if (visitedTiles.Contains(candidate)) continue;
				visitedTiles.Add(candidate);

				if (!IsValidTile(candidate)) continue;
				remainingTiles.Enqueue(candidate);
			}
		}

		return tiles;
	}

	public static IEnumerable<Vector2i> FloodFillOrthogonally(Vector2i startTile, IsValidTileDelegate IsValidTile)
	{
		// use flood fill to find all connected below-zero tiles with or without floors
		HashSet<Vector2i> visitedTiles = new HashSet<Vector2i>();
		HashSet<Vector2i> tiles = new HashSet<Vector2i>();
		if (!IsValidTile(startTile)) return tiles;
		Queue<Vector2i> remainingTiles = new Queue<Vector2i>();
		remainingTiles.Enqueue(startTile);
		visitedTiles.Add(startTile);
		while (remainingTiles.Count > 0)
		{
			Vector2i tile = remainingTiles.Dequeue();
			tiles.Add(tile);

			// add all below-zero neighbours that are not in a normal room
			foreach (Vector2i dir in Vector2i.OrthogonalDirections)
			{
				Vector2i candidate = tile + dir;
				if (visitedTiles.Contains(candidate)) continue;
				visitedTiles.Add(candidate);

				if (!IsValidTile(candidate)) continue;
				remainingTiles.Enqueue(candidate);
			}
		}

		return tiles;
	}

	public static Vector2 GetCenter(IEnumerable<Vector2i> tiles)
	{
		Vector2 sum = Vector2.zero;
		foreach (var tile in tiles) sum += tile;
		return sum / (float)tiles.Count();
	}

	public bool Equals(Vector2i other)
	{
		return x == other.x && y == other.y;
	}

	/*public override int GetHashCode()
	{
		return x + y << 16;
	}*/

	public override int GetHashCode()
	{
		// https://stackoverflow.com/questions/892618/create-a-hashcode-of-two-numbers
		int yRotated = (y << 16) | (y >> (32 - 16));
		return x ^ yRotated;
	}

	public Vector2i ChangeX(int x)
	{
		return new Vector2i(x, this.y);
	}

	public Vector2i ChangeY(int y)
	{
		return new Vector2i(this.x, y);
	}

	public int CompareTo(Vector2i other)
	{
		if (x == other.x && y == other.y) return 0;
		if (x == other.x) return (y < other.y) ? -1 : 1;
		return (x < other.x) ? -1 : 1;
	}

	public int magnitude
	{
		get
		{
			return Mathf.Abs(x) + Mathf.Abs(y);
		}
	}
}