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
    void InstallationOfWires(List<GameObject> listOfBuildingsGO, List<Collider2D> notAddedInList);

    public GameObject GetGameobject();
    public Collider2D GetBoxCollider();
    public void SetBuildingChargingPower(int power);
    public BuildingCharacteristicsClass GetBuildingCharacteristics();
    public int GetEnergy();
}
