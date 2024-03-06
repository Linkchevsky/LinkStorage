using System.Collections.Generic;
using UnityEngine;

struct LineFormationForUnits : FormationsInterface
{
    public List<Vector3> GetPositions(int unitCount, Vector3 center, float angle = 0, float radius = 0)
    {
        List<Vector3> _unitPositions = new List<Vector3>();

        center += new Vector3(0.5f, -0.5f, 0);
        angle *= Mathf.Deg2Rad;

        float _distanceBetweenPoints = 1f;
        float diameter = 2 + radius;


        float _MaxPointsPerLine;
        float _MaxNumberOfColumns;
        if (unitCount * _distanceBetweenPoints < diameter)
        {
            _MaxPointsPerLine = unitCount;
            _MaxNumberOfColumns = 1;
        }
        else
        {
            _MaxPointsPerLine = Mathf.Floor(diameter / _distanceBetweenPoints);
            _MaxNumberOfColumns = Mathf.Ceil(unitCount / _MaxPointsPerLine);
        }


        float _currentLength = 0;
        float _currentHeight = 0;

        for (int i = 0; i < unitCount; i++)
        {
            if (_currentLength < _MaxPointsPerLine)
            {
                _unitPositions.Add(new Vector3(
                    center.x - _MaxPointsPerLine / 2 * _distanceBetweenPoints + _distanceBetweenPoints * _currentLength, 
                    center.y + _MaxNumberOfColumns / 2 * _distanceBetweenPoints - _distanceBetweenPoints * _currentHeight, 0));


                _currentLength += 1;
            }
            else
            {
                _currentLength = 0;
                _currentHeight += 1;

                _unitPositions.Add(new Vector3(
                    center.x - _MaxPointsPerLine / 2 * _distanceBetweenPoints + _distanceBetweenPoints * _currentLength,
                    center.y + _MaxNumberOfColumns / 2 * _distanceBetweenPoints - _distanceBetweenPoints * _currentHeight, 0));

                _currentLength += 1;
            }
        }

        for (int i = 0; i < _unitPositions.Count; i++)
        {
            Vector3 offset = _unitPositions[i] - center;

            float rotatedX = offset.x * Mathf.Cos(angle) - offset.y * Mathf.Sin(angle);
            float rotatedY = offset.x * Mathf.Sin(angle) + offset.y * Mathf.Cos(angle);

            Vector3 rotatedPoint = new Vector3(rotatedX, rotatedY, 0) + center;
            _unitPositions[i] = new Vector3(rotatedPoint.x, rotatedPoint.y, 0);
        }

        return _unitPositions;
    }
}
