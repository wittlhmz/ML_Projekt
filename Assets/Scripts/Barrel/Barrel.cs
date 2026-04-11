using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrel : MonoBehaviour
{

    public enum BarrelType { Red, Blue, Green, Purple }
    public BarrelType barrelType;
    public Sprite brokenSprite;
    public float fadeDuration = 1f;
    private SpriteRenderer spriteRenderer;
    private bool isBroken = false;
    public RadialSpawnManager spawnManager;
    //public BarrelSpawnManager spawnManager;
    private BarrelBuffManager buffManager;
    private BeamSkillManager beamManager;
    public int maxHP = 100;
    public int currentHP;
    public int finalDamage = 0;
    public HealthBar healthBar;
    public int spawnIndex;

    void Start()
    {
        buffManager = FindObjectOfType<BarrelBuffManager>();
        beamManager = FindObjectOfType<BeamSkillManager>();
        spawnManager = FindObjectOfType<RadialSpawnManager>();
        //spawnManager = FindObjectOfType<BarrelSpawnManager>();

        // HP Bonus holen und auf UI machen 
        int hpBonus = buffManager.GetHpBonus(barrelType);
        maxHP += hpBonus;
        currentHP = maxHP;
        healthBar.SetMaxHealth(maxHP);
    }

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }


    public void ApplyDamage(string attackerTag, int baseDamage)
    {
        if (isBroken) return;

        GameManager.Instance.hitTracker++; //stats berechnung
        int damage = baseDamage;

        // --- Dense Reward: Treffer ---
        //GameManager.Instance.GrantReward(0.05f);

        //Schadensberechnung Tag
        if (this.CompareTag(attackerTag))

        {
            damage *= 2;
            GameManager.Instance.rightColour++;
            // --- Dense Reward: Farbtreffer --- (Extra Reward, wenn die Farbe übereinstimmt)
            //GameManager.Instance.GrantReward(0.05f);
        }

        // Resistenzen berechnen
        if (buffManager != null)
        {
            int resistBonus = buffManager.GetResistBonus(barrelType);

            if (attackerTag == "green")
            {
                // Chance basierend auf greenLevel
                float chance = beamManager.greenLevel * 0.3333f; // 0, 0.35, 0.7, 1.0
                if (Random.value < chance)
                {
                    // Resistenz wird ignoriert --> also NICHTS abziehen, außer wenn Item gekauft wurde
                    if (beamManager.greenOverdrive)
                    {
                        damage += resistBonus;
                    }
                }
                else
                {
                    // gleiche Farbe = volle Resistenz
                if (this.CompareTag(attackerTag))
                {
                    damage -= resistBonus;
                }
                // andere Farbe = halbe Resistenz
                else
                {
                    damage -= Mathf.RoundToInt(resistBonus * 0.5f);
                }

                damage = Mathf.Max(damage, 1);
                }
            }
            else
            {
                // gleiche Farbe = volle Resistenz
                if (this.CompareTag(attackerTag))
                {
                    damage -= resistBonus;
                }
                // andere Farbe = halbe Resistenz
                else
                {
                    damage -= Mathf.RoundToInt(resistBonus * 0.5f);
                }

                damage = Mathf.Max(damage, 1);
            }
        }

        finalDamage = damage;
        currentHP -= damage;

        healthBar.SetHealth(currentHP);

        //Dps counter für Observation
        GameManager.Instance.AddDps(finalDamage);
        // Damage Reward
        /*float reward = (float)damage / maxHP * 0.1f;
        GameManager.Instance.GrantReward(reward);*/

        Debug.Log("Last damage set to: " + finalDamage);
        if (currentHP <= 0)
        {
            // --- Dense Reward: Fass zerstört ---
            //GameManager.Instance.GrantReward(0.15f);
            Break();
        }
    }

    public void Break()
    {
        if (isBroken) return;
        isBroken = true;

        spriteRenderer.sprite = brokenSprite;

        GameManager.Instance.OnBarrelDestroyed(); 

        StartCoroutine(FadeOutAndDestroy());
    }

    private IEnumerator FadeOutAndDestroy()
    {
        //collider direkt disablen, damit cleave richtig klappt
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.enabled = false;
        }
        // langsamer fadeout
        float elapsed = 0f;
        float halfDuration = fadeDuration * 0.5f;
        Color originalColor = spriteRenderer.color;

        while (elapsed < halfDuration)
        {
            float alpha = Mathf.Lerp(1f, 0f, elapsed / halfDuration);
            spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // RadialSpawnManager informieren
        /*if (spawnManager != null)
        {
            spawnManager.OnBarrelDestroyed(gameObject, spawnIndex);
        }*/
        spawnManager?.OnBarrelDestroyed(gameObject);
        Destroy(gameObject);
    }

    public float[] GetObservation()
    {
        List<float> obs = new List<float>();

        // Position 
        Vector2 pos = transform.position;
        float radius = 3f;
        obs.Add(pos.x / radius);
        obs.Add(pos.y / radius);

        // HP normalisiert
        float hpNorm = currentHP / (float)maxHP;
        obs.Add(hpNorm);

        // Farbe OneHot
        float[] colorOneHot = new float[4];
        int colourIndex = GetColourIndexFromTag(tag);
        if (colourIndex >= 0 && colourIndex < 4)
            colorOneHot[colourIndex] = 1f;
        obs.AddRange(colorOneHot);

        return obs.ToArray();
    }
    int GetColourIndexFromTag(string tag)
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
}
