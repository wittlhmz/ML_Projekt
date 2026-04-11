using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RemainingBarrelsUI : MonoBehaviour
{
    public TextMeshProUGUI barrelsText;
    public TextMeshProUGUI colourAccuracy;
    public TextMeshProUGUI peakDpsText;
    public TextMeshProUGUI shotAccuracy;
    public TextMeshProUGUI timeText;

    private void Update()
    {
        if (GameManager.Instance != null && barrelsText != null && colourAccuracy != null && peakDpsText != null && shotAccuracy != null)
        {

            barrelsText.text = "" + GameManager.Instance.GetRemainingBarrels();

            peakDpsText.text = "peak DPS: " + GameManager.Instance.peakDps;

            float colourAcc = GameManager.Instance.GetHitAccuracy() * 100f; // in %
            colourAccuracy.text = $"Farben-Acc: {colourAcc:F1}%";
            float shotAcc = GameManager.Instance.GetShotAccuracy() * 100f; // in %
            shotAccuracy.text = $"Hit-Acc: {shotAcc:F1}%";

            float duration = GameManager.Instance.episodeDuration;
            timeText.text = $"{duration:F1} sec"; // eine Nachkommastelle
    
        }
    }
}
