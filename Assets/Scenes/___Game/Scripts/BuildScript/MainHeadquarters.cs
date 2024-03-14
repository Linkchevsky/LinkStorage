using Mirror;
using System.Collections.Generic;
using UnityEngine;

public class MainHeadquarters : NetworkBehaviour, BuildInterface
{
    private SpecificationsBuild _thisBuildSpecifications;

    [SerializeField] private Transform _buildSpawnPoint;

    public readonly List<string> ListOfSpawnUnits = new List<string>() { "ClassicUnit", "WarriorUnit" };

    private void Start() => _thisBuildSpecifications = SpecificationsBuild.GetBuildData(BuildTypeEnum.mainHeadquarters, gameObject);

    public void Interaction() => CanvasControl.Instance.UsingTheBuildCanvas(_thisBuildSpecifications, ListOfSpawnUnits, _buildSpawnPoint);
}