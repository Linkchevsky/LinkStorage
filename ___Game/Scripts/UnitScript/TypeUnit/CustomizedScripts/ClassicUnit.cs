using Mirror;
using Pathfinding;
using UnityEngine;

public class ClassicUnit : BasisOfTheUnits, UnitInterface
{
    private void Start()
    {
        UnitControl.Instance.AddUnitInAllUnit(this.gameObject);
        _thisUnitInfo = Resources.Load<UnitInfo>("Units/ClassicUnit");

        if (!isOwned)
        {
            _mainCollider.isTrigger = false;
            return;
        }

        UnitCurrentEnergy = _thisUnitInfo.MaxUnitEnergy;
        this.GetComponent<AIPath>().maxSpeed = _thisUnitInfo.UnitSpeed;
    }
}