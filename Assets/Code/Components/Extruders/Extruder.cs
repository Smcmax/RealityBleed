using System.Collections.Generic;
using UnityEngine;

public abstract class Extruder : MonoBehaviour {

	[Tooltip("The mesh's material")]
	public Material m_meshMaterial;

	[Tooltip("How thick should the extrusion be (starting from extrusionHeight going both sides)")]
	[Range(0, 10)] public float m_extrusionDepth;

	[Tooltip("At what height should the extrusion start")]
	[Range(-10, 10)] public float m_extrusionHeight;

	[Tooltip("What offset should the extrusion have relative to the parent?")]
	public Vector2 m_offset;

	[Tooltip("The mesh's layer")]
	public string m_layer;

	[Tooltip("Whether or not the extrusion should happen automatically on start")]
	public bool m_extrudeOnStart;

	void Start() {
		if(!m_extrudeOnStart || !CanExtrude()) return; // if low or under, we don't want to do this

		Extrude();
	}

	public bool CanExtrude() { 
		return QualitySettings.GetQualityLevel() > 1;
	}

	public abstract void Extrude();

	protected GameObject Create3DMeshObject(Vector2[] p_points, Transform p_parent, string p_name) {
		Triangulator triangulator = new Triangulator(p_points);
		int[] tris = triangulator.Triangulate();
		Mesh mesh = new Mesh();
		Vector3[] vertices = new Vector3[p_points.Length * 2];

		for(int i = 0; i < p_points.Length; i++) {
			vertices[i].x = p_points[i].x;
			vertices[i].y = p_points[i].y;
			vertices[i].z = m_extrusionHeight - m_extrusionDepth; // front vertex
			vertices[i + p_points.Length].x = p_points[i].x;
			vertices[i + p_points.Length].y = p_points[i].y;
			vertices[i + p_points.Length].z = m_extrusionHeight + m_extrusionDepth; // back vertex    
		}

		int[] triangles = new int[tris.Length * 2 + p_points.Length * 6];
		int count_tris = 0;

		for(int i = 0; i < tris.Length; i += 3) {
			// front vertices
			triangles[i] = tris[i];
			triangles[i + 1] = tris[i + 1];
			triangles[i + 2] = tris[i + 2];
		}

		count_tris += tris.Length;

		for(int i = 0; i < tris.Length; i += 3) {
			// back vertices
			triangles[count_tris + i] = tris[i + 2] + p_points.Length;
			triangles[count_tris + i + 1] = tris[i + 1] + p_points.Length;
			triangles[count_tris + i + 2] = tris[i] + p_points.Length;
		}

		count_tris += tris.Length;

		for(int i = 0; i < p_points.Length; i++) {
			// triangles around the perimeter of the object
			int n = (i + 1) % p_points.Length;

			triangles[count_tris] = i;
			triangles[count_tris + 1] = n;
			triangles[count_tris + 2] = i + p_points.Length;
			triangles[count_tris + 3] = n;
			triangles[count_tris + 4] = n + p_points.Length;
			triangles[count_tris + 5] = i + p_points.Length;

			count_tris += 6;
		}

		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.RecalculateNormals();
		mesh.RecalculateBounds();

		return CreateGameObjectFromMesh(mesh, p_parent, p_name);
	}

	protected GameObject CreateGameObjectFromMesh(Mesh p_mesh, Transform p_parent, string p_name) {
		GameObject meshObject = new GameObject(p_name);
		MeshRenderer renderer = (MeshRenderer)meshObject.AddComponent(typeof(MeshRenderer));
		MeshFilter filter = meshObject.AddComponent(typeof(MeshFilter)) as MeshFilter;

		meshObject.transform.SetParent(p_parent);
		meshObject.layer = LayerMask.NameToLayer(m_layer);
		meshObject.transform.position += (Vector3)m_offset;
		renderer.material = m_meshMaterial;
		renderer.receiveShadows = false;
		renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
		filter.mesh = p_mesh;

		return meshObject;
	}
}

class ClockwiseComparer : IComparer<Vector2> {

	private Vector2 m_origin;

	public ClockwiseComparer(Vector2 p_origin) {
		m_origin = p_origin;
	}

	public int Compare(Vector2 p_first, Vector2 p_second) {
		return IsClockwise(p_first, p_second, m_origin);
	}

	public static int IsClockwise(Vector2 p_first, Vector2 p_second, Vector2 p_origin) {
		if(p_first == p_second) return 0;

		Vector2 firstOffset = p_first - p_origin;
		Vector2 secondOffset = p_second - p_origin;

		float angle1 = Mathf.Atan2(firstOffset.x, firstOffset.y);
		float angle2 = Mathf.Atan2(secondOffset.x, secondOffset.y);

		if(angle1 < angle2) return -1;
		if(angle1 > angle2) return 1;

		// Check to see which point is closest
		return (firstOffset.sqrMagnitude < secondOffset.sqrMagnitude) ? -1 : 1;
	}
}
