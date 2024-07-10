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

        _buildingChargingPower = _buildingInterface.GetBuildingCharacteristics().ChargingPowerTheBuilding;
    }

    public void ChangePower(int power)
    {
        int freeChargingPower = _buildingInterface.GetBuildingCharacteristics().TheMainScriptOfTheElectricalNetwork.ElectricalSystemInfo.FreeChargingPower;
        int buildingChargingPower = _buildingInterface.GetBuildingCharacteristics().ChargingPowerTheBuilding;


        MainHeadquarter mainHeadquarterScript = _buildingInterface.GetBuildingCharacteristics().TheMainScriptOfTheElectricalNetwork;

        if (freeChargingPower - power < 0 || buildingChargingPower + power < 0)
        {
            Debug.LogError("Ошибка мощности!");
            return;
        }

        mainHeadquarterScript.TryingToGetPath(mainHeadquarterScript.ElectricalSystemInfo.ElectricalSystemList.IndexOf(_buildingInterface), 0, power);

        _buildingChargingPower += power;
        _buildingInterface.SetBuildingChargingPower(power);

        _chargingControlText.text = $"Мощность: {_buildingChargingPower}";
    }

    public void ReduceButtonClick() => ChangePower(-1);
    public void AddButtonClick() => ChangePower(1);
}
