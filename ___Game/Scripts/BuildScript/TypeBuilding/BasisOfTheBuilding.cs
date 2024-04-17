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


    protected SpecificationsBuilding _thisBuildingStats;
    [SyncVar] public int BuildCurrentEnergy;
    public int BuildMaxEnergy;

    protected BoxCollider2D _boxCollider => this.GetComponent<BoxCollider2D>();
    protected BuildingInterface _thisBuildingInterface => GetComponent<BuildingInterface>();
    protected Transform _buildingSpawnPoint;

    public readonly List<string> ListOfSpawnUnits = new List<string>() { "ClassicUnit", "WarriorUnit" };


    public void Interaction() => CanvasControl.Instance.UsingTheBuildCanvas(_thisBuildingInterface, this.gameObject, ListOfSpawnUnits, _buildingSpawnPoint);

    public void UsedEnergy(int amountOfEnergy)
    {
        BuildCurrentEnergy += amountOfEnergy;
        if (CanvasControl.Instance.UsedTheBuildCanvas && CanvasControl.Instance.UsedTheBuildCanvasGO == this.gameObject)
            CanvasControl.Instance.UsingTheBuildCanvas(_thisBuildingInterface, this.gameObject, ListOfSpawnUnits, _buildingSpawnPoint);
    }


    public BoxCollider2D GetBoxCollider() { return _boxCollider; }
    public BuildingInterface GetBuildingInterface() { return _thisBuildingInterface; }
    public SpecificationsBuilding GetBuildingStats() { return _thisBuildingStats; }
    public int GetCurrentBuildingEnergy() { return BuildCurrentEnergy; }
}
