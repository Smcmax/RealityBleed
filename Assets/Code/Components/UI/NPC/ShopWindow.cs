using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class ShopWindow : MonoBehaviour {

    public static List<ShopWindow> m_openedWindows = new List<ShopWindow>();

    [Tooltip("The panel's border size")]
    public float m_borderSize;

    private DialogController m_controller;
    private InventoryLoader m_shopLoader;
    private InventoryLoader m_playerLoader;
    private GameObject m_tradePanel;
    private GameObject m_npcTitlePanel;
    private UIItem m_tradeItemDisplay;
    private TMP_Text m_itemNameDisplay;
    private QuantitySelector m_quantitySelector;
    private Button m_tradeAcceptButton;
    private TMP_Text m_priceText;

    private void Init() {
        m_shopLoader = GetComponent<InventoryLoader>();
        m_playerLoader = transform.Find("PlayerInventory").GetComponent<InventoryLoader>();
        m_tradePanel = transform.Find("Trade Panel").gameObject;

        m_npcTitlePanel = transform.Find("NPC Inventory Title Panel").gameObject;
        m_npcTitlePanel.transform.Find("Title").GetComponent<TMP_Text>().text = m_controller.m_npc.gameObject.name;

        m_tradeItemDisplay = m_tradePanel.transform.Find("Item Background").Find("Item").GetComponent<UIItem>();
        m_tradeItemDisplay.m_blockItemBehaviours = true;
        m_tradeItemDisplay.UnbindPickup();

        m_itemNameDisplay = m_tradePanel.transform.Find("Item Name").GetComponent<TMP_Text>();

        m_quantitySelector = m_tradePanel.transform.Find("QuantitySelection").GetComponent<QuantitySelector>();
        m_quantitySelector.m_window = this;

        m_tradeAcceptButton = m_tradePanel.transform.Find("TradeAcceptButton").GetComponent<Button>();
        m_priceText = m_tradeAcceptButton.transform.Find("Sizer").Find("Price Text").GetComponent<TMP_Text>();
    }

    public void Setup(DialogController p_controller, Inventory p_inventory, string p_title) {
        m_controller = p_controller;

        Init();

        m_shopLoader.m_inventory = p_inventory;
        m_playerLoader.m_inventory = p_controller.m_interactor.m_inventory;

        gameObject.SetActive(true);
        m_openedWindows.Add(this);

        EventSystem.current.SetSelectedGameObject(gameObject);

        RectTransform playerRect = m_playerLoader.GetComponent<RectTransform>();
        RectTransform shopRect = m_shopLoader.GetComponent<RectTransform>();
        RectTransform tradeRect = m_tradePanel.GetComponent<RectTransform>();
        RectTransform npcTitleRect = m_npcTitlePanel.GetComponent<RectTransform>();
        Vector3 playerPos = playerRect.localPosition;
        Vector3 shopPos = shopRect.localPosition;
        Vector3 tradePos = tradeRect.localPosition;
        Vector3 npcTitlePos = npcTitleRect.localPosition;
        float playerHeight = playerRect.sizeDelta.y;
        float shopHeight = shopRect.sizeDelta.y;

        playerPos.y = -playerHeight / 2f - shopHeight / 2f + m_borderSize / 2f;
        shopPos.x = -shopRect.sizeDelta.x / 6f; // moves everything
        shopPos.y = shopHeight / 2f;
        tradePos.x = shopRect.sizeDelta.x / 2f + tradeRect.sizeDelta.x / 2f;
        tradePos.y = -(shopHeight + playerHeight) / 2f + shopHeight / 2f;
        npcTitlePos.y = shopHeight / 2f + npcTitleRect.sizeDelta.y / 2f - m_borderSize * 2f;

        playerRect.localPosition = playerPos;
        shopRect.localPosition = shopPos;
        tradeRect.localPosition = tradePos;
        npcTitleRect.localPosition = npcTitlePos;

        List<UIItem> combinedInvs = new List<UIItem>(p_inventory.m_uiItems);
        combinedInvs.AddRange(p_controller.m_interactor.m_inventory.m_uiItems);

        foreach(UIItem item in combinedInvs) {
            item.m_blockItemBehaviours = true;
            item.UnbindPickup();
            item.OnPickup += Trade;
        }

        m_tradeAcceptButton.onClick.AddListener(delegate { AcceptTrade(); });
    }

    public void Trade(object sender, EventArgs e) {
        Item selected = ((UIItem) sender).m_item;

        m_tradeItemDisplay.m_item = selected;
        m_tradeItemDisplay.UpdateInfo();
        m_tradeItemDisplay.transform.GetComponentInChildren<TextMeshProUGUI>().enabled = false;

        m_itemNameDisplay.text = Game.m_languages.GetLine(selected.m_item.GetDisplayName());
        m_itemNameDisplay.color = selected.m_item.m_nameColor.Value;

        m_quantitySelector.m_minimumQuantity = 1;
        m_quantitySelector.m_quantity = 1;
        m_quantitySelector.m_maximumQuantity = selected.m_amount;

        m_quantitySelector.UpdateText();

        UpdateSellPrice();

        if(GetOtherInventory().IsFull()) m_tradeAcceptButton.enabled = false;
        else m_tradeAcceptButton.enabled = true;
    }

    public void AcceptTrade() {
        // TODO: check player/npc money, then make the transaction
        Item selected = m_tradeItemDisplay.m_item;
        Item[] takenItems = selected.m_inventory.TakeAll(selected, m_quantitySelector.m_quantity);

        foreach(Item taken in takenItems)
            GetOtherInventory().Add(taken); // maybe check how many were actually taken and price according to that
    }

    public void UpdateSellPrice() {
        Item item = m_tradeItemDisplay.m_item;

        if(item != null && item.m_item) {
            m_priceText.text = (item.m_item.GetSellPrice(item.m_inventory.m_entity) * m_quantitySelector.m_quantity).ToString();
            m_priceText.color = ConstantColors.GREEN.GetColor(); // change based on coins left
        } else {
            m_priceText.text = "0";
            m_priceText.color = ConstantColors.WHITE.GetColor();
        }
    }
    public Inventory GetOtherInventory() {
        return m_tradeItemDisplay.m_item.m_inventory.m_entity.m_npc == m_controller.m_npc ?
               m_playerLoader.m_inventory : m_shopLoader.m_inventory;
    }
}
