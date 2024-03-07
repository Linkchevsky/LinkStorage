using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Storage : MonoBehaviour
{
    public static Storage Instance;

    public GameObject ClassicUnitPrefab;
    public GameObject WarriorUnitPrefab;

    private void Awake() //объявляю синглтон
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
}
