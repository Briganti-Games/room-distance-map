﻿using DungeonAlchemist.Data;
using PriorityQueue;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Profiling;
using UnityEngine;

namespace DungeonAlchemist.TerrainGeneration
{
	/**
	 * RoomDistanceMap is a helper class used to keep track of how far locations on the map are from rooms on the map.
	 * This is used in many different areas, but one example would be to keep grass on the terrain from spawning
	 * inside a room or near the edges of a room (which would make it overlap with the walls).
	 * 
	 * Each room has a set of tiles (1x1 pieces of the map) that lie in that room.
	 * 
	 * The main issue with this code is that it is too slow, especially on larger maps, or when adding/removing
	 * large rooms.
	 * 
	 * This class exposes exactly one method to its users: GetDistance(mapPos), which return the (estimated) distance
	 * of a location on the map from the room closest to it.
	 **/
	public class RoomDistanceMap
	{
		private class Point : PriorityQueueNode
		{

			private ISubdividedTileMap subdividedMap;

			public Vector2i point { get; }

			public bool hasRoot = false;
			public bool outOfRange = false;
			public Vector2i root;
			public float distance => hasRoot ? Vector2.Distance(subdividedMap.SubdividedGridPointToMapPos(point), subdividedMap.SubdividedGridPointToMapPos(root)) : float.MaxValue;

			public Point(ISubdividedTileMap subdividedMap, Vector2i point)
			{
				this.subdividedMap = subdividedMap;
				this.point = point;
				this.hasRoot = false;
			}

			public override double Priority => -(double)distance;
		}

		private ISubdividedTileMap subdividedMap;

		private HashSet<Vector2i> removedRoots = new HashSet<Vector2i>();

		private HashSet<Vector2i> tilesInRoom = new HashSet<Vector2i>();
		private List<string>[,] roomsPerTile;

		private Vector2i size;
		private float maxDistance;

		private Point[,] points;
		private HeapPriorityQueue<Point> pendingPoints;


		public RoomDistanceMap(IMap map, float maxDistance)
		{
			// divide each tile on the original map into 8x8 pieces so that we have a finer grid to calculate the distance on
			this.subdividedMap = map.Subdivide(8);
			this.maxDistance = maxDistance;
			this.size = subdividedMap.size;

			roomsPerTile = new List<string>[map.size.x, map.size.y];


			// set up the distance map data
			pendingPoints = new HeapPriorityQueue<Point>(size.x * size.y);
			points = new Point[size.x, size.y];
			for (int x = 0; x < size.x; ++x)
			{
				for (int y = 0; y < size.y; ++y)
				{
					Vector2i piece = new Vector2i(x, y);
					Point p = new Point(subdividedMap, piece);
					points[piece.x, piece.y] = p;
				}
			}
		}

		public void Insert(RoomInstanceData record)
		{
			Insert(record.id, record.tiles);
		}

		public void Delete(RoomInstanceData record)
		{
			Delete(record.id, record.tiles);
		}

		public void Update(RoomInstanceData previous, RoomInstanceData updated)
		{
			Update(previous.id, previous.tiles, updated.tiles);
		}

		private void Insert(string id, IEnumerable<Vector2i> record)
		{
			if (record == null) return;

			Profiler.BeginSample("Insert Room/Object");
			foreach (Vector2i tile in record)
			{
				GetRoomsPerTile(tile).Add(id);
			}

			UpdateTiles(record);

			UpdateDistanceMap();
			Profiler.EndSample();
		}

		private void Delete(string id, IEnumerable<Vector2i> record)
		{
			if (record == null) return;

			Profiler.BeginSample("Delete Room/Object");
			removedRoots.Clear();
			foreach (Vector2i tile in record)
			{
				GetRoomsPerTile(tile).Remove(id);
			}

			UpdateTiles(record);

			// first remove all tiles associated with this root - we need to re-explore them!
			RemoveRoots(removedRoots);

			// now update the distance map
			UpdateDistanceMap();
			Profiler.EndSample();
		}

