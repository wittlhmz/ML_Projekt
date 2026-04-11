using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using Unity.VisualScripting;


public class CatAgentHard : Agent
{
    public RadialSpawnManager barrelManager;
    public BarrelBuffManager buffManager;
    public AutoFireController autoFireController;
    public BeamSkillManager skillManager;
    private float episodeStartTime;
    private int maxBarrelsObserved = 6;
    private float maxHpScale = 300f;
    private float maxResistScale = 210f;


    void Start()
    {
        autoFireController = FindObjectOfType<AutoFireController>();
        barrelManager = FindObjectOfType<RadialSpawnManager>();
        buffManager = FindObjectOfType<BarrelBuffManager>();
        skillManager = FindObjectOfType<BeamSkillManager>();
    }

    public override void Initialize()
    {
        //StartCoroutine(WaitForGameManager());
        GameManager.Instance.OnAllBarrelsDestroyed += HandleAllBarrelsDestroyed;
        GameManager.Instance.OnEarlySkillbuild += HandleEarlyGameBuild;
        GameManager.Instance.OnReward += AddReward;
    }

    public override void OnEpisodeBegin()
    {
        GameManager.Instance.episodeStartTime = Time.time;
        autoFireController.currentX = 0;
        autoFireController.currentY = 1;
        autoFireController.currentBeamIndex = 0;
        GameManager.Instance.remainingBarrels = 75;
        GameManager.Instance.peakDps = 0;
        GameManager.Instance.destroyedBarrels = 0;
        GameManager.Instance.gold = 0;
        GameManager.Instance.hitTracker = 0;
        GameManager.Instance.rightColour = 0;
        GameManager.Instance.shotTracker = 0;
        barrelManager.ClearBarrels();
        GameManager.Instance.earlyBuildLogged = false;

        //--- Skills zurücksetzen ---
        skillManager.redLevel = 0;
        skillManager.blueLevel = 0;
        skillManager.greenLevel = 0;
        skillManager.purpleLevel = 0;
        GameManager.Instance.activeSkillPoints = 0;

        //--- Buffs zurücksetzen ---

        GameManager.Instance.barrelsUntilBuff = 5;
        buffManager.redResistBonus = 0;
        buffManager.redHpBonus = 0;
        buffManager.blueResistBonus = 0;
        buffManager.blueHpBonus = 0;
        buffManager.greenResistBonus = 0;
        buffManager.greenHpBonus = 0;
        buffManager.purpleResistBonus = 0;
        buffManager.purpleHpBonus = 0;
        buffManager.extraBarrelsSpawned = 0;

        // --- Items zurücksetzen ---
        // Inventar resetten
        ItemManager.Instance.ResetInventory();

        // - Tier 1 -
        barrelManager.maxActiveBarrels = 1;
        skillManager.baseDamage = 50;
        autoFireController.fireInterval = 0.5f;
        // - Tier 2 -
        skillManager.greenOverdrive = false;
        skillManager.cleave = false;
        GameManager.Instance.goldIncrease = 2;
        // - Tier 3 -
        skillManager.purpleItemBonusLevel = 0;
        skillManager.purpleItemBonusMaxShots = 0;
        skillManager.purpleMultiplier = 1.3f;
        skillManager.redMultiplier = 1;
        skillManager.blueMultiplier = false;

        // Alles für die Statistik

        ItemManager.Instance.itemsBought = 0;
        GameManager.Instance.redShots = 0;
        GameManager.Instance.blueShots = 0;
        GameManager.Instance.purpleShots = 0;
        GameManager.Instance.greenShots = 0;


        //Skill UI

        SkillUI.Instance.UpdateSkillBar("Red", 0);
        SkillUI.Instance.UpdateSkillBar("Blue", 0);
        SkillUI.Instance.UpdateSkillBar("Green", 0);
        SkillUI.Instance.UpdateSkillBar("Purple", 0);

        BuffUI.Instance.UpdateBuffUI(0, 0, 0, 0, 0, 0, 0, 0, 0, 4);

    }

