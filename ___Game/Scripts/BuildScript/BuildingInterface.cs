using Mirror;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public interface BuildingInterface
{
    void Interaction();


    void UsedEnergy(int amountOfEnergy);


    List<string> GetListOfSpawnUnits();
    GameObject GetGameobject();
    BoxCollider2D GetBoxCollider();
    BuildingInterface GetBuildingInterface();
    BuildingInfo GetBuildingInfo();
    int GetCurrentBuildingEnergy();
}