		private void Update(string id, IEnumerable<Vector2i> previous, IEnumerable<Vector2i> updated)
		{
			// first delete the tiles and process that
			Profiler.BeginSample("Update Room/Object");
			if (previous != null)
			{
				removedRoots.Clear();
				foreach (Vector2i tile in previous)
				{
					// it is possible that we created a new room distance map after a resize,
					// and that therefore the old tile is NOT part of the distance map yet.
					if (updated == null || !updated.Contains(tile))
					{
						GetRoomsPerTile(tile).Remove(id);
					}
				}
				UpdateTiles(previous);
				RemoveRoots(removedRoots);
				UpdateDistanceMap();
			}

			// then add new tiles and process those
			if (updated != null)
			{
				foreach (Vector2i tile in updated)
				{
					if (previous == null || !previous.Contains(tile))
					{

						//Debug.Log("Add room " + updated.ToString(database) + " to " + tile);
						GetRoomsPerTile(tile).Add(id);
					}
				}
				UpdateTiles(updated);
				UpdateDistanceMap();
			}
			Profiler.EndSample();
		}

		private void UpdateTiles(IEnumerable<Vector2i> tiles)
		{

			// only delete tiles that have no rooms in them anymore or that do now
			foreach (Vector2i tile in tiles)
			{
				bool inRoom = (GetRoomsPerTile(tile).Count > 0);
				bool wasInRoom = tilesInRoom.Contains(tile);
				if (wasInRoom && !inRoom) DeleteTile(tile);
				else if (!wasInRoom && inRoom) InsertTile(tile);
			}
		}

		private List<string> GetRoomsPerTile(Vector2i tile)
		{
			if (roomsPerTile[tile.x, tile.y] == null) roomsPerTile[tile.x, tile.y] = new List<string>();
			return roomsPerTile[tile.x, tile.y];
		}

		private void InsertTile(Vector2i tile)
		{
			if (tilesInRoom.Contains(tile)) throw new ArgumentException($"Tile {tile} is already in a room.");
			Profiler.BeginSample("Insert Tile");
			tilesInRoom.Add(tile);

			foreach (Vector2i piece in subdividedMap.GetSubdividedGridPointsOnTile(tile))
			{
				Point p = points[piece.x, piece.y];
				p.hasRoot = true;
				p.outOfRange = false;
				p.root = p.point;
				if (!pendingPoints.Contains(p))
				{
					pendingPoints.Enqueue(p);
				}
			}
			Profiler.EndSample();
		}

		private void DeleteTile(Vector2i tile)
		{
			if (!tilesInRoom.Contains(tile)) throw new ArgumentException($"Tile {tile} is not in a room and you are trying to delete it from one!");
			Profiler.BeginSample("Delete Tile");
			tilesInRoom.Remove(tile);

			foreach (Vector2i piece in subdividedMap.GetSubdividedGridPointsOnTile(tile))
			{
				Point p = points[piece.x, piece.y];
				if (!p.hasRoot) continue;

				// see if the piece has any adjacent rooms
				bool isStillTouchingRoom = subdividedMap.GetTilesTouchingSubdividedGridPoint(piece).Any(touchingTile => tilesInRoom.Contains(touchingTile));
				if (isStillTouchingRoom) continue;

				p.hasRoot = false;
				p.outOfRange = false;
				removedRoots.Add(piece);
			}
			Profiler.EndSample();
		}


