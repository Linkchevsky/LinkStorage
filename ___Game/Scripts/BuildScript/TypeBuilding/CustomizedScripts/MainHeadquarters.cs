using Mirror;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class MainHeadquarters : BasisOfTheBuilding
{
    private void Start()
    {
        ListOfSpawnUnits = new List<string>() { "ClassicUnit", "WarriorUnit" };
        listOfAdditionalFunctionality = new List<string> { "spawnUnits" };

        _thisBuildingInfo = Resources.Load<BuildingInfo>("Builds/MainHeadquarters");

        if (BuildCurrentEnergy == 0)
            BuildCurrentEnergy = _thisBuildingInfo.MaxBuildingEnergy;
    }
}
