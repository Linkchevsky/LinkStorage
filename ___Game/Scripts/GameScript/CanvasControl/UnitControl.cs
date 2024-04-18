using System;
using System.Collections.Generic;
using UnityEngine;

public class UnitControl : MonoBehaviour
{
    private List<GameObject> _allUnits = new List<GameObject>();
    public List<GameObject> AllUnits => _allUnits;


    private List<GameObject> _selectedUnits = new List<GameObject>();
    public List<GameObject> SelectedUnits => _selectedUnits;

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

    #region[All]
    public void AddUnitInAllUnit(GameObject unit) => _allUnits.Add(unit);

    public void RemoveUnitFromAllUnits(GameObject unit) => _allUnits.Remove(unit);
    #endregion


    #region[Selected]
    public void AddUnitInSelectedList(GameObject _unitGO) =>  _selectedUnits.Add(_unitGO);

    public void RemoveUnitFromSelectedUnits(GameObject unit) => _selectedUnits.Remove(unit);


    public void ClearSelectedUnitList()
    {
        s_cancelingUnitSelection?.Invoke();
        _selectedUnits.Clear();
    }
    #endregion


    public void UnitSetDestination(Vector3 coordinates) => _selectedUnits[0].GetComponent<UnitInterface>().UnitSetDestination(coordinates);

    public void UnitsSetDestination(List<Vector3> unitsPositions)
    {
        for (int i = 0; i < _selectedUnits.Count; i++)
        {
            _selectedUnits[i].GetComponent<UnitInterface>().UnitSetDestination(unitsPositions[i]);
        }
    }

    #region[для кнопок]
    public void SetLineFormation() => CurrentFormationType = "Line";
    public void SetCircleFormation() => CurrentFormationType = "Circle";
    #endregion
}