    // Trackt die Stats von den ersten 6 Skillpunkten.
    private void HandleEarlyGameBuild()
    {
        Academy.Instance.StatsRecorder.Add("EarlyGame/RedLevel", skillManager.redLevel, StatAggregationMethod.MostRecent);
        Academy.Instance.StatsRecorder.Add("EarlyGame/BlueLevel", skillManager.blueLevel, StatAggregationMethod.MostRecent);
        Academy.Instance.StatsRecorder.Add("EarlyGame/GreenLevel", skillManager.greenLevel, StatAggregationMethod.MostRecent);
        Academy.Instance.StatsRecorder.Add("EarlyGame/PurpleLevel", skillManager.purpleLevel, StatAggregationMethod.MostRecent);
    }
    private void HandleAllBarrelsDestroyed()
    {
        //Stats
        float accuracy = GameManager.Instance.GetHitAccuracy();
        //Genauigkeit von Blitzfarbe
        Academy.Instance.StatsRecorder.Add("Game/ColourAccuracy", accuracy, StatAggregationMethod.MostRecent);
        //PeakDPS
        Academy.Instance.StatsRecorder.Add("Game/PeakDPS", GameManager.Instance.peakDps, StatAggregationMethod.MostRecent);
        //Schussgenauigkeit
        float shotAccuracy = GameManager.Instance.GetShotAccuracy();
        Academy.Instance.StatsRecorder.Add("Game/ShotAccuracy", shotAccuracy, StatAggregationMethod.MostRecent);
        //Zeit
        float episodeDuration = GameManager.Instance.episodeDuration;
        Academy.Instance.StatsRecorder.Add("Game/EpisodeDuration", episodeDuration, StatAggregationMethod.MostRecent);
        //Item-Build als Statistik
        foreach (var item in ItemManager.Instance.availableItems)
        {
            bool owned = ItemManager.Instance.inventory.Contains(item);
            float value = owned ? 1f : 0f;
            Academy.Instance.StatsRecorder.Add($"ItemBuild/{item.name}", value, StatAggregationMethod.MostRecent);
        }
        // Wie viele Items gekauft wurden
        Academy.Instance.StatsRecorder.Add("Game/#Items bought", ItemManager.Instance.itemsBought, StatAggregationMethod.MostRecent);
        Academy.Instance.StatsRecorder.Add("Game/#Items in inventory", ItemManager.Instance.inventory.Count, StatAggregationMethod.MostRecent);
        //welcher Blitz wie oft abgeschossen wurde
        float totalShots = GameManager.Instance.shotTracker;

        if (totalShots > 0)
        {
            float redRatio = (float)GameManager.Instance.redShots / totalShots;
            float blueRatio = (float)GameManager.Instance.blueShots / totalShots;
            float greenRatio = (float)GameManager.Instance.greenShots / totalShots;
            float purpleRatio = (float)GameManager.Instance.purpleShots / totalShots;

            Academy.Instance.StatsRecorder.Add("Beams/Red", redRatio, StatAggregationMethod.MostRecent);
            Academy.Instance.StatsRecorder.Add("Beams/Blue", blueRatio, StatAggregationMethod.MostRecent);
            Academy.Instance.StatsRecorder.Add("Beams/Green", greenRatio, StatAggregationMethod.MostRecent);
            Academy.Instance.StatsRecorder.Add("Beams/Purple", purpleRatio, StatAggregationMethod.MostRecent);
        }

        //AddReward(1f);
        //einmaliger Reward
        /*float penalty = episodeDuration / 400f;  
        float reward = Mathf.Max(1f - penalty, 0.01f);
        AddReward(reward);*/

        EndEpisode();
    }

