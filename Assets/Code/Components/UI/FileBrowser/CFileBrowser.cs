using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CFileBrowser : MonoBehaviour {

	private Canvas m_canvas;

	void Start() {
		m_canvas = GetComponentInParent<Canvas>();
	}

}
