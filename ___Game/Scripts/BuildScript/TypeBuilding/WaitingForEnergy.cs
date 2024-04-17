using Mirror;
using System;
using UnityEngine;

public class WaitingForEnergy : NetworkBehaviour, BuildingInterface
{
    [SyncVar]
    public int _currentUnits;
    [SyncVar]
    private int _requiredUnits;

    private SpecificationsBuilding _thisBuildingStats;
    private BoxCollider2D _boxCollider => this.GetComponent<BoxCollider2D>();
    private BuildingInterface _thisBuildingInterface => GetComponent<BuildingInterface>();


    private bool _owned = false;

    [SyncVar]
    private string _buildType;

    [SyncVar(hook = nameof(IsReadyChange))]
    public bool _isReady = false;

    private void IsReadyChange(bool oldValue, bool newValue) 
    {
        if (!_owned)
            EnableComponent(); 
    }

    public void Started(string buildType, bool isReady = false)
    {
        _owned = true;

        _buildType = buildType;
        _isReady = isReady;

        if (isReady)
        {
            EnableComponent();
            return;
        }

        switch (_buildType)
        {
            case "MainHeadquarters":
                _requiredUnits = SpecificationsBuilding.GetBuildingData(BuildingTypeEnum.mainHeadquarters, gameObject).NumberOfUnitsToBuild;
                break;

            case "TestBuild":
                _requiredUnits = SpecificationsBuilding.GetBuildingData(BuildingTypeEnum.mainHeadquarters, gameObject).NumberOfUnitsToBuild;
                break;
        }
    }

    public void GetUnit()
    {
        _currentUnits++;
        if (_currentUnits >= _requiredUnits)
            EnableComponent();
    }

    private void EnableComponent()
    {
        switch (_buildType)
        {
            case "MainHeadquarters":
                if (CanvasControl.Instance.UsedWaitingForEnergyCanvas && CanvasControl.Instance.UsedWaitingForEnergyCanvasGO == this.gameObject)
                    CanvasControl.Instance.CloseAllCanvasMenu();

                gameObject.GetComponent<MainHeadquarters>().enabled = true;
                break;

            case "TestBuild":
                if (CanvasControl.Instance.UsedWaitingForEnergyCanvas && CanvasControl.Instance.UsedWaitingForEnergyCanvasGO == this.gameObject)
                    CanvasControl.Instance.CloseAllCanvasMenu();

                gameObject.GetComponent<MainHeadquarters>().enabled = true;
                break;
        }


        _isReady = true;

        Destroy(this.GetComponent<WaitingForEnergy>());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Unit")
        {
            UnitInterface unitInterface = collision.GetComponent<UnitInterface>();
            if (_boxCollider.bounds.Contains(unitInterface.GetUnitTarget().position))
            {
                unitInterface.DestroyThisUnit();
                GetUnit();

                if (CanvasControl.Instance.UsedWaitingForEnergyCanvas && CanvasControl.Instance.UsedWaitingForEnergyCanvasGO == this.gameObject)
                    TriggerInteraction();
            }
        }
    }

    private void TriggerInteraction() => CanvasControl.Instance.UsingWaitingForEnergyCanvas(_buildType, _currentUnits, _requiredUnits, this.gameObject);
    public void Interaction() => CanvasControl.Instance.UsingWaitingForEnergyCanvas(_buildType, _currentUnits, _requiredUnits, this.gameObject);

    public BoxCollider2D GetBoxCollider() { return _boxCollider; }
    public BuildingInterface GetBuildingInterface() { return _thisBuildingInterface; }
    public SpecificationsBuilding GetBuildingStats() { return _thisBuildingStats; }
    public int GetCurrentBuildingEnergy() { throw new ArgumentException("У заготовки не может быть энергии!"); }

    public void UsedEnergy(int amountOfEnergy) => throw new ArgumentException("У заготовки не может быть энергии!");
}
