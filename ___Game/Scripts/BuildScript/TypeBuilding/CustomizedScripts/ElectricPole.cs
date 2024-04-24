using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricPole : BasisOfTheBuilding
{
    [SerializeField] private CircleCollider2D circleCollider2D;
    private void Start()
    {
        _thisBuildingInfo = Resources.Load<BuildingInfo>("Builds/ElectricPole");

        circleCollider2D.radius = 4;

        if (BuildCurrentEnergy == 0)
            BuildCurrentEnergy = _thisBuildingInfo.MaxBuildingEnergy;
    }
}
