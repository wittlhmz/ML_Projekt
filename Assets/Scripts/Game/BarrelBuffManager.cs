using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelBuffManager : MonoBehaviour
{

    //Resistenzen
    public int resistBonus = 35;
    public int redResistBonus = 0;
    public int blueResistBonus = 0;
    public int greenResistBonus = 0;
    public int purpleResistBonus = 0;

    // HP Bonus
    public int hpBonus = 50;
    public int redHpBonus = 0;
    public int blueHpBonus = 0;
    public int greenHpBonus = 0;
    public int purpleHpBonus = 0;
    //Variablen für den max. Barrelspawn
    public float barrelChance = 0.33f;
    public int extraBarrelsSpawned = 0; 
    public int maxExtraBarrels = 4;
    //public BarrelSpawnManager spawnManager;
    public RadialSpawnManager spawnManager;


    void Start()
    {
        spawnManager = FindObjectOfType<RadialSpawnManager>();
    }
    public void ApplyRandomBuff()
    {
        // Zufällig eine Fassfarbe wählen
        int type = Random.Range(0, 8);

        switch (type)
        {
            case 0: 
                redResistBonus += resistBonus;
                break;
            case 1: 
                blueResistBonus += resistBonus;
                break;
            case 2: 
                greenResistBonus += resistBonus;
                break;
            case 3: 
                purpleResistBonus += resistBonus;
                break;
            case 4: 
                redHpBonus += hpBonus;
                break;
            case 5: 
                blueHpBonus += hpBonus;
                break;
            case 6: 
                greenHpBonus += hpBonus;
                break;
            case 7: 
                purpleHpBonus += hpBonus;
                break;
        }
        // Chance auf zusätzliches Fass
        if (extraBarrelsSpawned < maxExtraBarrels)
        {
            if (Random.value < barrelChance)
            {
                spawnManager.maxActiveBarrels++;
                extraBarrelsSpawned++;
            }
        }

        BuffUI.Instance.UpdateBuffUI(
            redHpBonus, blueHpBonus, greenHpBonus, purpleHpBonus,
            redResistBonus, blueResistBonus, greenResistBonus, purpleResistBonus,
            extraBarrelsSpawned, maxExtraBarrels
        );
    }
    
    // Getter für Barrel beim Spawnen
    public int GetHpBonus(Barrel.BarrelType type)
    {
        return type switch
        {
            Barrel.BarrelType.Red => redHpBonus,
            Barrel.BarrelType.Blue => blueHpBonus,
            Barrel.BarrelType.Green => greenHpBonus,
            Barrel.BarrelType.Purple => purpleHpBonus,
            _ => 0
        };
    }
    // getter für Resistenzen in der Schadensberechnung im Barrel
    public int GetResistBonus(Barrel.BarrelType type)
    {
        return type switch
        {
            Barrel.BarrelType.Red => redResistBonus,
            Barrel.BarrelType.Blue => blueResistBonus,
            Barrel.BarrelType.Green => greenResistBonus,
            Barrel.BarrelType.Purple => purpleResistBonus,
            _ => 0
        };
    }
}
