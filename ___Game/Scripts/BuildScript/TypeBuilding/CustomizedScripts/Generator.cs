using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generator : BasisOfTheBuilding
{
    private void Start()
    {
        _thisBuildingInfo = Resources.Load<BuildingInfo>("Builds/Generator");

        if (BuildCurrentEnergy == 0)
            BuildCurrentEnergy = _thisBuildingInfo.MaxBuildingEnergy;
    }
}
