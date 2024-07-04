using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wire : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    public int currentEnergy = 0;

    public void Used(int energy, string wireColor) //цвета - Red, Green
    {
        currentEnergy += energy;

        switch (wireColor)
        {
            case "Red":
                spriteRenderer.color = new Color(0.2f + 0.2f * currentEnergy, 0, 0);
                break;

            case "Green":
                spriteRenderer.color = new Color(0, 0.2f + 0.2f * currentEnergy, 0);
                break;
        }
    }
}
