using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class BuffUI : MonoBehaviour
{
    [Header("HP Buff Texts")]
    public TextMeshProUGUI redHpText;
    public TextMeshProUGUI blueHpText;
    public TextMeshProUGUI greenHpText;
    public TextMeshProUGUI purpleHpText;

    [Header("Resist Buff Texts")]
    public TextMeshProUGUI redResistText;
    public TextMeshProUGUI blueResistText;
    public TextMeshProUGUI greenResistText;
    public TextMeshProUGUI purpleResistText;

    [Header("Extra Barrels")]
    public TextMeshProUGUI extraBarrelsText;
    public static BuffUI Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public void UpdateBuffUI(
        int redHp, int blueHp, int greenHp, int purpleHp,
        int redResist, int blueResist, int greenResist, int purpleResist,
        int extraBarrels, int maxExtraBarrels)
    {
        // HP
        redHpText.text = $"Rot: {redHp}";
        blueHpText.text = $"Blau: {blueHp}";
        greenHpText.text = $"Grün: {greenHp}";
        purpleHpText.text = $"Lila: {purpleHp}";

        // Resist
        redResistText.text = $"Rot: {redResist}";
        blueResistText.text = $"Blau: {blueResist}";
        greenResistText.text = $"Grün: {greenResist}";
        purpleResistText.text = $"Lila: {purpleResist}";

        // Extra Barrels
        extraBarrelsText.text = $"Extra Barrels: {extraBarrels} / {maxExtraBarrels}";
    }
}
