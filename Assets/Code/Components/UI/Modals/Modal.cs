using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public abstract class Modal : MonoBehaviour {

	[Tooltip("The modal's canvas")]
	public Canvas m_canvas;

	[Tooltip("The description shown to the user before they make a choice")]
	public Text m_description;

	[Tooltip("Called when opening the modal")]
	public UnityEvent m_openModalEvent;

	[Tooltip("Called when closing the modal")]
	public UnityEvent m_closeModalEvent;

	[Tooltip("The event to fire when the modal is accepted")]
	public GameEvent m_eventToFireOnSuccess;

	[HideInInspector] public object m_info; // the information stored for the modal's use (item to destroy, ability to unlock, etc.)

	public void OpenModal() {
		m_openModalEvent.Invoke();
		m_canvas.gameObject.SetActive(true);
		gameObject.SetActive(true);
	}

	public void CloseModal() {
		m_closeModalEvent.Invoke();
		gameObject.SetActive(false);
		m_canvas.gameObject.SetActive(false);
	}

	public abstract void AcceptModal();
	public abstract void DeclineModal();
}
