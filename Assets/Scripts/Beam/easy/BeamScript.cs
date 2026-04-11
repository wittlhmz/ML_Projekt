using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeamScript : MonoBehaviour
{
    public GameObject[] beamPrefabs; // 0 = rot, 1 = blau, usw.
    public Transform firePointUp;
    public Transform firePointRight;
    public Transform firePointDown;
    public Transform firePointLeft;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            
            FireInRandomDirection();
        }
    }

    void FireInRandomDirection()
    {
        int dirIndex = Random.Range(0, 4); // 0 = up, 1 = right, 2 = down, 3 = left
        FireBeam(dirIndex);
    }

    void FireBeam(int direction)
    {
        // Wähle zufälligen Beam
        int prefabIndex = Random.Range(0, beamPrefabs.Length);
        GameObject beamPrefab = beamPrefabs[prefabIndex];

        Transform firePoint = firePointUp; // Default
        Quaternion rotation = Quaternion.identity;

        switch (direction)
        {
            case 0: // Up
                firePoint = firePointUp;
                rotation = Quaternion.identity;
                break;

            case 1: // Right
                firePoint = firePointRight;
                rotation = Quaternion.Euler(0, 0, -90); // 90 Grad nach rechts
                break;

            case 2: // Down
                firePoint = firePointDown;
                rotation = Quaternion.Euler(0, 0, 180);
                break;

            case 3: // Left
                firePoint = firePointLeft;
                rotation = Quaternion.Euler(0, 0, 90); // 90 Grad nach links
                break;
        }

        Instantiate(beamPrefab, firePoint.position, rotation);
    }
}
