using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricPole : BasisOfTheBuilding
{
    private void Start()
    {
        if (BuildCurrentEnergy == 0)
            BuildCurrentEnergy = BuildingCharacteristics.ThisBuildingInfo.MaxBuildingEnergy;

        CheckingElectricalNetwork();

        BuildingCharacteristics.TheMainScriptOfTheElectricalNetwork.AddElectricPoleInElectricalSystemList(BuildingCharacteristics.ThisScriptFromInspector, this);
        BuildingCharacteristics.NumberInTheElectricalSystem = BuildingCharacteristics.TheMainScriptOfTheElectricalNetwork.ElectricalSystemInfo.ElectricalSystemList.Count - 1;
    }
}
