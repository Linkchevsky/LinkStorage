using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitingForEnergy : NetworkBehaviour, BuildInterface
{
    [SyncVar]
    public int _currentUnits;
    [SyncVar]
    private int _requiredUnits;

    [SerializeField] private BoxCollider2D _boxCollider;


    private bool _owned = false;

    [SyncVar]
    private string _buildType;

    [SyncVar(hook = nameof(IsReadyChange))]
    public bool _isReady = false;

    private void IsReadyChange(bool oldValue, bool newValue) 
    {
        if (!_owned)
            AddComponent(); 
    }

    public void Started(string buildType, bool isReady = false)
    {
        _owned = true;

        _buildType = buildType;
        _isReady = isReady;

        if (isReady)
        {
            AddComponent();
            return;
        }

        switch (_buildType)
        {
            case "MainHeadquarters":
                _requiredUnits = SpecificationsBuild.GetBuildData(BuildTypeEnum.mainHeadquarters, gameObject).BuildMaxEnergy / 100;
                break;

            case "TestBuild":
                _requiredUnits = SpecificationsBuild.GetBuildData(BuildTypeEnum.mainHeadquarters, gameObject).BuildMaxEnergy / 100;
                break;
        }
    }

    public void GetUnit()
    {
        _currentUnits++;
        if (_currentUnits >= _requiredUnits)
            AddComponent();
    }

    private void AddComponent()
    {
        switch (_buildType)
        {
            case "MainHeadquarters":
                gameObject.AddComponent<MainHeadquarters>();
                break;

            case "TestBuild":
                gameObject.AddComponent<MainHeadquarters>();
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
            }
        }
    }


    public void Interaction() => CanvasControl.Instance.UsingWaitingForEnergyCanvas(_buildType, _currentUnits, _requiredUnits);
}
