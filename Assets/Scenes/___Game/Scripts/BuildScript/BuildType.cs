using System;
using UnityEngine;

public enum BuildingTypeEnum
{
    mainHeadquarters
}

public struct SpecificationsBuilding
{
    #region[Инф поля]
    public BuildingTypeEnum buildType;

    public int BuildingMaxEnergy; //главное чтобы делилось на 100

    public int NumberOfUnitsToBuild;
    #endregion

    private static readonly SpecificationsBuilding StaticMainHeadquartersBuilding = new SpecificationsBuilding(BuildingTypeEnum.mainHeadquarters, 100, 4);

    public SpecificationsBuilding(BuildingTypeEnum buildingTypeName, int maxEnergy, int numberOfUnitsToConstruction)
    {
        buildType = buildingTypeName;
        BuildingMaxEnergy = maxEnergy;
        NumberOfUnitsToBuild = numberOfUnitsToConstruction;
    }

    public static SpecificationsBuilding GetBuildingData(BuildingTypeEnum buildClass, GameObject theCallingObject)
    {
        switch (buildClass)
        {
            case BuildingTypeEnum.mainHeadquarters:
                return StaticMainHeadquartersBuilding;
            default:
                throw new ArgumentException("Invalid build class", nameof(buildClass));
        }
    }
}