    public override void OnActionReceived(ActionBuffers actions)
    {

        int colorIndex = actions.DiscreteActions[0]; // Branch 1 --> 0=Red, 1=Blue, 2=Green, 3=Purple
        int levelUpChoice = actions.DiscreteActions[1]; // Level-Up: 0=keine Aktion, 1=Red, 2=Blue, 3=Green, 4=Purple
        int buyItemAction = actions.DiscreteActions[2]; // Item-Kauf: 0 = nichts kaufen, 1–9 = Items kaufen
        int sellItemAction = actions.DiscreteActions[3]; // Item-Verkauf: 0 = nichts verkaufen, 1-4 für den Index Item verkaufen

        // Continuous Actions für Richtung
        autoFireController.currentX = actions.ContinuousActions[0];
        autoFireController.currentY = actions.ContinuousActions[1];

        if (colorIndex > 0)
        {
            autoFireController.currentBeamIndex = colorIndex - 1;
        }

        // ---- Level-Ups ----
        switch (levelUpChoice)
        {
            case 1:
                skillManager.LevelUpRed();
                break;
            case 2:
                skillManager.LevelUpBlue();
                break;
            case 3:
                skillManager.LevelUpGreen();
                break;
            case 4:
                skillManager.LevelUpPurple();
                break;
            default:
                // 0 = nichts tun
                break;
        }

        // ---Item kaufen---
        if (buyItemAction > 0) // 0 = kein Kauf
        {
            int buyItemIndex = buyItemAction - 1; // weil Agent 1–9 mapped, Liste aber 0–8
            Item itemToBuy = ItemManager.Instance.availableItems[buyItemIndex];
            ItemManager.Instance.BuyItem(itemToBuy);
            //Alles für die Statistik
            //Welches Item wie oft gekauft wurde
            Academy.Instance.StatsRecorder.Add($"Items/Bought/{itemToBuy.name}", 1, StatAggregationMethod.Sum);
            //Wie viel Gold ausgegeben wurde
            Academy.Instance.StatsRecorder.Add("Items/GoldSpent", itemToBuy.cost, StatAggregationMethod.Sum);
        }

        //--Item verkaufen--
        if (sellItemAction > 0) // 0 = kein Verkauf
        {
            int itemIndex = sellItemAction - 1; // weil Agent 1–9 mapped, Liste aber 0–8
            ItemManager.Instance.SellItemAtIndex(itemIndex);
            //Statistik
            //WIe viele Items verkauft wurden
            Academy.Instance.StatsRecorder.Add($"Items/Sold", 1, StatAggregationMethod.Sum);
        }
        //Sparse: AddReward(-0.0001f);
        AddReward(-1f);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        //89 Insgesamt
        // ---leichte Schwierigkeit --- 6x7 = 52 Obs
        // Farben (4), x und y pos (2) und hp (1)pro barrel.
        AddBarrelObservations(sensor);

        // Anzahl der verbleibenden Barrels für das Spiel
        sensor.AddObservation(GameManager.Instance.GetRemainingBarrelsNormalized());

        // Aktuelle Schussrichtung
        sensor.AddObservation(autoFireController.currentX);
        sensor.AddObservation(autoFireController.currentY);

        // Aktueller Beam-Typ
        sensor.AddOneHotObservation(autoFireController.currentBeamIndex, 4);
        // normalisierter CD vom Schuss
        sensor.AddObservation(autoFireController.timer / autoFireController.fireInterval);
        //normalisierter Schaden
        sensor.AddObservation(GameManager.Instance.dps / 100f);
        //höchster jemals gemachter Schaden
        sensor.AddObservation(GameManager.Instance.peakDps / 100f);

        // --- Mittlere Schwierigkeit --- 16 Obs

        sensor.AddObservation(GameManager.Instance.barrelsUntilBuff / 5f);
        sensor.AddObservation(GameManager.Instance.activeSkillPoints / 12f);

        // - Skill Observations -

        sensor.AddObservation(skillManager.redLevel / 3f);
        sensor.AddObservation(skillManager.blueLevel / 3f);
        sensor.AddObservation(skillManager.greenLevel / 3f);
        sensor.AddObservation(skillManager.purpleLevel / 3f);
        sensor.AddObservation(skillManager.buffedShotsRemaining / 5f);

        // - Barrel Buff Observations -

        sensor.AddObservation(buffManager.redResistBonus / maxResistScale);
        sensor.AddObservation(buffManager.blueResistBonus / maxResistScale);
        sensor.AddObservation(buffManager.greenResistBonus / maxResistScale);
        sensor.AddObservation(buffManager.purpleResistBonus / maxResistScale);
        sensor.AddObservation(buffManager.redHpBonus / maxHpScale);
        sensor.AddObservation(buffManager.blueHpBonus / maxHpScale);
        sensor.AddObservation(buffManager.greenHpBonus / maxHpScale);
        sensor.AddObservation(buffManager.purpleHpBonus / maxHpScale);
        //Diesmal mit maxActiveBarrels wegen Item kaufen
        sensor.AddObservation(barrelManager.maxActiveBarrels / 6f);

        // --- Schwere Schwierigkeit --- 21 Obs
        // - Item Observations -
        AddItemsInInventory(sensor); //macht oneHotObs, z.B. 0.25;0,0,0.5;10000, dann hätte Agent 3 Items an den stellen mit 0.25 = 1 usw.
        //Gold
        sensor.AddObservation(GameManager.Instance.gold / 100f);
        //Inventar
        int itemCount = ItemManager.Instance.inventory.Count;
        sensor.AddObservation(itemCount / 4f);
        //Item-Effekte
        // - Tier 1 -
        sensor.AddObservation(skillManager.baseDamage / 60f);
        sensor.AddObservation(autoFireController.fireInterval);
        // - Tier 2 -
        sensor.AddObservation(skillManager.greenOverdrive ? 1f : 0f); //ternärer operator: Bedingung ? 1 wenn true : 0 wenn false
        sensor.AddObservation(skillManager.cleave ? 1f : 0f);
        sensor.AddObservation(GameManager.Instance.goldIncrease / 3f);
        // - Tier 3 -
        sensor.AddObservation(skillManager.purpleItemBonusLevel / 3f);
        sensor.AddObservation(skillManager.purpleItemBonusMaxShots / 3f);
        sensor.AddObservation(skillManager.purpleMultiplier / 1.7f);
        sensor.AddObservation(skillManager.redMultiplier / 2);
        sensor.AddObservation(skillManager.blueMultiplier ? 1f : 0f);

    }

