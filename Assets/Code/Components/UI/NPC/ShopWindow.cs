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
    private InventoryLoader m_shopLoader;
    private InventoryLoader m_playerLoader;

    private void Init() {
        m_shopLoader = GetComponent<InventoryLoader>();
        m_playerLoader = transform.Find("PlayerInventory").GetComponent<InventoryLoader>();
    }

    public void Setup(DialogController p_controller, Inventory p_inventory, string p_title) {
        Init();

        m_controller = p_controller;
        m_shopLoader.m_inventory = p_inventory;
        m_playerLoader.m_inventory = p_controller.m_interactor.m_inventory;

        gameObject.SetActive(true);
        m_openedWindows.Add(this);

        EventSystem.current.SetSelectedGameObject(gameObject);

        RectTransform playerRect = m_playerLoader.GetComponent<RectTransform>();
        RectTransform shopRect = m_shopLoader.GetComponent<RectTransform>();
        Vector3 playerPos = playerRect.localPosition;
        Vector3 shopPos = shopRect.localPosition;
        float playerHeight = playerRect.sizeDelta.y;
        float shopHeight = shopRect.sizeDelta.y;

        playerPos.y = -playerHeight / 2 - shopHeight / 2 + m_borderSize / 2;
        shopPos.y = shopHeight / 2;

        playerRect.localPosition = playerPos;
        shopRect.localPosition = shopPos;

        List<UIItem> combinedInvs = new List<UIItem>(p_inventory.m_uiItems);
        combinedInvs.AddRange(p_controller.m_interactor.m_inventory.m_uiItems);

        foreach(UIItem item in combinedInvs) {
            item.m_blockItemBehaviours = true;
            item.UnbindPickup();
            item.OnPickup += Sell;
        }
    }

    public void Sell(object sender, EventArgs e) {
        Debug.Log("sell");
    }
}
