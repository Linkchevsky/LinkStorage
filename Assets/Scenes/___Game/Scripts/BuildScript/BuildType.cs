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

    public int BuildMaxEnergy;
    public int BuildCurrentEnergy; //главное чтобы делилось на 100

    public int NumberOfUnitsToBuild;
    #endregion

    private static readonly SpecificationsBuild StaticMainHeadquartersBuild = new SpecificationsBuild(BuildTypeEnum.mainHeadquarters, 1000, 4);

    public SpecificationsBuild(BuildTypeEnum buildTypeName, int maxEnergy, int numberOfUnitsToBuild)
    {
        buildType = buildTypeName;

        BuildMaxEnergy = maxEnergy;
        BuildCurrentEnergy = maxEnergy;

        NumberOfUnitsToBuild = numberOfUnitsToBuild;
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
