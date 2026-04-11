using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoFireController : MonoBehaviour //radiales automatisches Feuern für mittlere und harte Schwierigkeit
{

    public GameObject[] beamPrefabs;
    public Transform firePoint;
    public float fireInterval = 0.5f; // Sekunden zwischen den Schüssen

    public float currentX = 0f;
    public float currentY = 1f;
    public int currentBeamIndex = 0;
    public float timer;
    public bool manualControl = false;
    private float manualAngle = 0f;
    private BeamSkillManager skillManager;
    private ItemManager itemManager;
    public Item[] allItems;

    void Start()
    {
        skillManager = FindObjectOfType<BeamSkillManager>();
        if (manualControl)
        {
            itemManager = FindObjectOfType<ItemManager>();
        }

    }
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
            FireBeam(currentX, currentY);
        }
    }

    // Methode zum Testen für mich 
    void HandleInput()
    {
        // Richtung setzen
        /*if (Input.GetKeyDown(KeyCode.Alpha1)) currentDirection = 0; // oben
        if (Input.GetKeyDown(KeyCode.Alpha2)) currentDirection = 1; // rechts
        if (Input.GetKeyDown(KeyCode.Alpha3)) currentDirection = 2; // unten
        if (Input.GetKeyDown(KeyCode.Alpha4)) currentDirection = 3; // links*/

        // Taste 1  um 30 Grad weiter
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            manualAngle += 30f;
            if (manualAngle >= 360f) manualAngle -= 360f;
        }

        // Taste 2 um 30 Grad zurück
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            manualAngle -= 30f;
            if (manualAngle < 0f) manualAngle += 360f;
        }

        // Richtung in X/Y umrechnen 
        float rad = manualAngle * Mathf.Deg2Rad;
        currentX = Mathf.Cos(rad);
        currentY = Mathf.Sin(rad);

        // Beam-Prefab auswählen
        if (Input.GetKeyDown(KeyCode.Alpha5)) currentBeamIndex = 0; //rot
        if (Input.GetKeyDown(KeyCode.Alpha6)) currentBeamIndex = 1; //lila
        if (Input.GetKeyDown(KeyCode.Alpha7)) currentBeamIndex = 2; //grün
        if (Input.GetKeyDown(KeyCode.Alpha8)) currentBeamIndex = 3; //blau

        if (Input.GetKeyDown(KeyCode.A)) itemManager.BuyItem(allItems[0]);
        if (Input.GetKeyDown(KeyCode.B)) itemManager.BuyItem(allItems[1]);
        if (Input.GetKeyDown(KeyCode.C)) itemManager.BuyItem(allItems[2]);
        if (Input.GetKeyDown(KeyCode.D)) itemManager.BuyItem(allItems[3]);
        if (Input.GetKeyDown(KeyCode.E)) itemManager.BuyItem(allItems[4]);
        if (Input.GetKeyDown(KeyCode.F)) itemManager.BuyItem(allItems[5]);
        if (Input.GetKeyDown(KeyCode.G)) itemManager.BuyItem(allItems[6]);
        if (Input.GetKeyDown(KeyCode.H)) itemManager.BuyItem(allItems[7]);
        if (Input.GetKeyDown(KeyCode.I)) itemManager.BuyItem(allItems[8]);

        // Verkaufen
        if (Input.GetKeyDown(KeyCode.J)) itemManager.SellItem(allItems[0]);
        if (Input.GetKeyDown(KeyCode.K)) itemManager.SellItem(allItems[1]);
        if (Input.GetKeyDown(KeyCode.L)) itemManager.SellItem(allItems[2]);
        if (Input.GetKeyDown(KeyCode.M)) itemManager.SellItem(allItems[3]);
        if (Input.GetKeyDown(KeyCode.N)) itemManager.SellItem(allItems[4]);
        if (Input.GetKeyDown(KeyCode.O)) itemManager.SellItem(allItems[5]);
        if (Input.GetKeyDown(KeyCode.P)) itemManager.SellItem(allItems[6]);
        if (Input.GetKeyDown(KeyCode.Q)) itemManager.SellItem(allItems[7]);
        if (Input.GetKeyDown(KeyCode.R)) itemManager.SellItem(allItems[8]);
    }

    public void FireBeam(float ax, float ay)
    {
        if (currentBeamIndex < 0 || currentBeamIndex >= beamPrefabs.Length)
            return;

        
        Vector2 dir = new Vector2(ax, ay);
        if (dir.sqrMagnitude < 0.01f)
            dir[0] = 0.01f;

        dir.Normalize();

        GameObject prefab = beamPrefabs[currentBeamIndex];
        Beam beamComponent = prefab.GetComponent<Beam>();
        Beam.BeamType type = beamComponent.beamType;

        // Spawn-Position auf Kreisradius
        float fireRadius = 2.5f;
        Vector3 firePos = firePoint.position + (Vector3)(dir * fireRadius);

        // Rotation aus Richtung bestimmen
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        float spriteOffset = -90f;
        Quaternion rotation = Quaternion.Euler(0, 0, angle + spriteOffset);

        Instantiate(prefab, firePos, rotation);

        // Statistik
        GameManager.Instance.shotTracker++;
        AddShotStatistic(type);
        int extraShots = skillManager.GetExtraShots(type);
        if (extraShots > 0)
        {
            float spreadAngle = 35f; // Grad pro Skillpunkt

            for (int i = 1; i <= extraShots; i++)
            {
                float extraBeamAngle = angle + (i * spreadAngle);
                Vector2 extraDir = new Vector2(Mathf.Cos(extraBeamAngle * Mathf.Deg2Rad), Mathf.Sin(extraBeamAngle * Mathf.Deg2Rad));
                Vector3 extraPos = transform.position + (Vector3)(extraDir * fireRadius);

                Instantiate(prefab, extraPos, Quaternion.Euler(0, 0, extraBeamAngle + spriteOffset));

                GameManager.Instance.shotTracker++;
                AddShotStatistic(type);
            }
        }
    }

    /*void FireBeam()
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

        // Eextra Beams bei blauem Blitz

        int extraShots = skillManager.GetExtraShots(type);

        for (int i = 1; i <= extraShots; i++)
        {
            int extraDir = (currentDirection + i) % 4;
            Transform extraFirePoint = firePointUp;
            Quaternion extraRotation = Quaternion.identity;

            switch (extraDir)
            {
                case 0:
                    extraFirePoint = firePointUp;
                    extraRotation = Quaternion.identity;
                    break;
                case 1:
                    extraFirePoint = firePointRight;
                    extraRotation = Quaternion.Euler(0, 0, -90);
                    break;
                case 2:
                    extraFirePoint = firePointDown;
                    extraRotation = Quaternion.Euler(0, 0, 180);
                    break;
                case 3:
                    extraFirePoint = firePointLeft;
                    extraRotation = Quaternion.Euler(0, 0, 90);
                    break;
            }

            Instantiate(prefab, extraFirePoint.position, extraRotation);
            GameManager.Instance.shotTracker++;
        }
    }*/
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
