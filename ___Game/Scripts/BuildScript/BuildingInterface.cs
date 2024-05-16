using Mirror;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public interface BuildingInterface
{
    void Interaction();
    void UsedEnergy(int amountOfEnergy);
    void CheckingElectricalNetwork();
    void InstallationOfWires(List<GameObject> listOfBuildingsGO, bool theWireUsed = false);


    List<string> GetListOfSpawnUnits();
    GameObject GetGameobject();
    Collider2D GetBoxCollider();
    BuildingInterface GetBuildingInterface();
    BuildingInfo GetBuildingInfo();
    int GetCurrentBuildingEnergy();
    MainHeadquarter ReturnTheMainScriptOfTheElectricalNetwork();
    List<BuildingInterface> GetBuildingNeighbors();
    int GetBuildingNumberInElectricalNetwork();
}