    int GetColorIndexFromTag(string tag)
    {
        switch (tag)
        {
            case "red": return 0;
            case "blue": return 1;
            case "green": return 2;
            case "purple": return 3;
            default: return -1;
        }
    }
    //Richtung des Barrels als oneHot OBservation
    int GetDirectionIndex(Vector2 dir)
    {
        if (Vector2.Dot(dir, Vector2.up) > 0.9f) return 0;
        if (Vector2.Dot(dir, Vector2.right) > 0.9f) return 1;
        if (Vector2.Dot(dir, Vector2.down) > 0.9f) return 2;
        if (Vector2.Dot(dir, Vector2.left) > 0.9f) return 3;
        return -1;
    }



    private void AddBarrelObservations(VectorSensor sensor)
    {

        // Schleife über alle möglichen Plätze
        for (int i = 0; i < maxBarrelsObserved; i++)
        {
            if (i < barrelManager.activeBarrels.Count && barrelManager.activeBarrels[i] != null)
            {
                Barrel b = barrelManager.activeBarrels[i].GetComponent<Barrel>();
                float[] obs = b.GetObservation();
                sensor.AddObservation(obs);
            }
            else
            {
                // Wenn kein Fass: einfach Nullen füllen
                sensor.AddObservation(new float[7]);
            }
        }
    }


    private void AddItemsInInventory(VectorSensor sensor)
    {
        int totalSlots = ItemManager.Instance.maxInventorySize; 
        float[] items = new float[ItemManager.Instance.availableItems.Count];
        for (int i = 0; i < ItemManager.Instance.availableItems.Count; i++)
        {
            Item item = ItemManager.Instance.availableItems[i];
            int slotIndex = ItemManager.Instance.inventory.IndexOf(item);

            if (slotIndex >= 0)
            {
                
                float normalizedSlot = (slotIndex + 1) / (float)totalSlots;
                items[i] = normalizedSlot;
            }
            else
            {
                // Item nicht im Inventar
                items[i] = 0f;
            }
        }
        sensor.AddObservation(items);
    }
    
    // Spawnpunkte Fass als 0 oder 1

    /*private void AddBarrelPositions(VectorSensor sensor)
    {
        foreach (bool occupied in barrelManager.spawnOccupied)
        {
            sensor.AddObservation(occupied ? 1f : 0f);
        }
    }   
     // Gibt für jedes Fass die Farbe als OneHot-Vector zurück
    private void AddBarrelColors(VectorSensor sensor)
    {
        float[] arr = new float[barrelManager.spawnPoints.Length * 4];
        for (int i = 0; i < barrelManager.spawnPoints.Length; i++)
        {
            if (barrelManager.spawnOccupied[i] && barrelManager.barrelsAtSpawn[i] != null)
            {
                int colorIndex = GetColorIndexFromTag(barrelManager.barrelsAtSpawn[i].tag);
                arr[i * 4 + colorIndex] = 1f;
            }
            else
            {
                // gibt 4 nullen wieder
            }
        }
        sensor.AddObservation(arr);
    }    
    private void AddBarrelHp(VectorSensor sensor)
    {
        float[] hpArr = new float[barrelManager.spawnPoints.Length];
        for (int i = 0; i < barrelManager.spawnPoints.Length; i++)
        {
            if (barrelManager.spawnOccupied[i] && barrelManager.barrelsAtSpawn[i] != null)
            {
                float hpNorm = barrelManager.barrelsAtSpawn[i].currentHP / 
                            (float)barrelManager.barrelsAtSpawn[i].maxHP;
                hpArr[i] = hpNorm;
            }
            else
            {
                hpArr[i] = 0f; // kein Fass an diesem Slot
            }
        }
        sensor.AddObservation(hpArr);
    }
    */
}
