using System.Collections.Generic;
using UnityEngine;

namespace DungeonAlchemist.TerrainGeneration
{
	public interface ISubdividedTileMap
	{
		Vector2 SubdividedGridPointToMapPos(Vector2i pixel);
		Vector2 MapPosToSubdividedGridPoint(Vector2 mapPos);

		IEnumerable<Vector2i> GetSubdividedGridPointsOnTile(Vector2i tile);
		IEnumerable<Vector2i> GetTilesTouchingSubdividedGridPoint(Vector2i piece);
		bool IsValidSubdividedGridPoint(Vector2i piece);

		Vector2i size { get; }
	}
}