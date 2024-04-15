using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Storage : MonoBehaviour
{
    public static Storage Instance;

    public GameObject ClassicUnitPrefab;
    public GameObject WarriorUnitPrefab;
    [Space]
    public GameObject MainHeadquartersPrefab;
    public GameObject TestBuildingPrefab;


    private void Awake() //объявляю синглтон
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public static Action s_energyTick;
    private void Start() => StartCoroutine(ExecuteAfterTime());
    private IEnumerator ExecuteAfterTime()
    {
        while (true)
        {
            s_energyTick?.Invoke();
            yield return new WaitForSeconds(1);
        }
    }
}
