using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Analytics;

public class CatAgent : Agent
{
    public BarrelSpawnManager barrelManager;
    public FireController FireController;

    void Start()
    {
        FireController = FindObjectOfType<FireController>();
        barrelManager = FindObjectOfType<BarrelSpawnManager>();
    }
    public override void Initialize()
    {
        GameManager.Instance.OnAllBarrelsDestroyed += HandleAllBarrelsDestroyed;
        GameManager.Instance.OnReward += AddReward;
    }

    private void HandleAllBarrelsDestroyed()
    {
        // Accuracy
        float accuracy = GameManager.Instance.GetHitAccuracy();
        Academy.Instance.StatsRecorder.Add("Game/ColourAccuracy", accuracy, StatAggregationMethod.MostRecent);

        float shotAccuracy = GameManager.Instance.GetShotAccuracy();
        Academy.Instance.StatsRecorder.Add("Game/ShotAccuracy", shotAccuracy, StatAggregationMethod.MostRecent);

        float episodeDuration = GameManager.Instance.episodeDuration;
        Academy.Instance.StatsRecorder.Add("Game/EpisodeDuration", episodeDuration, StatAggregationMethod.MostRecent);

        // Peak DPS
        //statsRecorder.Add("Game/PeakDPS", GameManager.Instance.peakDps, StatAggregationMethod.MostRecent);

        //einmaliger Reward
        /*float penalty = episodeDuration / 300f;  
        float reward = Mathf.Max(1f - penalty, 0.01f);
        AddReward(reward);*/
        AddReward(1f);
        EndEpisode();
    }

    public override void OnEpisodeBegin()
    {
        //autoFireController.currentDirection = 0;
        FireController.currentBeamIndex = 0;
        GameManager.Instance.remainingBarrels = 50;
        GameManager.Instance.peakDps = 0;
        GameManager.Instance.destroyedBarrels = 0; //dazu da, dass die Variable nicht ins unendliche wächst beim Training
        GameManager.Instance.gold = 0;
        GameManager.Instance.hitTracker = 0;
        GameManager.Instance.rightColour = 0;
        GameManager.Instance.shotTracker = 0;
        GameManager.Instance.episodeStartTime = Time.time;
        GameManager.Instance.redShots = 0;
        GameManager.Instance.blueShots = 0;
        GameManager.Instance.purpleShots = 0;
        GameManager.Instance.greenShots = 0;
    }

    public override void OnActionReceived(ActionBuffers actions)
    {

        int direction = actions.DiscreteActions[0]; // Branch 0 --> 1=Up, 2=Right, 3=Down, 4=Left
        int colorIndex = actions.DiscreteActions[1]; // Branch 1 --> 1=Red, 2=Blue, 3=Green, 4=Purple

        //Agent entscheidet über RIchtung und Farbe des Blitzes
        if (direction > 0)
        {
            FireController.currentDirection = direction - 1; // -1 weil 1=Up, 2=Right etc.   
        }

        if (colorIndex > 0)
        {
            FireController.currentBeamIndex = colorIndex - 1;
        }
        //Sparse: AddReward(-0.0001f);
        //AddReward(-0.01f);

    }

    public override void CollectObservations(VectorSensor sensor)
    {

        // Space Size = 20
        // Positionen
        AddBarrelHp(sensor);

        // Farben
        AddSingleBarrelColor(sensor); // (0,1,0,0)

        // Anzahl der verbleibenden Barrels für das Spiel
        sensor.AddObservation(GameManager.Instance.GetRemainingBarrelsNormalized());

        // Aktuelle Schussrichtung
        sensor.AddOneHotObservation(FireController.currentDirection, 4); 

        // Aktueller Beam-Typ
        sensor.AddOneHotObservation(FireController.currentBeamIndex, 4);
        // normalisierter CD vom Schuss
        sensor.AddObservation(FireController.timer / FireController.fireInterval);
        // normalisierter Schaden
        sensor.AddObservation(GameManager.Instance.dps / 100f);
        // höchster jemals gemachter Schaden
        sensor.AddObservation(GameManager.Instance.peakDps / 100f);
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

    // Spawnpunkte Fass als 0 oder 1
    
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
    
    private void AddSingleBarrelColor(VectorSensor sensor)
    {
        if (barrelManager.activeBarrels.Count > 0)
        {
            GameObject barrelObj = barrelManager.activeBarrels[0]; //erstes Objekt der Liste holen
            Barrel barrel = barrelObj.GetComponent<Barrel>();

            if (barrel != null)
            {
                int colorIndex = GetColorIndexFromTag(barrel.tag);

                float[] oneHot = new float[4];
                oneHot[colorIndex] = 1f;

                sensor.AddObservation(oneHot);   
            }
        }
        else
        {
            // Kein Fass -> alle Nullen
            sensor.AddObservation(new float[4]);
        }
    }

}
