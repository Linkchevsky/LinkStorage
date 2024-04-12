using Mirror;
using System.Numerics;
using UnityEngine;

public interface BuildingInterface
{
    void Interaction();


    void RemoveEnergy(int amountOfEnergy);


    BoxCollider2D GetBoxCollider();
    BuildingInterface GetBuildingInterface();
    SpecificationsBuilding GetBuildingStats();
    int GetCurrentBuildingEnergy();
}
