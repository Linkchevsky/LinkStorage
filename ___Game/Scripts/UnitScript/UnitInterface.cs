using UnityEngine;

public interface UnitInterface
{
    void Interaction();
    void UnitSetDestination(Vector3 coordinate);

    void DestroyThisUnit();
    void UsedEnergy(int amountOfEnergy);

    Transform GetUnitTarget();
    UnitInterface GetUnitInterface();
    UnitInfo GetUnitInfo();
    int GetCurrentUnitEnergy();
}