		private void RemoveRoots(HashSet<Vector2i> roots)
		{
			// nothing changed
			if (removedRoots.Count == 0) return;

			Profiler.BeginSample("Remove Roots");
			// first, remove all roots
			int nRootsRemoved = 0;
			for (int x = 0; x < size.x; ++x)
			{
				for (int y = 0; y < size.y; ++y)
				{
					Point p = points[x, y];
					if (p.hasRoot && roots.Contains(p.root))
					{
						p.hasRoot = false;
						p.outOfRange = false;
						++nRootsRemoved;
					}
				}
			}

			// then, find all points that have a neighbour without root
			int nNewPoints = 0;
			for (int x = 0; x < size.x; ++x)
			{
				for (int y = 0; y < size.y; ++y)
				{
					Point p = points[x, y];
					if (p.outOfRange) continue; // we are out of range - we never update anything beyond this point
					if (p.hasRoot && HasNonRootNeighbour(p))
					{
						pendingPoints.Enqueue(p);
						++nNewPoints;
					}
				}
			}
			Profiler.EndSample();
		}

		private bool HasNonRootNeighbour(Point p)
		{
			for (int dx = -1; dx <= 1; ++dx)
			{
				for (int dy = -1; dy <= 1; ++dy)
				{
					if (dx == dy) continue;
					Vector2i neighbour = p.point + new Vector2i(dx, dy);
					if (!subdividedMap.IsValidSubdividedGridPoint(neighbour)) continue;
					if (!points[neighbour.x, neighbour.y].hasRoot) return true;
				}
			}

			return false;
		}

		public void UpdateDistanceMap()
		{
			Profiler.BeginSample("Update Distance Map");

			// go over all pending points until we are done
			int nProcessed = 0;
			while (pendingPoints.Count > 0)
			{
				++nProcessed;
				Point p = pendingPoints.Dequeue();
				for (int dx = -1; dx <= 1; ++dx)
				{
					for (int dy = -1; dy <= 1; ++dy)
					{
						if (dx == dy) continue;
						ExploreNeighbour(p, p.point + new Vector2i(dx, dy));
					}
				}
			}

			Profiler.EndSample();
		}

		private void ExploreNeighbour(Point parent, Vector2i position)
		{
			if (!subdividedMap.IsValidSubdividedGridPoint(position)) return;

			// calculate the distance of the neighbour from the root, and if it changes, we add it to the queue
			Point candidate = points[position.x, position.y];
			float newDistance = Vector2.Distance(subdividedMap.SubdividedGridPointToMapPos(position), subdividedMap.SubdividedGridPointToMapPos(parent.root));
			if (newDistance < candidate.distance)
			{
				candidate.root = parent.root;
				candidate.hasRoot = true;

				if (newDistance <= maxDistance)
				{
					if (!pendingPoints.Contains(candidate))
					{
						pendingPoints.Enqueue(candidate);
					}
					else
					{
						pendingPoints.UpdatePriority(candidate);
					}
				}
				else
				{
					candidate.outOfRange = true;
				}
			}
		}

		public float GetDistance(Vector2 mapPos)
		{
			Vector2 piece = subdividedMap.MapPosToSubdividedGridPoint(mapPos);
			Vector2i p00 = Vector2i.Floor(piece);
			Vector2i p11 = Vector2i.Min(p00 + 1, size - 1);

			// get the average of all points around this piece
			float distance = (points[p00.x, p00.y].distance + points[p00.x, p11.y].distance + points[p11.x, p00.y].distance + points[p11.x, p11.y].distance) / 4;
			return distance;
		}

		public void DrawGizmos()
		{
			for (int x = 0; x < size.x; ++x)
			{
				for (int y = 0; y < size.y; ++y)
				{
					Vector2i p = new Vector2i(x, y);
					float t = Mathf.InverseLerp(0, 5, GetDistance(p));
					float height = t * 1.0f;
					Vector2 bottomLeft = subdividedMap.SubdividedGridPointToMapPos(p);
					Vector2 topRight = subdividedMap.SubdividedGridPointToMapPos(p + 1);
					Vector2 center = (bottomLeft + topRight) * 0.5f;
					Gizmos.color = Color.Lerp(Color.red, Color.green, t);
					Gizmos.DrawCube(new Vector3(center.x, -1 + height * 0.5f, center.y), new Vector3(topRight.x - bottomLeft.x, height, topRight.y - bottomLeft.y));
				}
			}
		}
	}
}