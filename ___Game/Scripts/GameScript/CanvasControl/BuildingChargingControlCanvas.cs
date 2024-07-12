using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static BasisOfTheBuilding;

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

    public void ChangePower(int power, bool added)
    {
        BuildingCharacteristicsClass buildingCharacteristics = _buildingInterface.GetBuildingCharacteristics();

        int freeChargingPower = buildingCharacteristics.TheMainScriptOfTheElectricalNetwork.ElectricalSystemInfo.FreeChargingPower;
        int buildingChargingPower = buildingCharacteristics.ChargingPowerTheBuilding;

        MainHeadquarter mainHeadquarterScript = buildingCharacteristics.TheMainScriptOfTheElectricalNetwork;

        if (freeChargingPower - power < 0 || buildingChargingPower + power < 0)
        {
            Debug.LogError("Ошибка мощности!");
            return;
        }


        if (added)
        {
            if (!mainHeadquarterScript.EnergyTransfer(0, 0, power))
            {
                Debug.LogError("Ошибка поиска пути!");
                return;
            }
        }
        else
        {
            List<List<Vector3>> pathsList = buildingCharacteristics.EnergyPaths;

            if (pathsList.Count > 0)
            {
                foreach (List<Vector3> path in pathsList)
                {
                    mainHeadquarterScript.ElectricalSystemInfo.GeneratorsInElectricalList[path[path.Count - 1]].EnergyOutput += power;

                    for (int i = 0; i < path.Count - 1; i++)
                    {
                        Vector3 wirePosition = (path[i] + path[i + 1]) / 2f;
                        wirePosition = new Vector3((float)Math.Round(wirePosition.x * 100) / 100, (float)Math.Round(wirePosition.y * 100) / 100, 0);

                        Storage.Instance.WiresDictionary[wirePosition].Used(power, "Red");
                        continue;
                    }
                }
            }
        }


        _buildingChargingPower += power;
        _buildingInterface.SetBuildingChargingPower(power);

        _chargingControlText.text = $"Мощность: {_buildingChargingPower}";
    }

    public void ReduceButtonClick() => ChangePower(-1, false);
    public void AddButtonClick() => ChangePower(1, true);
}
