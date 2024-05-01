using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasisOfTheBuilding : NetworkBehaviour, BuildingInterface
{
    private void OnEnable() => GlobalUpdate.s_energyTick += EnergyTick;
    private void OnDisable() => GlobalUpdate.s_energyTick -= EnergyTick;
    private void EnergyTick()
    {
        if (BuildCurrentEnergy > 0)
            UsedEnergy(-1);
    }


    protected BuildingInfo _thisBuildingInfo;
    [SyncVar] public int BuildCurrentEnergy;

    protected BoxCollider2D _boxCollider => this.GetComponent<BoxCollider2D>();
    protected BuildingInterface _thisBuildingInterface => GetComponent<BuildingInterface>();

    protected List<string> ListOfSpawnUnits = null;
    protected List<string> listOfAdditionalFunctionality = null;

    protected bool canvasUsed;

    public void Interaction() 
    {
        canvasUsed = true;
        CanvasControl.Instance.deselectFromCanvas += Deselect;

        CanvasControl.Instance.UsingCanvas(_thisBuildingInfo.Id, $"{BuildCurrentEnergy}/{_thisBuildingInfo.MaxBuildingEnergy}", null, listOfAdditionalFunctionality, _thisBuildingInterface); 
    }
    public void Deselect()
    {
        canvasUsed = false;
        CanvasControl.Instance.deselectFromCanvas -= Deselect;
    }

    public void UsedEnergy(int amountOfEnergy)
    {
        BuildCurrentEnergy += amountOfEnergy;
        if (canvasUsed)
            CanvasControl.Instance.EnergyChangeAction?.Invoke($"{BuildCurrentEnergy}/{_thisBuildingInfo.MaxBuildingEnergy}");
    }

    public List<string> GetListOfSpawnUnits() { return ListOfSpawnUnits; }
    public GameObject GetGameobject() { return this.gameObject; }
    public BoxCollider2D GetBoxCollider() { return _boxCollider; }
    public BuildingInterface GetBuildingInterface() { return _thisBuildingInterface; }
    public BuildingInfo GetBuildingInfo() { return _thisBuildingInfo; }
    public int GetCurrentBuildingEnergy() { return BuildCurrentEnergy; }
}
