using Mirror;
using Pathfinding;
using UnityEngine;

public class WarriorUnit : BasisOfTheUnits, UnitInterface
{
    private void Start()
    {
        UnitControl.Instance.AddUnitInAllUnit(this.gameObject);
        thisUnitSpecifications = SpecificationsUnit.GetUnitData(UnitTypeEnum.WarriorUnit, this.gameObject);

        if (!isOwned)
        {
            _mainCollider.isTrigger = false;
            return; 
        }

        UnitMaxEnergy = thisUnitSpecifications.UnitMaxEnergy;
        UnitCurrentEnergy = UnitMaxEnergy;
    }
}