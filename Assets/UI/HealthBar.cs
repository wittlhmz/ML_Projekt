using UnityEngine;
using UnityEngine.UI; 

public class HealthBar : MonoBehaviour
{
    [Header("UI")]
    public Image fillImage; // Das rote Füll-Image

    private int maxHP;
    private int currentHP;

    public void SetMaxHealth(int maxHealth)
    {
        maxHP = maxHealth;
        currentHP = maxHealth;
        UpdateFill();
    }

    public void SetHealth(int health)
    {
        currentHP = Mathf.Clamp(health, 0, maxHP);
        UpdateFill();
    }

    private void UpdateFill()
    {
        if (fillImage != null && maxHP > 0)
        {
            fillImage.fillAmount = (float)currentHP / maxHP;
        }
    }
}
