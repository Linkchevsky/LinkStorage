using System;
using System.Collections.Generic;
using UnityEngine;

public class UnitControl : MonoBehaviour
{
    private List<GameObject> _allUnits = new List<GameObject>();
    public List<GameObject> AllUnits => _allUnits;

    [HideInInspector] public List<GameObject> SelectedUnits = new List<GameObject>();

    [HideInInspector] public string CurrentFormationType = "Line";

    public static Action s_cancelingUnitSelection;
    public static UnitControl Instance;
    private void Awake() //объявляю синглтон
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void ClearSelectedUnitList()
    {
        s_cancelingUnitSelection?.Invoke();
        SelectedUnits.Clear();
    }

    public void AddUnitInSelectedList(GameObject _unitGO) => SelectedUnits.Add(_unitGO);

    public void UnitSetDestination(Vector3 coordinates) => SelectedUnits[0].GetComponent<UnitInterface>().UnitSetDestination(coordinates);

    public void UnitsSetDestination(List<Vector3> unitsPositions)
    {
        for (int i = 0; i < SelectedUnits.Count; i++)
        {
            SelectedUnits[i].GetComponent<UnitInterface>().UnitSetDestination(unitsPositions[i]);
        }
    }

    #region[для кнопок]
    public void SetLineFormation() => CurrentFormationType = "Line";
    public void SetCircleFormation() => CurrentFormationType = "Circle";
    #endregion
}
