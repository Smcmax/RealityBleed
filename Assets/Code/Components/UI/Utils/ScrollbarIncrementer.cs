using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Button))]
public class ScrollbarIncrementer : MonoBehaviour {

	[Tooltip("The scrollbar to scroll with the button")]
	public Scrollbar m_targetScrollbar;

	[Tooltip("How much of the scrollbar are we scrolling each step")]
	[Range(0, 0.1f)] public float m_step = 0.05f;

	[Tooltip("If this button increments or decrements every click")]
	public bool m_incrementing;

	private Button m_button;
	private Image m_image;
	
	void Start() { 
		m_button = GetComponent<Button>();
		m_image = GetComponent<Image>();
	}

	void Update() {
		if(!m_targetScrollbar.gameObject.activeSelf) {
			m_button.interactable = false;
			m_image.enabled = false;
			
			return;
		}

		m_button.interactable = m_incrementing ? m_targetScrollbar.value <= 0.99 : m_targetScrollbar.value >= 0.01;
		m_image.enabled = m_button.interactable;
	}

	public void Increment() { 
		m_targetScrollbar.value = Mathf.Clamp(m_targetScrollbar.value + m_step, 0, 1);
		m_button.interactable = m_targetScrollbar.value != 1;
		m_image.enabled = m_button.interactable;
	}

	public void Decrement() { 
		m_targetScrollbar.value = Mathf.Clamp(m_targetScrollbar.value - m_step, 0, 1);
		m_button.interactable = m_targetScrollbar.value != 0;
		m_image.enabled = m_button.interactable;
	}

	public IEnumerator IncrementDecrementSequence(bool p_increment) { 
		for(;;) {
			yield return new WaitForSeconds(0.025f);

			if(p_increment) Increment();
			else Decrement();
		}
	} 

	public void OnPointerDown() {
		StartCoroutine("IncrementDecrementSequence", m_incrementing);
	}

	public void OnPointerUp() {
		StopCoroutine("IncrementDecrementSequence");
	}
}
