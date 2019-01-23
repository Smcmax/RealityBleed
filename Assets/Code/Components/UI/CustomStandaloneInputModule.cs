using UnityEngine;
using UnityEngine.EventSystems;

public class CustomStandaloneInputModule : StandaloneInputModule {

	public GameObject GetGameObjectUnderPointer(int p_pointerId) { 
		PointerEventData lastPointer = GetLastPointerEventData(p_pointerId);

		if(lastPointer != null) return lastPointer.pointerCurrentRaycast.gameObject;

		return null;
	}

	public GameObject GetGameObjectUnderPointer() { 
		return GetGameObjectUnderPointer(kMouseLeftId);
	}
}
