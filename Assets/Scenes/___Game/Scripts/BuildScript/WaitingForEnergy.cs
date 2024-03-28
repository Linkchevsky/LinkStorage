using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitingForEnergy : MonoBehaviour
{
    private string _buildType;

    public int _currentEnergy = 0;
    public int _requiredEnergy;

    public void Started(string buildType, bool isReady = false)
    {
        _buildType = buildType;
        Debug.Log(1);
        Debug.Log(_buildType);
        switch (_buildType)
        {
            case "MainHeadquarters":
                _requiredEnergy = SpecificationsBuild.GetBuildData(BuildTypeEnum.mainHeadquarters, gameObject).BuildMaxEnergy;
                break;

            case "TestBuild":
                _requiredEnergy = SpecificationsBuild.GetBuildData(BuildTypeEnum.mainHeadquarters, gameObject).BuildMaxEnergy;
                break;
        }

        if (isReady)
            GetEnergy(_requiredEnergy);
    }

    public void GetEnergy(int energy)
    {
        if ((_currentEnergy += energy) >= _requiredEnergy)
        {
            switch(_buildType)
            {
                case "MainHeadquarters":
                    gameObject.AddComponent<MainHeadquarters>();
                    break;

                case "TestBuild":
                    gameObject.AddComponent<MainHeadquarters>();
                    break;
            }

            Destroy(this.GetComponent<WaitingForEnergy>());
        }
    }
}
