using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelSpawnManager : MonoBehaviour //Spawnt die Barrels in der leichten Schwierigkeit
{
    public GameObject[] barrelPrefabs;
    public Transform[] spawnPoints;
    public Barrel[] barrelsAtSpawn;
    public bool[] spawnOccupied; //welche spawnpunkte noch frei sind
    public int maxActiveBarrels = 1;
    public List<GameObject> activeBarrels = new List<GameObject>();

    void Start()
    {
        spawnOccupied = new bool[spawnPoints.Length];
        barrelsAtSpawn = new Barrel[spawnPoints.Length];
    }

    private void Update()
    {
        if (activeBarrels.Count < maxActiveBarrels)
        {
            SpawnRandomBarrel();
        }
    }

    public void SpawnRandomBarrel()
    {
        if (barrelPrefabs.Length == 0 || spawnPoints.Length == 0) return;

        // alle freien Spawnpunkte sammeln
        List<int> freeIndices = new List<int>();
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            if (!spawnOccupied[i])
                freeIndices.Add(i);
        }

        if (freeIndices.Count == 0) return; // kein Platz frei

        // zufälligen freien Index wählen
        int chosenIndex = freeIndices[Random.Range(0, freeIndices.Count)];
        Transform spawnPoint = spawnPoints[chosenIndex];

        GameObject prefab = barrelPrefabs[Random.Range(0, barrelPrefabs.Length)];
        GameObject barrel = Instantiate(prefab, spawnPoint.position, Quaternion.identity);

        Barrel barrelScript = barrel.GetComponent<Barrel>();
        if (barrelScript != null)
        {
            //barrelScript.spawnManager = this;
            barrelScript.spawnIndex = chosenIndex; // merken, wo das fass gespawnt wurde
            barrelsAtSpawn[chosenIndex] = barrelScript;
        }

        activeBarrels.Add(barrel);
        spawnOccupied[chosenIndex] = true; // Spawnpunkt als belegt markieren
    }

    public void OnBarrelDestroyed(GameObject barrel, int spawnIndex)
    {
        if (activeBarrels.Contains(barrel))
        {
            activeBarrels.Remove(barrel);
        }

        if (spawnIndex >= 0 && spawnIndex < spawnOccupied.Length)
        {
            spawnOccupied[spawnIndex] = false; // Spawnpunkt wieder freigeben
            
            if (barrelsAtSpawn[spawnIndex] != null && barrelsAtSpawn[spawnIndex].gameObject == barrel)
            {
                barrelsAtSpawn[spawnIndex] = null;
            }
        }
    }

    public void SetMaxActiveBarrels(int newMax)
    {
        maxActiveBarrels = newMax;
    }
    public void ClearBarrels()
    {
        // Alle Barrels zerstören
        foreach (GameObject barrel in activeBarrels)
        {
            if (barrel != null)
            {
                Destroy(barrel);
            }
        }

        activeBarrels.Clear();

        // Spawnpunkte zurücksetzen
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            spawnOccupied[i] = false;
            barrelsAtSpawn[i] = null;
        }
    }
}
