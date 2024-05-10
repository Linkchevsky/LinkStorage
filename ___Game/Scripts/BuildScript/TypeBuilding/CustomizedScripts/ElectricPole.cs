using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricPole : BasisOfTheBuilding
{
    private void Start()
    {
        if (BuildCurrentEnergy == 0)
            BuildCurrentEnergy = _thisBuildingInfo.MaxBuildingEnergy;
    }
}
