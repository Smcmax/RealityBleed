using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class ObjectPooler : MonoBehaviour {

	[Tooltip("The amount of objects to instantiate on awake, automatically resizes if it can't meet demand")]
	[Range(0, 2500)] public int m_size;

	[HideInInspector] public List<GameObject> m_pool;
	[HideInInspector] public List<GameObject> m_activeObjects;

	[Tooltip("The object to pool")]
	public GameObject m_object;

	void Awake() {
		if(GameObject.Find(gameObject.name) != gameObject) return;

		SceneManager.sceneLoaded += OnSceneLoaded;

		m_pool = new List<GameObject>();
		m_activeObjects = new List<GameObject>();

		for(int i = 0; i < m_size; ++i) {
			GameObject obj = Instantiate(m_object, transform);
			obj.SetActive(false);

			m_pool.Add(obj);
		}
	}

	public GameObject Get() {
		if(m_pool.Count == 0) {
			m_size++;

			GameObject newObj = Instantiate(m_object, transform);
			newObj.SetActive(false);

			return newObj;
		}

		GameObject obj = m_pool[0];

		m_pool.Remove(obj);
		m_activeObjects.Add(obj);

		return obj;
	}

	public virtual void Remove(GameObject p_obj) {
		if(p_obj.transform.childCount > 0)
			for(int i = 0; i < p_obj.transform.childCount; i++)
				Destroy(p_obj.transform.GetChild(i).gameObject);

		p_obj.transform.position = Vector3.zero;
		p_obj.SetActive(false);

		m_pool.Add(p_obj);
		m_activeObjects.Remove(p_obj);
	}

	void OnSceneLoaded(Scene p_scene, LoadSceneMode p_mode) {
		Debug.Log("Destroying all active projectiles...");

		foreach(GameObject active in new List<GameObject>(m_activeObjects))
			Remove(active);
	}
}
