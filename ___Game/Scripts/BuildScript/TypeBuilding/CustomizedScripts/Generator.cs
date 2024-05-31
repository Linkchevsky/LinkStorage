using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generator : BasisOfTheBuilding
{
    private void Start()
    {
        if (BuildCurrentEnergy == 0)
            BuildCurrentEnergy = _buildingCharacteristics.ThisBuildingInfo.MaxBuildingEnergy;
    }
}
