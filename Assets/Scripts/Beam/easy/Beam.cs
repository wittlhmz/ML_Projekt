using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beam : MonoBehaviour
{
    public float lifetime = 0.49f;
    public enum BeamType { Red, Blue, Green, Purple }
    private BeamSkillManager skillManager;
    //private BarrelSpawnManager spawnManager;
    public BeamType beamType;

    void Start()
    {
        Destroy(gameObject, lifetime);
        skillManager = FindObjectOfType<BeamSkillManager>();
        //spawnManager = FindObjectOfType<BarrelSpawnManager>();

    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Prüfen, ob  Objekt ein Barrel ist 
        if (other.GetComponent<Barrel>() != null)
        {
            // Damage vom Manager holen
            int finalDamage = skillManager.GetDamage(beamType);
            Debug.Log($"[Beam] {beamType} trifft! Gemachter Schaden: {finalDamage}");

            Barrel barrel = other.GetComponent<Barrel>();
            // normaler Schaden
            barrel.ApplyDamage(this.tag, finalDamage);

            /* Cleave prüfen, wenn Item gekauft
            if (skillManager.cleave) // bool für Item
            {
                // Nachbarn ermitteln 
                int[][] neighbors = new int[4][] {
                    new int[]{3,1}, // Up -> Left + Right
                    new int[]{0,2}, // Right -> Up + Down
                    new int[]{3,1}, // Down -> Left + Right
                    new int[]{0,2}  // Left -> Up + Down
                };

                int hitIndex = barrel.spawnIndex;
                foreach (int n in neighbors[hitIndex])
                {
                    Barrel neighbor = spawnManager.barrelsAtSpawn[n];
                    if (neighbor != null)
                    {
                        neighbor.ApplyDamage(this.tag, Mathf.RoundToInt(finalDamage * 0.5f));
                    }
                }
            }*/
        }
    }
}
