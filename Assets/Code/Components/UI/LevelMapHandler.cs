using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class LevelMapHandler : MonoBehaviour {

	[Tooltip("The tilemap representing the walls")]
	public Tilemap m_wallsTilemap;

	[Tooltip("The tilemap representing the ground")]
	public Tilemap m_groundTilemap;

	[Tooltip("The material used to color the map")]
	public Material m_mapMaterial;

	[Tooltip("Color used for the walls")]
	public Color m_wallsColor;

	[Tooltip("RuntimeSet of entities allowed to remove the fog around them")]
	public EntityRuntimeSet m_fogRemovers;

	[Tooltip("The radius around the removers where the fog will be removed")]
	[Range(0, 10)] public int m_removalRadius;

	private Tilemap m_levelMapWalls;
	private Tilemap m_levelMapGround;

	void Awake() {
		if(!m_levelMapWalls)
			m_levelMapWalls = CopyAndColorTilemap("LevelMapWalls", m_wallsTilemap, m_wallsColor);
		if(!m_levelMapGround)
			m_levelMapGround = CopyAndColorTilemap("LevelMapGround", m_groundTilemap, m_wallsColor);

		foreach(Entity entity in m_fogRemovers.m_items) {
			MapDiscoverer discoverer = entity.gameObject.AddComponent<MapDiscoverer>();
			discoverer.m_tilemap = m_levelMapGround;
			discoverer.m_removalRadius = m_removalRadius;
		}
	}

	private Tilemap CopyAndColorTilemap(string p_newTilemapName, Tilemap p_original, Color p_color) {
		GameObject newTilemap = new GameObject(p_newTilemapName);

		newTilemap.transform.parent = p_original.transform.parent;
		newTilemap.layer = LayerMask.NameToLayer("LevelMap");

		Tilemap levelMapTilemap = (Tilemap) p_original.Copy(typeof(Tilemap), newTilemap);
		TilemapRenderer tileRenderer = (TilemapRenderer) p_original.Copy(typeof(TilemapRenderer), newTilemap);

		Material mat = new Material(m_mapMaterial);
		mat.SetColor(Shader.PropertyToID("_Color"), p_color);
		tileRenderer.material = mat;

		List<Vector3Int> positions = new List<Vector3Int>();
		List<TileBase> tiles = new List<TileBase>();

		for(int y = p_original.origin.y; y < (p_original.origin.y + p_original.size.y); y++) {
			for(int x = p_original.origin.x; x < (p_original.origin.x + p_original.size.x); x++) {
				Vector3Int position = new Vector3Int(x, y, 0);
				TileBase tile = p_original.GetTile(position);

				if(tile != null) {
					positions.Add(position);
					tiles.Add(tile);
				}
			}
		}

		levelMapTilemap.SetTiles(positions.ToArray(), tiles.ToArray());

		foreach(Vector3Int pos in positions) {
			levelMapTilemap.SetTileFlags(pos, TileFlags.None);
			levelMapTilemap.SetColor(pos, p_color);
		}

		return levelMapTilemap;
	}
}