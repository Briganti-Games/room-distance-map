namespace DungeonAlchemist.TerrainGeneration
{
	public interface IMap
	{
		Vector2i size { get; }
		ISubdividedTileMap Subdivide(int nSubdivisions);
	}
}