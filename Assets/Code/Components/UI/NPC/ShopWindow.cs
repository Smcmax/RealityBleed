using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class ShopWindow : MonoBehaviour {

    public static List<ShopWindow> m_openedWindows = new List<ShopWindow>();

    [Tooltip("The panel's border size")]
    public float m_borderSize;

    private DialogController m_controller;
    private InventoryLoader m_loader;

    private void Init() {
        m_loader = GetComponent<InventoryLoader>();
    }

    public void Setup(DialogController p_controller, Inventory p_inventory, string p_title) {
        Init();

        m_controller = p_controller;
        m_loader.m_inventory = p_inventory;

        gameObject.SetActive(true);
        m_openedWindows.Add(this);

        EventSystem.current.SetSelectedGameObject(gameObject);

        foreach(UIItem item in p_inventory.m_uiItems) {
            item.m_isShopItem = true;
            item.UnbindPickup();
            item.OnPickup += Sell;
        }
    }

    public void Sell(object sender, EventArgs e) {
        Debug.Log("sell");
    }
}
