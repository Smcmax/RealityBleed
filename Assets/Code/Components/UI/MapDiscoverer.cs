using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;

public class MapDiscoverer : MonoBehaviour {

	[HideInInspector] public int m_removalRadius;
	[HideInInspector] public Tilemap m_tilemap;

	void Awake() { 
		StartCoroutine(Discover());
	}

	private IEnumerator Discover() { 
		while(gameObject.activeSelf) {
			yield return new WaitForSeconds(Constants.FOG_DISCOVERY_UPDATE_RATE);

			Vector3Int cell = m_tilemap.WorldToCell(transform.position);

			for(int x = cell.x - m_removalRadius; x < cell.x + m_removalRadius; x++) {
				for(int y = cell.y - m_removalRadius; y < cell.y + m_removalRadius; y++) {
					Vector3Int targetCell = new Vector3Int(x, y, cell.z);

					if(m_tilemap.HasTile(targetCell)) m_tilemap.SetTile(targetCell, null);
				}
			}
		}
	}
}