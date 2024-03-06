using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface FormationsInterface
{
    List<Vector3> GetPositions(int unitCount, Vector3 target, float angle, float radius);
}
