using Mirror;
using Pathfinding;
using UnityEngine;

public class ClassicUnit : BasisOfTheUnits, UnitInterface
{
    private void Start()
    {
        UnitControl.Instance.AddUnitInAllUnit(this.gameObject);
        _thisUnitInfo = Resources.Load<UnitInfo>("Units/ClassicUnit");
        this.GetComponent<AIPath>().maxSpeed = _thisUnitInfo.UnitSpeed;

        if (UnitCurrentEnergy == 0)
            UnitCurrentEnergy = _thisUnitInfo.MaxUnitEnergy;

        if (!isOwned)
        {
            _mainCollider.isTrigger = false;
            return;
        }
    }
}