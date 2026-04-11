using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillUI : MonoBehaviour
{
    [Header("Skill UI Images")]
    public Image[] redImages;
    public Image[] blueImages;
    public Image[] greenImages;
    public Image[] purpleImages;

    [Header("Skill Colors")]
    public Color redColor = Color.red;
    public Color blueColor = Color.blue;
    public Color greenColor = Color.green;
    public Color purpleColor = new Color(0.6f, 0, 0.6f); // Lila
    public Color inactiveColor = Color.white;
    //public BeamSkillManager skillManager;

    public static SkillUI Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        //skillManager = FindObjectOfType<BeamSkillManager>();
    }
    /*void Update()
    {
        UpdateSkillBar("Red", skillManager.redLevel);
        UpdateSkillBar("Blue", skillManager.blueLevel);
        UpdateSkillBar("Green", skillManager.greenLevel);
        UpdateSkillBar("Purple", skillManager.purpleLevel);
    }*/

    public void UpdateSkillBar(string skill, int level)
    {
        switch (skill)
        {
            case "Red":
                UpdateBar(redImages, level, redColor);
                break;
            case "Blue":
                UpdateBar(blueImages, level, blueColor);
                break;
            case "Green":
                UpdateBar(greenImages, level, greenColor);
                break;
            case "Purple":
                UpdateBar(purpleImages, level, purpleColor);
                break;
        }
    }

    private void UpdateBar(Image[] images, int level, Color activeColor)
    {
        for (int i = 0; i < images.Length; i++)
        {
            if (i < level)
                images[i].color = activeColor;
            else
                images[i].color = inactiveColor;
        }
    }
}
