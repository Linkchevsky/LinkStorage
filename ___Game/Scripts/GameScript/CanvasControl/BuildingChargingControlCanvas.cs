using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildingChargingControlCanvas : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _chargingControlText;

    private BuildingInterface _buildingInterface;

    private int _buildingChargingPower;

    public void Used(BuildingInterface buildingInterface)
    {
        _buildingInterface = buildingInterface;

        _buildingChargingPower = _buildingInterface.GetBuildingCharacteristics().ChargingTheBuilding;
        Change(0);
    }

    public void Change(int power)
    {
        int freeChargingPower = _buildingInterface.GetBuildingCharacteristics().TheMainScriptOfTheElectricalNetwork.FreeChargingPower;
        int buildingChargingPower = _buildingInterface.GetBuildingCharacteristics().ChargingTheBuilding;

        if (freeChargingPower - power < 0 || buildingChargingPower + power < 0)
        {
            Debug.LogError("Ошибка мощности!");
            return;
        }

        _buildingChargingPower += power;
        _buildingInterface.SetBuildingChargingPower(power);

        _chargingControlText.text = $"Мощность: {_buildingChargingPower}";
    }

    public void ReduceButtonClick() => Change(-1);
    public void AddButtonClick() => Change(1);
}
