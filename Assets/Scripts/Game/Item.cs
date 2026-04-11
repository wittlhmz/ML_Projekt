using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "NewItem", menuName = "Items/Item")]
public class Item : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public int cost;
    public ItemType type;
    public int barrelsDestroyedAtPurchase; //wichtig für Rewards
}
public enum ItemType
{
    // -------- Tier 1 -------- 6 Gold
    ExtraBarrel,         // +1 Fass-Spawn mehr
    BaseDamageUp,        // +20 Base Damage
    FasterAttack,        // -0.2 Attack Speed (schneller feuern)

    // -------- Tier 2 -------- 16 Gold
    GoldPerBarrel,       // +2 Gold pro zerstörtem Fass
    GreenOverdrive,      // Grün ignoriert Resi nicht nur, sondern addiert den Schaden
    CleaveBoost,         // +50% Cleave

    // -------- Tier 3 -------- 30 Gold
    PurpleOverload,      // Lila: +3 Blitze (max. 7) und +40% Schaden
    RedMultiplier,       // Rot Buff *3
    BlueDoubleDamage     // Blau Buff doppelter Schaden

}
