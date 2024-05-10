using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using Color = UnityEngine.Color;

public class WorkPiece : MonoCache
{
    [SerializeField] private SpriteRenderer _spriteRenderer;

    public void Used(string buildingType)
    {
        _spriteRenderer.color = Color.green;

        switch (buildingType)
        {
            case "Main Headquarter":
                transform.localScale = new Vector3(2, 2, 0);
                break;
            case "Electric Pole":
                transform.localScale = new Vector3(1, 1, 0);
                break;
            case "Generator":
                transform.localScale = new Vector3(2, 1, 0);
                break;
        }
    }

    public SpriteRenderer SpriteRendererReturn() {  return _spriteRenderer; }

    public override void OnTick() => Allocation();
    private void Allocation()
    {
        if (Physics2D.OverlapBoxAll(transform.position, transform.localScale * 1.5f, 0f, LayerMask.GetMask("Obstacle")).Length != 0)
            _spriteRenderer.color = Color.red;
        else
            _spriteRenderer.color = Color.green;
    }
}
