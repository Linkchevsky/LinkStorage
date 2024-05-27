using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WireScript : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    public int[] buildingsNumbers = new int[2];
    public int currentEnergy;

    public void Used(int energy)
    {
        currentEnergy += energy;
        spriteRenderer.color = Color.red;
    }
}
