using System;
using UnityEngine;

public enum BuildTypeEnum
{
    mainHeadquarters
}

public struct SpecificationsBuild
{
    #region[Инф поля]
    public BuildTypeEnum buildType;

    public float buildMaxEnergy;
    public float buildCurrentEnergy;
    #endregion

    private static readonly SpecificationsBuild StaticMainHeadquartersBuild = new SpecificationsBuild(BuildTypeEnum.mainHeadquarters, 1000);

    public SpecificationsBuild(BuildTypeEnum buildTypeName, float maxEnergy)
    {
        buildType = buildTypeName;

        buildMaxEnergy = maxEnergy;
        buildCurrentEnergy = maxEnergy;
    }

    public static SpecificationsBuild GetBuildData(BuildTypeEnum buildClass, GameObject theCallingObject)
    {
        switch (buildClass)
        {
            case BuildTypeEnum.mainHeadquarters:
                //theCallingObject.AddComponent<MainHeadquarters>();
                return StaticMainHeadquartersBuild;
            default:
                throw new ArgumentException("Invalid build class", nameof(buildClass));
        }
    }
}
