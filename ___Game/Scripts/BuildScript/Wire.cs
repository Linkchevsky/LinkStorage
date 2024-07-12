using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Wire : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;

    public int maxEnergy = 5;
    public int currentEnergy = 0;

    private Color[] redColors = new Color[]
    {
        Color.white,                     // белый
        new Color(1f, 0.15f, 0.15f),     // бледно красный
        Color.red,                       // €рко красный
        new Color(0.7f, 0f, 0f),         // немного тЄмный красный
        new Color(0.5f, 0f, 0f),         // тЄмно красный
        new Color(0.3f, 0f, 0f)          // еще темнее красный
    };

    public void Used(int energy, string wireColor) //цвета - Red, Green
    {
        currentEnergy += energy;

        switch (wireColor)
        {
            case "Red":
                spriteRenderer.color = redColors[currentEnergy];
                break;

            case "Green":
                spriteRenderer.color = new Color(1, 1f - 0.2f * currentEnergy, 1);
                break;
        }
    }
}
