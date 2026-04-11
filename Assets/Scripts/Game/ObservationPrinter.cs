using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObservationPrinter : MonoBehaviour
{
    private AutoFireController autoFireController;
    public BarrelSpawnManager barrelManager;
    public BarrelBuffManager buffManager;
    public BeamSkillManager skillManager;
    public enum Difficulty { Easy, Medium, Hard }
    public RadialSpawnManager spawnManager;
    private int maxBarrelsObserved;
    void Start()
    {
        autoFireController = FindObjectOfType<AutoFireController>();
        barrelManager = FindObjectOfType<BarrelSpawnManager>();
        buffManager = FindObjectOfType<BarrelBuffManager>();
        skillManager = FindObjectOfType<BeamSkillManager>();
        spawnManager = FindObjectOfType<RadialSpawnManager>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S)) // S für ObservationS --> alles andere ist belegt
        {
            var obs = GetObservations(Difficulty.Easy);
        }
        if (Input.GetKeyDown(KeyCode.T)) // T für ObservaTions --> alles andere ist belegt
        {
            maxBarrelsObserved = 5;
            var obs = GetObservations(Difficulty.Medium);
        }
        if (Input.GetKeyDown(KeyCode.U)) // U für ObservatiUns--> alles andere ist belegt
        {
            maxBarrelsObserved = 6;
            var obs = GetObservations(Difficulty.Hard);
        }
    }

    public List<float> GetObservations(Difficulty difficulty)
    {

        List<float> obs = new List<float>();

        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        sb.AppendLine("===== OBSERVATIONS =====");




        if (GameManager.Instance.easyMode)
        {
            float[] barrelColour = GetSingleBarrelColorArray();
            obs.AddRange(barrelColour);
            sb.AppendLine($"1 Colour ({barrelColour.Length}): {string.Join(", ", barrelColour)}");
            // ---- Positionen ----
            float[] barrelPos = GetBarrelHp();
            obs.AddRange(barrelPos);
            sb.AppendLine($"Positions + health ({barrelPos.Length}): {string.Join(", ", barrelPos)}");
            // ---- Farben ----
            float[] barrelColors = GetBarrelColorsArray();
            obs.AddRange(barrelColors);
            sb.AppendLine($"Colors ({barrelColors.Length}): {string.Join(", ", barrelColors)}");
            // ---- Verbleibende Barrels ----
            float remaining2 = GameManager.Instance.GetRemainingBarrelsNormalized();
            obs.Add(remaining2);
            sb.AppendLine($"Remaining Barrels: {remaining2}");

            // ---- Aktuelle Schussrichtung ----
            /*float[] dir2 = OneHot(autoFireController.currentDirection, 4);
            obs.AddRange(dir2);
            sb.AppendLine($"Current Direction (one-hot): {string.Join(", ", dir2)}");*/

            // ---- Aktueller Beam ----
            float[] beam2 = OneHot(autoFireController.currentBeamIndex, 4);
            obs.AddRange(beam2);
            sb.AppendLine($"Current Beam (one-hot): {string.Join(", ", beam2)}");

            // ---- Cooldown ----
            float cooldown2 = autoFireController.timer / autoFireController.fireInterval;
            obs.Add(cooldown2);
            sb.AppendLine($"Cooldown: {cooldown2}");

            // ---- DPS ----
            float dmg2 = GameManager.Instance.dps / 100f;
            obs.Add(dmg2);        // Debug
            sb.AppendLine($"DPS: {dmg2}");

            // ---- peakDPS ----
            float peakDMG2 = GameManager.Instance.peakDps / 100f;
            obs.Add(peakDMG2);
            sb.AppendLine($"peak DMG: {peakDMG2}");
            }
        else
            {

                AddBarrelObservations(obs, sb);
                // ---- Verbleibende Barrels ----
                float remaining = GameManager.Instance.GetRemainingBarrelsNormalized();
                obs.Add(remaining);
                sb.AppendLine($"Remaining Barrels: {remaining}");

            // ---- Aktuelle Schussrichtung ----
                float x = autoFireController.currentX;
                obs.Add(x);
                sb.AppendLine($"Current X Direction : {string.Join(", ", x)}");
                float y = autoFireController.currentY;
                obs.Add(y);
                sb.AppendLine($"Current Y Direction : {string.Join(", ", y)}");

                // ---- Aktueller Beam ----
                float[] beam = OneHot(autoFireController.currentBeamIndex, 4);
                obs.AddRange(beam);
                sb.AppendLine($"Current Beam (one-hot): {string.Join(", ", beam)}");

                // ---- Cooldown ----
                float cooldown = autoFireController.timer / autoFireController.fireInterval;
                obs.Add(cooldown);
                sb.AppendLine($"Cooldown: {cooldown}");

                // ---- DPS ----
                float dmg = GameManager.Instance.dps / 100f;
                obs.Add(dmg);        // Debug
                sb.AppendLine($"DPS: {dmg}");

                // ---- peakDPS ----
                float peakDMG = GameManager.Instance.peakDps / 100f;
                obs.Add(peakDMG);
                sb.AppendLine($"peak DMG: {peakDMG}");

            }

        
        if (difficulty >= Difficulty.Medium)
        {
            float barrelsUntilBuff = GameManager.Instance.barrelsUntilBuff / 5f;
            obs.Add(barrelsUntilBuff);
            sb.AppendLine($"barrelsUntilBuff: {barrelsUntilBuff}");

            float activeSkillPoints = GameManager.Instance.activeSkillPoints / 12f;
            obs.Add(activeSkillPoints);
            sb.AppendLine($"activeSkillPoints: {activeSkillPoints}");

            // --- Skills ---
            float redLevel = skillManager.redLevel / 3f;
            obs.Add(redLevel);
            sb.AppendLine($"redLevel: {redLevel}");

            float blueLevel = skillManager.blueLevel / 3f;
            obs.Add(blueLevel);
            sb.AppendLine($"blueLevel: {blueLevel}");

            float greenLevel = skillManager.greenLevel / 3f;
            obs.Add(greenLevel);
            sb.AppendLine($"greenLevel: {greenLevel}");

            float purpleLevel = skillManager.purpleLevel / 3f;
            obs.Add(purpleLevel);
            sb.AppendLine($"purpleLevel: {purpleLevel}");

            float buffedShotsRemaining = skillManager.buffedShotsRemaining / 5f;
            obs.Add(buffedShotsRemaining);
            sb.AppendLine($"buffedShotsRemaining: {buffedShotsRemaining}");

            // --- Barrel Buffs ---
            float redResistBonus = buffManager.redResistBonus / 100f;
            obs.Add(redResistBonus);
            sb.AppendLine($"redResistBonus: {redResistBonus}");

            float blueResistBonus = buffManager.blueResistBonus / 100f;
            obs.Add(blueResistBonus);
            sb.AppendLine($"blueResistBonus: {blueResistBonus}");

            float greenResistBonus = buffManager.greenResistBonus / 100f;
            obs.Add(greenResistBonus);
            sb.AppendLine($"greenResistBonus: {greenResistBonus}");

            float purpleResistBonus = buffManager.purpleResistBonus / 100f;
            obs.Add(purpleResistBonus);
            sb.AppendLine($"purpleResistBonus: {purpleResistBonus}");

            float redHpBonus = buffManager.redHpBonus / 100f;
            obs.Add(redHpBonus);
            sb.AppendLine($"redHpBonus: {redHpBonus}");

            float blueHpBonus = buffManager.blueHpBonus / 100f;
            obs.Add(blueHpBonus);
            sb.AppendLine($"blueHpBonus: {blueHpBonus}");

            float greenHpBonus = buffManager.greenHpBonus / 100f;
            obs.Add(greenHpBonus);
            sb.AppendLine($"greenHpBonus: {greenHpBonus}");

            float purpleHpBonus = buffManager.purpleHpBonus / 100f;
            obs.Add(purpleHpBonus);
            sb.AppendLine($"purpleHpBonus: {purpleHpBonus}");

            float maxActiveBarrels = spawnManager.maxActiveBarrels / 5f;
            obs.Add(maxActiveBarrels);
            sb.AppendLine($"maxActiveBarrels: {maxActiveBarrels}");
        }
        if (difficulty >= Difficulty.Hard)
        {
            // --- Item OneHot ---
            int totalSlots = ItemManager.Instance.maxInventorySize; 
            float[] items = new float[ItemManager.Instance.availableItems.Count];
            for (int i = 0; i < ItemManager.Instance.availableItems.Count; i++)
            {
                Item item = ItemManager.Instance.availableItems[i];
                int slotIndex = ItemManager.Instance.inventory.IndexOf(item);

                if (slotIndex >= 0)
                {
                    // Slot in Range 1..totalSlots → Normierung
                    float normalizedSlot = (slotIndex + 1) / (float)totalSlots;
                    items[i] = normalizedSlot;
                }
                else
                {
                    // Item nicht im Inventar
                    items[i] = 0f;
                }
            }
            obs.AddRange(items);
            sb.AppendLine($"Items: {string.Join("", items)}");

            // --- Inventar ---
            int itemCount = ItemManager.Instance.inventory.Count;
            float normalizedItemCount = itemCount / 4f;
            obs.Add(normalizedItemCount);
            sb.AppendLine($"itemCount: {normalizedItemCount} (raw: {itemCount})");

            // --- Gold ---
            float gold = GameManager.Instance.gold / 100f;
            obs.Add(gold);
            sb.AppendLine($"gold: {gold}");

            // --- Item-Effekte ---
            float baseDamage = skillManager.baseDamage / 70f;
            obs.Add(baseDamage);
            sb.AppendLine($"baseDamage: {baseDamage}");

            float fireInterval = autoFireController.fireInterval;
            obs.Add(fireInterval);
            sb.AppendLine($"fireInterval: {fireInterval}");

            float greenOverdrive = skillManager.greenOverdrive ? 1f : 0f;
            obs.Add(greenOverdrive);
            sb.AppendLine($"greenOverdrive: {greenOverdrive}");

            float cleave = skillManager.cleave ? 1f : 0f;
            obs.Add(cleave);
            sb.AppendLine($"cleave: {cleave}");

            float goldIncrease = GameManager.Instance.goldIncrease / 4f;
            obs.Add(goldIncrease);
            sb.AppendLine($"goldIncrease: {goldIncrease}");

            float purpleItemBonusLevel = skillManager.purpleItemBonusLevel / 3f;
            obs.Add(purpleItemBonusLevel);
            sb.AppendLine($"purpleItemBonusLevel: {purpleItemBonusLevel}");

            float purpleItemBonusMaxShots = skillManager.purpleItemBonusMaxShots / 3f;
            obs.Add(purpleItemBonusMaxShots);
            sb.AppendLine($"purpleItemBonusMaxShots: {purpleItemBonusMaxShots}");

            float purpleMultiplier = skillManager.purpleMultiplier / 1.7f;
            obs.Add(purpleMultiplier);
            sb.AppendLine($"purpleMultiplier: {purpleMultiplier}");

            float redMultiplier = skillManager.redMultiplier / 3f;
            obs.Add(redMultiplier);
            sb.AppendLine($"redMultiplier: {redMultiplier}");

            float blueMultiplier = skillManager.blueMultiplier ? 1f : 0f;
            obs.Add(blueMultiplier);
            sb.AppendLine($"blueMultiplier: {blueMultiplier}");
        }


        Debug.Log(sb.ToString());

        return obs;
    }

    // -------- Hilfsfunktionen --------
    private float[] GetBarrelPositionsArray()
    {
        float[] arr = new float[barrelManager.spawnOccupied.Length];
        for (int i = 0; i < barrelManager.spawnOccupied.Length; i++)
        {
            arr[i] = barrelManager.spawnOccupied[i] ? 1f : 0f;
        }
        return arr;
    }

    private float[] GetBarrelColorsArray()
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
                // Leerer OneHot
            }
        }

        return arr;
    }

    private int GetColorIndexFromTag(string tag)
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

    private float[] OneHot(int index, int length)
    {
        float[] arr = new float[length];
        if (index >= 0 && index < length) arr[index] = 1f;
        return arr;
    }
    private float[] GetBarrelHp()
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
        return hpArr;
    }

    private float[] GetSingleBarrelColorArray()
    {
        float[] oneHot = new float[4];

        if (barrelManager.activeBarrels.Count > 0)
        {
            GameObject barrelObj = barrelManager.activeBarrels[0]; //erstes Objekt der Liste holen
            Barrel barrel = barrelObj.GetComponent<Barrel>();

            if (barrel != null)
            {
                int colorIndex = GetColorIndexFromTag(barrel.tag);
                oneHot[colorIndex] = 1f;
            }
        }
        else
        {

        }
        return oneHot;
    }

    private void AddBarrelObservations(List<float> obs, System.Text.StringBuilder sb)
    {
        for (int i = 0; i < maxBarrelsObserved; i++)
        {
            if (i < spawnManager.activeBarrels.Count && spawnManager.activeBarrels[i] != null)
            {
                Barrel b = spawnManager.activeBarrels[i].GetComponent<Barrel>();
                float[] barrelObs = b.GetObservation(); // z.B. [x, y, hp, oneHot(4)]
                obs.AddRange(barrelObs);
                sb.AppendLine($"Barrel {i}: {string.Join(", ", barrelObs)}");
            }
            else
            {
                float[] zeros = new float[7]; // gleiche Länge wie Barrel.GetObservation()
                obs.AddRange(zeros);
                sb.AppendLine($"Barrel {i}: {string.Join(", ", zeros)}");
            }
        }
    }
}

