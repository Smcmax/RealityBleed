using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class QuantitySelector : Selectable {

    [Tooltip("The minimum the quantity can go (inclusive)")]
    public int m_minimumQuantity = 1;

    [Tooltip("The maximum the quantity can go (inclusive)")]
    public int m_maximumQuantity = 99;

    [HideInInspector] public int m_quantity;
    private bool m_xPressed = false;

    void Update() {
        if(EventSystem.current.currentSelectedGameObject == gameObject) {
            float xAxis = MenuHandler.Instance.m_handlingPlayer.GetAxisRaw("UIMoveX");

            if(xAxis != 0 && !m_xPressed) {
                m_xPressed = true;

                if(xAxis > 0) Add();
                else if(xAxis < 0) Subtract();
            } else if(Mathf.Abs(xAxis) < 0.01f) m_xPressed = false;
        }
    }

    public void Add() {
        if(m_quantity + 1 <= m_maximumQuantity) m_quantity++;
    }

    public void Subtract() {
        if(m_quantity - 1 >= m_minimumQuantity) m_quantity--;
    }
}
