using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public static ItemManager Instance { get; private set; }
    public List<Item> availableItems; // Items, die gekauft werden können
    public List<Item> inventory = new List<Item>(); // aktuelle Items (max 4), beim Start leer
    private AutoFireController fireController;
    private BeamSkillManager skillManager;
    private RadialSpawnManager spawnManager;
    public int itemsBought = 0;

    public int maxInventorySize = 4;

    private void Start()
    {
        fireController = FindAnyObjectByType<AutoFireController>();
        skillManager = FindAnyObjectByType<BeamSkillManager>();
        spawnManager = FindAnyObjectByType<RadialSpawnManager>();
    }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Update()
    {

        /*float reward = 0f;
        int count = inventory.Count;
        if (count > 0)
        {
            reward = Mathf.Pow(count, 1.1f) / 1500;
        }
        else
        {
            reward = 0;
        }
        
        GameManager.Instance.GrantReward(reward);*/
    }

    public void BuyItem(Item item)
    {
        if (inventory.Count >= maxInventorySize) return;
        if (GameManager.Instance.gold < item.cost) return;
        if (inventory.Contains(item)) return;

        GameManager.Instance.gold -= item.cost;
        // Zeitpunkt des Kaufs speichern
        item.barrelsDestroyedAtPurchase = GameManager.Instance.destroyedBarrels;

        inventory.Add(item);

        ApplyItemEffect(item);
        itemsBought++;
        //Reward
        //float buyReward = (float)item.cost / 500f; Training 1 & 2
        //float buyReward = Mathf.Pow(item.cost, 1.2f) / 400f; //reward^1.2 Training 3
        //GameManager.Instance.GrantReward(buyReward); 
        UIItemManager.Instance.UpdateInventoryUI(inventory);
    }

    public void SellItem(Item item)
    {
        if (inventory.Contains(item))
        {
            inventory.Remove(item);

            RemoveItemEffect(item);
            GameManager.Instance.gold += item.cost / 2; // halbes Gold kommt zurück beim Verkauf
            UIItemManager.Instance.UpdateInventoryUI(inventory);
        }
    }

    public void SellItemAtIndex(int index)
    {
        if (index >= 0 && index < inventory.Count)
        {
            Item item = inventory[index];
            inventory.RemoveAt(index);
            RemoveItemEffect(item);
            GameManager.Instance.gold += item.cost / 2; // halbes Gold zurück

            //int barrelsSincePurchase = GameManager.Instance.destroyedBarrels - item.barrelsDestroyedAtPurchase;
            //float penalty = Mathf.Max(item.cost * 2 - 2 * barrelsSincePurchase, item.cost); //der Malus sind die Item-Kosten, nur wenn nicht amortisiert ist gibts -Reward)
            //float penalty = Mathf.Max(item.cost - GameManager.Instance.goldIncrease * 3* barrelsSincePurchase, 0);
            //float penalty = item.cost - GameManager.Instance.goldIncrease * 3* barrelsSincePurchase;
            //GameManager.Instance.GrantReward(-penalty / 300f);

            UIItemManager.Instance.UpdateInventoryUI(inventory);
        }
    }

    private void ApplyItemEffect(Item item)
    {
        switch (item.type)
        {
            // -------- Tier 1 -------- 5 Gold
            case ItemType.GoldPerBarrel:
                GameManager.Instance.goldIncrease += 1;
                break;
            case ItemType.BaseDamageUp:
                skillManager.baseDamage += 10;
                break;
            case ItemType.ExtraBarrel:
                spawnManager.maxActiveBarrels++;
                break;
            // -------- Tier 2 -------- 15 Gold
            case ItemType.FasterAttack:
                fireController.fireInterval -= 0.15f;
                break;
            case ItemType.GreenOverdrive:
                skillManager.greenOverdrive = true;
                break;
            case ItemType.CleaveBoost:
                skillManager.cleave = true;
                break;

            // -------- Tier 3 -------- 30 Gold
            case ItemType.PurpleOverload:
                skillManager.purpleItemBonusLevel += 3;
                skillManager.purpleItemBonusMaxShots += 3;
                skillManager.purpleMultiplier += 0.7f;
                break;
            case ItemType.RedMultiplier:
                skillManager.redMultiplier = 2;
                break;
            case ItemType.BlueDoubleDamage:
                skillManager.blueMultiplier = true; //*2.5
                break;
        }
    }

    private void RemoveItemEffect(Item item)
    {
        switch (item.type)
        {
            // -------- Tier 1 -------- 5 Gold
            case ItemType.ExtraBarrel:
                spawnManager.maxActiveBarrels--;
                break;
            case ItemType.BaseDamageUp:
                skillManager.baseDamage -= 10;
                break;
            case ItemType.FasterAttack:
                fireController.fireInterval += 0.15f;
                break;

            // -------- Tier 2 -------- 15 Gold

            case ItemType.GoldPerBarrel:
                GameManager.Instance.goldIncrease -= 1;
                break;
            case ItemType.GreenOverdrive:
                skillManager.greenOverdrive = false;
                break;
            case ItemType.CleaveBoost:
                skillManager.cleave = false;
                break;
            // -------- Tier 3 -------- 30 Gold

            case ItemType.PurpleOverload:
                skillManager.purpleItemBonusLevel -= 3;
                skillManager.purpleItemBonusMaxShots -= 3;
                skillManager.purpleMultiplier -= 0.7f;
                break;
            case ItemType.RedMultiplier:
                skillManager.redMultiplier = 1;
                break;
            case ItemType.BlueDoubleDamage:
                skillManager.blueMultiplier = false;
                break;
        }
    }
    public void ResetInventory()
    {
        inventory.Clear();
        UIItemManager.Instance.UpdateInventoryUI(inventory);
    }
}
