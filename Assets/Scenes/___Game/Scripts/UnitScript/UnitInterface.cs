using UnityEngine;

public interface UnitInterface
{
    void Interaction();
    void UnitSetDestination(Vector3 coordinate);

    SpecificationsUnit GetUnitSpecifications();
    Transform GetUnitTarget();

    void DestroyThisUnit();
}
