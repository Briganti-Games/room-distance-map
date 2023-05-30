using System.Collections.Generic;
using UnityEngine;

namespace DungeonAlchemist.TerrainGeneration
{
	public interface ISubdividedTileMap
	{
		Vector2 PieceToMapPos(Vector2i pixel);
		Vector2 MapPosToPiece(Vector2 mapPos);

		IEnumerable<Vector2i> GetSubdividedPiecesOnTile(Vector2i tile);
		IEnumerable<Vector2i> GetTilesTouchingPiece(Vector2i piece);
		bool IsValidPiece(Vector2i piece);

		Vector2i size { get; }
	}
}