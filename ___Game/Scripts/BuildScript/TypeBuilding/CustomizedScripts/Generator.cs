using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class Generator : BasisOfTheBuilding
{
    public int EnergyOutput = 0;
    public int EnergyGenerated = 3;

    private void Start()
    {
        if (BuildCurrentEnergy == 0)
            BuildCurrentEnergy = BuildingCharacteristics.ThisBuildingInfo.MaxBuildingEnergy;

        CheckingElectricalNetwork();

        BuildingCharacteristics.TheMainScriptOfTheElectricalNetwork.AddGeneratorInElectricalSystemList(BuildingCharacteristics.ThisScriptFromInspector, this);
        BuildingCharacteristics.NumberInTheElectricalSystem = BuildingCharacteristics.TheMainScriptOfTheElectricalNetwork.ElectricalSystemInfo.ElectricalSystemList.Count - 1;
    }
}
