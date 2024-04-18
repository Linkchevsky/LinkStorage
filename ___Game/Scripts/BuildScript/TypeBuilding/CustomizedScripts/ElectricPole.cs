using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricPole : MonoBehaviour
{
    [SerializeField] private CircleCollider2D circleCollider2D;
    private void Start()
    {
        circleCollider2D.radius = 4;
    }
}
