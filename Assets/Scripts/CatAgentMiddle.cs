using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using System;

public class CatAgentMiddle : Agent
{
    public RadialSpawnManager barrelManager;
    public BarrelBuffManager buffManager;
    public AutoFireController autoFireController;
    public BeamSkillManager skillManager;
    private float episodeStartTime;
    private int maxBarrelsObserved = 5;


    void Start()
    {
        autoFireController = FindObjectOfType<AutoFireController>();
        barrelManager = FindObjectOfType<RadialSpawnManager>();
        buffManager = FindObjectOfType<BarrelBuffManager>();
        skillManager = FindObjectOfType<BeamSkillManager>();
    }
    public override void OnEpisodeBegin()
    {
        GameManager.Instance.episodeStartTime = Time.time;
        autoFireController.currentX = 0;
        autoFireController.currentY = 1;
        autoFireController.currentBeamIndex = 0;
        GameManager.Instance.remainingBarrels = 50;
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

        //Skill UI

        SkillUI.Instance.UpdateSkillBar("Red", 0);
        SkillUI.Instance.UpdateSkillBar("Blue", 0);
        SkillUI.Instance.UpdateSkillBar("Green", 0);
        SkillUI.Instance.UpdateSkillBar("Purple", 0);


    }
    public override void Initialize()
    {
        GameManager.Instance.OnAllBarrelsDestroyed += HandleAllBarrelsDestroyed;
        GameManager.Instance.OnEarlySkillbuild += HandleEarlyGameBuild;
        GameManager.Instance.OnReward += AddReward;
    }

    // Trackt die Stats von den ersten 6 Skillpunkten.
    private void HandleEarlyGameBuild()
    {

        Debug.Log($"EarlyGame build recorded: R={skillManager.redLevel}, B={skillManager.blueLevel}, G={skillManager.greenLevel}, P={skillManager.purpleLevel}");
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
        //Schuss-genauigkeit
        float shotAccuracy = GameManager.Instance.GetShotAccuracy();
        Academy.Instance.StatsRecorder.Add("Game/ShotAccuracy", shotAccuracy, StatAggregationMethod.MostRecent);
        //Zeit
        float episodeDuration = GameManager.Instance.episodeDuration; 
        Academy.Instance.StatsRecorder.Add("Game/EpisodeDuration", episodeDuration, StatAggregationMethod.MostRecent);

        /*einmaliger Reward
        float penalty = episodeDuration / 300f;  
        float reward = Mathf.Max(1f - penalty, 0.01f);
        AddReward(reward);*/
        AddReward(1f);
        EndEpisode();
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        autoFireController.currentX = actions.ContinuousActions[0];
        autoFireController.currentY = actions.ContinuousActions[1];
        int colorIndex = actions.DiscreteActions[0]; // Branch 1 --> 0=Red, 1=Blue, 2=Green, 3=Purple
        int levelUpChoice = actions.DiscreteActions[1]; // 0=keine Aktion, 1=Red, 2=Blue, 3=Green, 4=Purple



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

        //AddReward(-0.0001f); //Sparse
        //AddReward(-0.002f);

    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // ---leichte Schwierigkeit --- 45 obs
        // pos, Farben, HP; 5x7 Obs --> max 5 Aktive Fässer  35
        AddBarrelObservations(sensor);

        // Anzahl der verbleibenden Barrels für das Spiel 1
        sensor.AddObservation(GameManager.Instance.GetRemainingBarrelsNormalized());

        // Aktuelle Schussrichtung 2
        sensor.AddObservation(autoFireController.currentX);
        sensor.AddObservation(autoFireController.currentY);

        // Aktueller Beam-Typ 4
        sensor.AddOneHotObservation(autoFireController.currentBeamIndex, 4);
        // normalisierter CD vom Schuss 1
        sensor.AddObservation(autoFireController.timer / autoFireController.fireInterval);
        //normalisierter Schaden 1
        sensor.AddObservation(GameManager.Instance.dps / 100f);
        //höchster jemals gemachter Schaden 1
        sensor.AddObservation(GameManager.Instance.peakDps / 100f);

        // --- Mittlere Schwierigkeit --- 16 obs

        sensor.AddObservation(GameManager.Instance.barrelsUntilBuff / 5f);
        sensor.AddObservation(GameManager.Instance.activeSkillPoints / 12f);

        // - Skill Observations -

        sensor.AddObservation(skillManager.redLevel / 3f);
        sensor.AddObservation(skillManager.blueLevel / 3f);
        sensor.AddObservation(skillManager.greenLevel / 3f);
        sensor.AddObservation(skillManager.purpleLevel / 3f);
        sensor.AddObservation(skillManager.buffedShotsRemaining / 5f);

        // - Barrel Buff Observations -

        sensor.AddObservation(buffManager.redResistBonus / 100f);
        sensor.AddObservation(buffManager.blueResistBonus / 100f);
        sensor.AddObservation(buffManager.greenResistBonus / 100f);
        sensor.AddObservation(buffManager.purpleResistBonus / 100f);
        sensor.AddObservation(buffManager.redHpBonus / 100f);
        sensor.AddObservation(buffManager.blueHpBonus / 100f);
        sensor.AddObservation(buffManager.greenHpBonus / 100f);
        sensor.AddObservation(buffManager.purpleHpBonus / 100f);
        sensor.AddObservation(buffManager.extraBarrelsSpawned / 5f);

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
                // Wenn kein Fass: mit 0 füllen
                sensor.AddObservation(new float[7]);
            }
        }
    }

    /* Spawnpunkte Fass als 0 oder 1

    private void AddBarrelPositions(VectorSensor sensor)
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
