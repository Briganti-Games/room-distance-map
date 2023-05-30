using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DungeonAlchemist.Data
{
	public class RoomInstanceData : IEquatable<RoomInstanceData>
	{
		public int Version = 1;

		public string id { get; private set; }
		public List<Vector2i> tiles { get; private set; }


		public Vector2i Min => Vector2i.Min(tiles);
		public Vector2i BottomLeft => Min;
		public Vector2i TopRight => Max + 1;
		public Vector2i Max => Vector2i.Max(tiles);
		public IEnumerable<Vector2i> BoundingBox => Vector2i.FromTo(Min, Max);
		public Vector2 Center => ((Vector2)Min + (Vector2)(Max + 1)) * 0.5f;


		private RoomInstanceData()
		{
		}

		public RoomInstanceData(List<Vector2i> tiles)
		{
			id = new Guid().ToString();
			this.tiles = tiles;
		}

		public RoomInstanceData(RoomInstanceData source)
		{
			this.id = source.id;
			this.tiles = source.tiles;
		}

		public RoomInstanceData Clone()
		{
			return new RoomInstanceData(this);
		}

		public RoomInstanceData AddTiles(IEnumerable<Vector2i> newTiles)
		{
			RoomInstanceData data = Clone();
			data.tiles = new List<Vector2i>(data.tiles.Union(newTiles));
			return data;
		}

		public RoomInstanceData RemoveTiles(IEnumerable<Vector2i> removedTiles)
		{
			RoomInstanceData data = Clone();
			data.tiles = new List<Vector2i>(data.tiles.Except(removedTiles));
			return data;
		}

		public RoomInstanceData AddTile(Vector2i tile)
		{
			RoomInstanceData data = Clone();
			data.tiles = new List<Vector2i>(data.tiles);
			data.tiles.Add(tile);
			return data;
		}

		public RoomInstanceData RemoveTile(Vector2i tile)
		{
			RoomInstanceData data = Clone();
			data.tiles = new List<Vector2i>(data.tiles);
			data.tiles.Remove(tile);
			return data;
		}

		public RoomInstanceData UpdateTiles(IEnumerable<Vector2i> tiles)
		{
			RoomInstanceData data = Clone();
			data.tiles = new List<Vector2i>(tiles);
			return data;
		}

		public override string ToString()
		{
			string fromTo = "";
			if (tiles.Count > 0) fromTo = $" from {Min} to {Max}";
			return $"id {id} and {tiles.Count} tiles" + fromTo;
		}

		public bool Equals(RoomInstanceData other)
		{
			return id == other.id;
		}
	}

}