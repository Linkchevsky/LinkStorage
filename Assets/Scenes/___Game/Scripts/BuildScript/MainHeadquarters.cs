using Mirror;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class MainHeadquarters : NetworkBehaviour, BuildingInterface
{
    private SpecificationsBuilding _thisBuildingStats;
    [SyncVar] public int BuildCurrentEnergy;
    public int BuildMaxEnergy;

    private BoxCollider2D _boxCollider => this.GetComponent<BoxCollider2D>();
    private BuildingInterface _thisBuildingInterface => GetComponent<BuildingInterface>();
    private Transform _buildingSpawnPoint;

    public readonly List<string> ListOfSpawnUnits = new List<string>() { "ClassicUnit", "WarriorUnit" };
    private void Start() 
    {
        _buildingSpawnPoint = transform.GetChild(0);

        _thisBuildingStats = SpecificationsBuilding.GetBuildingData(BuildingTypeEnum.mainHeadquarters, gameObject);
        BuildMaxEnergy = _thisBuildingStats.BuildMaxEnergy;
        if (BuildCurrentEnergy == 0)
            BuildCurrentEnergy = _thisBuildingStats.BuildMaxEnergy;
    }



    public void Interaction() => CanvasControl.Instance.UsingTheBuildCanvas(_thisBuildingInterface, ListOfSpawnUnits, _buildingSpawnPoint);

    public void AddEnergy(int amountOfEnergy) => BuildCurrentEnergy += amountOfEnergy;
    public void RemoveEnergy(int amountOfEnergy) => BuildCurrentEnergy -= amountOfEnergy;

 
    public BoxCollider2D GetBoxCollider() { return _boxCollider; }
    public BuildingInterface GetBuildingInterface() { return _thisBuildingInterface; }
    public SpecificationsBuilding GetBuildingStats() { return _thisBuildingStats; }
    public int GetCurrentBuildingEnergy() { return BuildCurrentEnergy; }
}
