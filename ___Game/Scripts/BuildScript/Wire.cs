using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wire : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    public int[] buildingsNumbers = new int[2];
    public int currentEnergy;

    public void Used(int energy)
    {
        currentEnergy += energy;
        spriteRenderer.color = new Color(0.5f + 0.1f * currentEnergy, 0, 0);
    }
}
