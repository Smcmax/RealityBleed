using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class QuantitySelector : Selectable {

    [Tooltip("The text displaying the quantity")]
    public TMP_Text m_quantityText;

    [Tooltip("The minimum the quantity can go (inclusive)")]
    public int m_minimumQuantity = 1;

    [Tooltip("The maximum the quantity can go (inclusive)")]
    public int m_maximumQuantity = 99;

    [HideInInspector] public int m_quantity;
    [HideInInspector] public ShopWindow m_window;
    private float m_xLastPress = 0f;

    protected override void OnEnable() {
        base.OnEnable();

        m_quantity = m_minimumQuantity;
        UpdateText();
    }

    void Update() {
        if(EventSystem.current.currentSelectedGameObject == gameObject) {
            float xAxis = MenuHandler.Instance.m_handlingPlayer.GetAxisRaw("UIMoveX");

            if(xAxis != 0 && Time.time - m_xLastPress >= 0.2f) {
                m_xLastPress = Time.time;

                if(xAxis > 0) Add();
                else if(xAxis < 0) Subtract();
            } else if(Mathf.Abs(xAxis) <= 0.01f) m_xLastPress = 0f;
        }
    }

    public void Add() {
        if(m_quantity + 1 <= m_maximumQuantity) m_quantity++;

        UpdateText();
    }

    public void Subtract() {
        if(m_quantity - 1 >= m_minimumQuantity) m_quantity--;

        UpdateText();
    }

    public void UpdateText() {
        m_quantityText.text = m_quantity.ToString();
        m_window.UpdateSellPrice();
    }
}
