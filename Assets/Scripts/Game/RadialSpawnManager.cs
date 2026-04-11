using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadialSpawnManager : MonoBehaviour //Barrelspawnmanager für mittlere und harte Schwierigkeit
{
    public GameObject[] barrelPrefabs;
    public Transform centerPoint; 
    public float radius = 3f;
    public int maxActiveBarrels = 1;
    public float minAngleSeparation = 30f; 
    public List<GameObject> activeBarrels = new List<GameObject>();
    private List<float> activeAngles = new List<float>(); // merkt sich die Winkel

    private void Update()
    {
        if (activeBarrels.Count < maxActiveBarrels)
        {
            SpawnRandomBarrel();
        }
    }

    public void SpawnRandomBarrel()
    {
        if (barrelPrefabs.Length == 0) return;

        // Maximal 50 Versuche, einen freien Winkel zu finden
        for (int attempt = 0; attempt < 50; attempt++)
        {
            float angle = Random.Range(0f, 360f);

            if (IsAngleValid(angle))
            {
                // Position berechnen
                Vector3 spawnPos = centerPoint.position +
                                   new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad), 0) * radius;

                // Fass instanziieren
                GameObject prefab = barrelPrefabs[Random.Range(0, barrelPrefabs.Length)];
                GameObject barrel = Instantiate(prefab, spawnPos, Quaternion.identity);


                activeBarrels.Add(barrel);
                activeAngles.Add(angle);
                return;
            }
        }
    }

    private bool IsAngleValid(float newAngle)
    {
        foreach (float angle in activeAngles)
        {
            float diff = Mathf.Abs(Mathf.DeltaAngle(newAngle, angle)); 
            if (diff < minAngleSeparation)
                return false;
        }
        return true;
    }

    public void OnBarrelDestroyed(GameObject barrel)
    {
        int index = activeBarrels.IndexOf(barrel);
        if (index >= 0)
        {
            activeBarrels.RemoveAt(index);
            activeAngles.RemoveAt(index);
        }
    }

    public void SetMaxActiveBarrels(int newMax)
    {
        maxActiveBarrels = newMax;
    }

    public void ClearBarrels()
    {
        foreach (GameObject barrel in activeBarrels)
        {
            if (barrel != null)
            {
                Destroy(barrel);
            }
        }
        activeBarrels.Clear();
        activeAngles.Clear();
        maxActiveBarrels = 1;
    }
}
