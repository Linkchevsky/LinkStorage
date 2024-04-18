using Mirror;
using Pathfinding;
using UnityEngine;

public class WarriorUnit : BasisOfTheUnits, UnitInterface
{
    private void Start()
    {
        UnitControl.Instance.AddUnitInAllUnit(this.gameObject);
        _thisUnitInfo = Resources.Load<UnitInfo>("Units/WarriorUnit");

        if (!isOwned)
        {
            _mainCollider.isTrigger = false;
            return;
        }

        UnitCurrentEnergy = _thisUnitInfo.MaxUnitEnergy;
        this.GetComponent<AIPath>().maxSpeed = _thisUnitInfo.UnitSpeed;
    }
}