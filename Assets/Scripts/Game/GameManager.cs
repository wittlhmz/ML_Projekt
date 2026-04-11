using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private int maxBarrels = 50;
    public int remainingBarrels;
    public int barrelsUntilBuff = 5; // alle 5 zerstörten Fässer Buff
    public int gold;
    public int activeSkillPoints;
    public bool easyMode;
    private int maxSkillPoints = 12;
    private int earlyGameSkills = 6;
    private int firstSixPoints = 0;
    public int destroyedBarrels = 0; // Countvariable für zerstörte Fässer
    public int goldIncrease = 2;
    private BarrelBuffManager buffManager;
    private float accumulatedDamage = 0f;
    private float damageTimer = 0f;
    public float dps = 0f;
    public float peakDps = 0f;


    //StatsStatsStatsStatsStats
    public float shotTracker = 0f;
    public float hitTracker = 0f;
    public float rightColour = 0f;
    public float episodeStartTime;
    public float episodeDuration => Time.time - episodeStartTime;
    public int redShots = 0;
    public int blueShots = 0;
    public int greenShots = 0;
    public int purpleShots = 0;
    public bool earlyBuildLogged = false;

    // Actions für Stats und Episode beenden
    public event Action OnAllBarrelsDestroyed;
    public event Action OnEarlySkillbuild;
    // Action für die dense Rewards
    public event Action<float> OnReward;

    void Start()
    {
        buffManager = FindObjectOfType<BarrelBuffManager>();
    }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        remainingBarrels = maxBarrels;
    }

    void FixedUpdate()
    {
        //Berechnung DPS
        damageTimer += Time.fixedDeltaTime;

        if (damageTimer >= 1f)
        {
            dps = accumulatedDamage / damageTimer;
            if (dps > peakDps)
            {
                peakDps = dps;
            }
            accumulatedDamage = 0f;
            damageTimer = 0f;
        }
    }

    public void OnBarrelDestroyed()
    {
        destroyedBarrels++;
        remainingBarrels--;
        barrelsUntilBuff--;
        // Buff hinzufügen
        if (barrelsUntilBuff <= 0)
        {
            if (!easyMode)
            {
                buffManager.ApplyRandomBuff();
            }
            barrelsUntilBuff = 5;
        }

        // Alle 3 Fässer Skillpunkt hinzufügen
        if (destroyedBarrels % 3 == 0 && activeSkillPoints < maxSkillPoints)
        {
            activeSkillPoints++;
            
            if (!earlyBuildLogged)
            {
                firstSixPoints++;

                if (firstSixPoints == earlyGameSkills)
                {
                    OnEarlySkillbuild?.Invoke();
                    earlyBuildLogged = true; 
                    
                }
            }
        }

        gold += goldIncrease;

        if (remainingBarrels <= 0)
        {
            OnAllBarrelsDestroyed?.Invoke();
        }
    }

    public float GetRemainingBarrelsNormalized()
    {
        return (float)remainingBarrels / maxBarrels;
    }

    public int GetRemainingBarrels()
    {
        return remainingBarrels;
    }
    public void AddDps(int damage)
    {
        accumulatedDamage += damage;
    }
    public float GetHitAccuracy()
    {
        return (float)rightColour / hitTracker;
    }
    public float GetShotAccuracy()
    {
        return (float)hitTracker / shotTracker;
    }
    //Rewardsetzung: 
    //Easy: Barrel Script: 3x in ApplyDamage
    //Medium: + Reward für Skillpunkt-Setzung: BeamSkillManager 4x
    //Hard: ItemManager 2x
    public void GrantReward(float value)
    {
        OnReward?.Invoke(value);
    }
}
