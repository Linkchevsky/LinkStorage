using Mirror;
using System.Collections.Generic;
using UnityEngine;

public class MainHeadquarters : NetworkBehaviour, BuildInterface
{
    public SpecificationsBuild thisBuildSpecifications;

    [SerializeField] private Transform _buildSpawnPoint;

    public readonly List<string> listOfSpawnUnits = new List<string>() { "ClassicUnit", "WarriorUnit" };

    private void Start() => thisBuildSpecifications = SpecificationsBuild.GetBuildData(BuildTypeEnum.mainHeadquarters, gameObject);

    public void Interaction() => CanvasControl.Instance.UsingTheBuildCanvas(thisBuildSpecifications, listOfSpawnUnits, _buildSpawnPoint);
}
