using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

public class LevelMap : MonoBehaviour, IDragHandler, IScrollHandler {

	[Tooltip("The tilemap representing the walls")]
	public Tilemap m_wallsTilemap;

	[Tooltip("The material used to color the map")]
	public Material m_mapMaterial;

	[Tooltip("Color used for the walls")]
	public Color m_wallsColor;
	
	[Tooltip("Color used for the ground")]
	public Color m_groundColor;

	[Tooltip("The camera used to render the map")]
	public Camera m_mapCamera;

	[Tooltip("Inverts the drag's movement")]
	public bool m_invertDrag;

	private Tilemap m_levelMapTilemap;

	void Awake() { 
		//Vector2 parentPosition = m_mapCamera.transform.parent.position;
		//m_mapCamera.transform.position = new Vector3(parentPosition.x, parentPosition.y, m_mapCamera.transform.position.z);
		m_mapCamera.backgroundColor = m_groundColor;

		Vector2 size = GetComponent<RectTransform>().sizeDelta;
		float scale = GetComponentInParent<Canvas>().scaleFactor;
		RenderTexture newRender = new RenderTexture((int) (size.x * scale), (int) (size.y * scale), 24);
		newRender.filterMode = FilterMode.Point;

		m_mapCamera.targetTexture = newRender;
		GetComponent<RawImage>().texture = newRender;

		if (!m_levelMapTilemap) { 
			GameObject newTilemap = new GameObject("LevelMapWalls");

			newTilemap.transform.parent = m_wallsTilemap.transform.parent;
			newTilemap.layer = LayerMask.NameToLayer("LevelMap");

			m_levelMapTilemap = (Tilemap) m_wallsTilemap.Copy(typeof(Tilemap), newTilemap);
			TilemapRenderer tileRenderer = (TilemapRenderer) m_wallsTilemap.Copy(typeof(TilemapRenderer), newTilemap);

			m_mapMaterial.SetColor(Shader.PropertyToID("_Color"), m_wallsColor);
			tileRenderer.material = m_mapMaterial;
			
			List<Vector3Int> positions = new List<Vector3Int>();
			List<TileBase> tiles = new List<TileBase>();

			for(int y = m_wallsTilemap.origin.y; y < (m_wallsTilemap.origin.y + m_wallsTilemap.size.y); y++) { 
				for(int x = m_wallsTilemap.origin.x; x < (m_wallsTilemap.origin.x + m_wallsTilemap.size.x); x++) { 
					Vector3Int position = new Vector3Int(x, y, 0);
					TileBase tile = m_wallsTilemap.GetTile(position);

					if(tile != null) { 
						positions.Add(position);
						tiles.Add(tile);
					}
				}
			}

			m_levelMapTilemap.SetTiles(positions.ToArray(), tiles.ToArray());
			
			foreach(Vector3Int pos in positions) {
				m_levelMapTilemap.SetTileFlags(pos, TileFlags.None);
				m_levelMapTilemap.SetColor(pos, m_wallsColor);
			}
		}
	}

	public void OnDrag(PointerEventData p_eventData) {
		Vector3 delta = p_eventData.delta / 300 * m_mapCamera.orthographicSize;
		if(m_invertDrag) delta = new Vector2(-delta.x, -delta.y);

		m_mapCamera.transform.position += delta;
	}

	public void OnScroll(PointerEventData p_eventData) {
		float size = m_mapCamera.orthographicSize - p_eventData.scrollDelta.y;

		if(size < 1) m_mapCamera.orthographicSize = 1;
		else if(size > 35) m_mapCamera.orthographicSize = 35;
		else m_mapCamera.orthographicSize = size;
	}
}
