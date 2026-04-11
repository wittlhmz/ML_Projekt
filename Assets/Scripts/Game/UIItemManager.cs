using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIItemManager : MonoBehaviour
{
    public static UIItemManager Instance;
    public Image[] inventorySlots;
    public TextMeshProUGUI goldText;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    private void Update()
    {
        if (GameManager.Instance != null && goldText != null)
        {
            goldText.text = "Gold: " + GameManager.Instance.gold;
        }
    }

    public void UpdateInventoryUI(List<Item> inventory)
    {
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if (i < inventory.Count)
            {
                inventorySlots[i].sprite = inventory[i].icon;
                inventorySlots[i].enabled = true;
            }
            else
            {
                inventorySlots[i].sprite = null;
                inventorySlots[i].enabled = false;
            }
        }
    }
}
