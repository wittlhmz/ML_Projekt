using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeamSkillManager : MonoBehaviour
{
    public int redLevel = 0;
    public int blueLevel = 0;
    public int greenLevel = 0;
    public int purpleLevel = 0;
    public int buffedShotsRemaining = 0; //für den Lila Skill
    public int purpleItemBonusLevel = 0;
    public int purpleItemBonusMaxShots = 0;
    public int purpleMaxShots = 5;
    public float purpleMultiplier = 1.3f; //multiplier für Lila SKill
    public int redMultiplier = 1;
    public bool blueMultiplier = false;
    public int baseDamage = 50;
    public bool greenOverdrive = false;
    public bool cleave = false;
    private float malusMultiplier = 0.1f;

    public int GetDamage(Beam.BeamType type)
    {
        int damage = baseDamage;

        switch (type)
        {
            case Beam.BeamType.Red:
                //+15 schaden pro Level
                damage += redLevel * 15 * redMultiplier;
                break;

            case Beam.BeamType.Green:
                /* Resistenz ignorieren mit Chance
                float chance = greenLevel * 0.35f; // 0%, 35%, 70%, 100%
                --> Ist im Barrel Skript
                */
                break;

            case Beam.BeamType.Purple:
                // Lila BEAM schaltet den Buff für nächste Schüsse an
                if (!GameManager.Instance.easyMode)
                {
                    ActivatePurpleBuff();
                }
                break;

            case Beam.BeamType.Blue:
                if (blueMultiplier)
                {
                    damage = Mathf.RoundToInt(damage * 2.5f);
                }

                break;

        }

        // Extra SHot Malus
        int extraShots = GetExtraShots(type);

        // Malus berechnen pro schuss -15%
        float reduction = 1f - (extraShots * malusMultiplier);

        reduction = Mathf.Max(reduction, 0.1f); // mind. 10% Schaden bleibt

        damage = Mathf.RoundToInt(damage * reduction);

        //Lila SKill Buff
        if (buffedShotsRemaining > 0)
        {
            buffedShotsRemaining--;
            damage = Mathf.RoundToInt(damage * purpleMultiplier);
        }

        return damage;
    }

    public int GetExtraShots(Beam.BeamType type)
    {
        int extra = 0;

        // Blau: +1 pro Skillstufe
        if (type == Beam.BeamType.Blue)
        {
            extra += blueLevel;    
        }
            
        // CleaveBoost: immer +2, unabhängig von Farbe
        if (cleave == true)
        {
            extra += 2;  
        }

        return extra;
    }

    public void ActivatePurpleBuff()
    {
        int totalLevel = purpleLevel + purpleItemBonusLevel;
        int totalMaxShots = purpleMaxShots + purpleItemBonusMaxShots;

        buffedShotsRemaining = Mathf.Min(2 + totalLevel, totalMaxShots);
    }
    public void LevelUpRed()
    {
        if (GameManager.Instance.activeSkillPoints > 0 && redLevel < 3)
        {
            redLevel++;
            GameManager.Instance.activeSkillPoints--;
            //GameManager.Instance.GrantReward(0.1f);
            SkillUI.Instance.UpdateSkillBar("Red", redLevel);
        }
        else
        {
            Debug.Log("Kein Skillpunkt verfügbar!");
        }
    }

    public void LevelUpBlue()
    {
        if (GameManager.Instance.activeSkillPoints > 0 && blueLevel < 3)
        {
            blueLevel++;
            GameManager.Instance.activeSkillPoints--;
            //GameManager.Instance.GrantReward(0.1f);
            SkillUI.Instance.UpdateSkillBar("Blue", blueLevel);
        }
        else
        {
            Debug.Log("Kein Skillpunkt verfügbar!");
        }
    }
    public void LevelUpGreen()
    {
        if (GameManager.Instance.activeSkillPoints > 0 && greenLevel < 3)
        {
            greenLevel++;
            GameManager.Instance.activeSkillPoints--;
            //GameManager.Instance.GrantReward(0.1f);
            SkillUI.Instance.UpdateSkillBar("Green", greenLevel);
        }
        else
        {
            Debug.Log("Kein Skillpunkt verfügbar!");
        }
    }

    public void LevelUpPurple()
    {
        if (GameManager.Instance.activeSkillPoints > 0 && purpleLevel < 3)
        {
            purpleLevel++;
            GameManager.Instance.activeSkillPoints--;
            //GameManager.Instance.GrantReward(0.1f);
            SkillUI.Instance.UpdateSkillBar("Purple", purpleLevel);
        }
        else
        {
            Debug.Log("Kein Skillpunkt verfügbar!");
        }
    }
}
