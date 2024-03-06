using System;
using System.Collections.Generic;
using UnityEngine;

public class UnitControl : MonoBehaviour
{
    public List<GameObject> _allUnits = new List<GameObject>();
    public List<GameObject> AllUnits => _allUnits;

    public List<GameObject> SelectedUnits = new List<GameObject>();

    public static Action s_cancelingUnitSelection;

    public string CurrentFormationType = "Line";
    public static UnitControl Instance;
    private void Awake() //�������� ��������
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

    #region[��� ������]
    public void SetLineFormation() => CurrentFormationType = "Line";
    public void SetCircleFormation() => CurrentFormationType = "Circle";
    #endregion
}
