using Mirror;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using static BasisOfTheBuilding;
using Vector3 = UnityEngine.Vector3;

public interface BuildingInterface
{
    void Interaction();
    void UsedEnergy(int amountOfEnergy);
    void CheckingElectricalNetwork();
    void InstallationOfWires(List<GameObject> listOfBuildingsGO);

    public GameObject GetGameobject();
    public Collider2D GetBoxCollider();
    public void SetBuildingChargingPower(int power);
    public BuildingCharacteristics GetBuildingCharacteristics();
    public int GetEnergy();
}
