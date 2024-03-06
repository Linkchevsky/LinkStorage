using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitManagerScript : NetworkBehaviour
{
    public static UnitManagerScript Instance;

    public List<GameObject> AllUnit;

    public List<GameObject> PlayerUnit;

    public List<GameObject> TeamUnit;

    public List<GameObject> EnemyUnit;

    private void Awake() //объявляю синглтон
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        AllUnit = GameObject.FindGameObjectsWithTag("Bot").ToList<GameObject>();
        StartCoroutine(TimerForClearList());
    }

    private void ClearList()
    {
        AllUnit.RemoveAll(x => x == null);
        PlayerUnit.RemoveAll(x => x == null);
        TeamUnit.RemoveAll(x => x == null);
        EnemyUnit.RemoveAll(x => x == null);
    }

    private IEnumerator TimerForClearList()
    {
        yield return new WaitForSeconds(0.4f);
        ClearList();
        StartCoroutine(TimerForClearList());
    }
}
