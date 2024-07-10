using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricPole : BasisOfTheBuilding
{
    private void Start()
    {
        if (BuildCurrentEnergy == 0)
            BuildCurrentEnergy = _buildingCharacteristics.ThisBuildingInfo.MaxBuildingEnergy;

        CheckingElectricalNetwork();

        _buildingCharacteristics.TheMainScriptOfTheElectricalNetwork.AddElectricPoleInElectricalSystemList(_buildingCharacteristics.ThisScriptFromInspector, this);
        _buildingCharacteristics.NumberInTheElectricalSystem = _buildingCharacteristics.TheMainScriptOfTheElectricalNetwork.ElectricalSystemInfo.ElectricalSystemList.Count - 1;
    }
}
