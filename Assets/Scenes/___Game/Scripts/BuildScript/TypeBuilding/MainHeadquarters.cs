using Mirror;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class MainHeadquarters : BasisOfTheBuilding
{
    private void Start()
    {
        _buildingSpawnPoint = transform.GetChild(0);

        _thisBuildingStats = SpecificationsBuilding.GetBuildingData(BuildingTypeEnum.mainHeadquarters, gameObject);
        BuildMaxEnergy = _thisBuildingStats.BuildingMaxEnergy;
        if (BuildCurrentEnergy == 0)
            BuildCurrentEnergy = _thisBuildingStats.BuildingMaxEnergy;
    }
}
