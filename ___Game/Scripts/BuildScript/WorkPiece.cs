using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkPiece : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [Space]
    private int _collidersCount;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Build")
        {
            _collidersCount++;
            _spriteRenderer.color = Color.red;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Build")
        {
            _collidersCount--;

            if (_collidersCount == 0)
                _spriteRenderer.color = Color.green;
        }
    }
}
