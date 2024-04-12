using UnityEngine;

public interface UnitInterface
{
    void Interaction();
    void UnitSetDestination(Vector3 coordinate);

    Transform GetUnitTarget();
    UnitInterface GetUnitInterface();
    SpecificationsUnit GetUnitStats();
    int GetCurrentUnitEnergy();

    void DestroyThisUnit();
}
