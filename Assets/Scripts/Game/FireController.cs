using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireController : MonoBehaviour //automatisches Feuern für die leichte Schwierigkeit
{

    public GameObject[] beamPrefabs;
    public float fireInterval = 0.5f; // Sekunden zwischen den Schüssen
    public int currentBeamIndex = 0;
    public float timer;
    public bool manualControl = false;
    public Transform firePointUp;
    public Transform firePointRight;
    public Transform firePointDown;
    public Transform firePointLeft;
    public int currentDirection = 0;

    void Update()
    {
        if (manualControl)
        {
            HandleInput();
        }

        timer += Time.deltaTime;
        if (timer >= fireInterval)
        {
            timer = 0f;
            FireBeam();
        }
    }

    // Methode zum Testen für mich 
    void HandleInput()
    {
        // Richtung setzen
        if (Input.GetKeyDown(KeyCode.Alpha1)) currentDirection = 0; // oben
        if (Input.GetKeyDown(KeyCode.Alpha2)) currentDirection = 1; // rechts
        if (Input.GetKeyDown(KeyCode.Alpha3)) currentDirection = 2; // unten
        if (Input.GetKeyDown(KeyCode.Alpha4)) currentDirection = 3; // links
    }

    void FireBeam()
    {
        if (currentBeamIndex < 0 || currentBeamIndex >= beamPrefabs.Length)
        {

            return;
        }

        GameObject prefab = beamPrefabs[currentBeamIndex];
        Beam beamComponent = prefab.GetComponent<Beam>();
        Beam.BeamType type = beamComponent.beamType;
        Transform firePoint = firePointUp;
        Quaternion rotation = Quaternion.identity;

        switch (currentDirection)
        {
            case 0: // Up
                firePoint = firePointUp;
                rotation = Quaternion.identity;
                break;
            case 1: // Right
                firePoint = firePointRight;
                rotation = Quaternion.Euler(0, 0, -90);
                break;
            case 2: // Down
                firePoint = firePointDown;
                rotation = Quaternion.Euler(0, 0, 180);
                break;
            case 3: // Left
                firePoint = firePointLeft;
                rotation = Quaternion.Euler(0, 0, 90);
                break;
        }

        Instantiate(prefab, firePoint.position, rotation);
        GameManager.Instance.shotTracker++;
        //Statistik, welcher beam wie oft abgeschossen wurde
        switch (type)
        {
            case Beam.BeamType.Red:
                GameManager.Instance.redShots++;
                break;
            case Beam.BeamType.Blue:
                GameManager.Instance.blueShots++;
                break;
            case Beam.BeamType.Green:
                GameManager.Instance.greenShots++;
                break;
            case Beam.BeamType.Purple:
                GameManager.Instance.purpleShots++;
                break;
        }

    }
    private void AddShotStatistic(Beam.BeamType type)
    {
        switch (type)
        {
            case Beam.BeamType.Red: GameManager.Instance.redShots++; break;
            case Beam.BeamType.Blue: GameManager.Instance.blueShots++; break;
            case Beam.BeamType.Green: GameManager.Instance.greenShots++; break;
            case Beam.BeamType.Purple: GameManager.Instance.purpleShots++; break;
        }
    }
}
