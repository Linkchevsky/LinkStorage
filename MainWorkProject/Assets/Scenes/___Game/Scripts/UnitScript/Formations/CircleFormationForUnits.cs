using System.Collections.Generic;
using UnityEngine;

struct CircleFormationForUnits : FormationsInterface
{
    public List<Vector3> GetPositions(int unitCount, Vector3 center, float angle = 0, float radius = 0.5f)
    {
        List<Vector3> _unitPositions = new List<Vector3>();

        Vector2 _center = new Vector2(center.x, center.y);
        angle *= Mathf.Deg2Rad;

        for (int i = 0; i < unitCount; i++)
        {
            float _angle = i * 2 * Mathf.PI / unitCount + angle;
            float _x = _center.x + radius * Mathf.Cos(_angle);
            float _y = _center.y + radius * Mathf.Sin(_angle);

            Vector2 _point = new Vector2(_x, _y);

            _unitPositions.Add(_point);
        }

        return _unitPositions;
    }
}
