using System;
using UnityEngine;

public enum UnitTypeEnum
{
    ClassicUnit,
    WarriorUnit
}

public struct SpecificationsUnit
{
    #region[Инф поля]
    public UnitTypeEnum UnitType;

    public int UnitMaxEnergy;

    public float UnitSpeed;
    #endregion

    public static readonly SpecificationsUnit StaticClassicUnit = new SpecificationsUnit(UnitTypeEnum.ClassicUnit, 100, 5);
    public static readonly SpecificationsUnit StaticWarriorUnit = new SpecificationsUnit(UnitTypeEnum.WarriorUnit, 125, 4);

    public SpecificationsUnit(UnitTypeEnum unitType, int MaxEnergy, float Speed)
    {
        this.UnitType = unitType;
        UnitMaxEnergy = MaxEnergy;
        UnitSpeed = Speed;
    }

    public static SpecificationsUnit GetUnitData(UnitTypeEnum unitClass, GameObject theCallingObject)
    {
        switch (unitClass)
        {
            case UnitTypeEnum.ClassicUnit:
                return StaticClassicUnit;
            case UnitTypeEnum.WarriorUnit:
                return StaticWarriorUnit;
            default:
                throw new ArgumentException("Invalid unit class", nameof(unitClass));
        }
    }
}
