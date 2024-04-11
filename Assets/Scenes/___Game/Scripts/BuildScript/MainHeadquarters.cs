using Mirror;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class MainHeadquarters : NetworkBehaviour, BuildInterface
{
    private SpecificationsBuild _thisBuildSpecifications;
    private BoxCollider2D _boxCollider => this.GetComponent<BoxCollider2D>();

    private Transform _buildSpawnPoint;

    public readonly List<string> ListOfSpawnUnits = new List<string>() { "ClassicUnit", "WarriorUnit" };

    private void Start() 
    {
        _buildSpawnPoint = transform.GetChild(0);
        _thisBuildSpecifications = SpecificationsBuild.GetBuildData(BuildTypeEnum.mainHeadquarters, gameObject); 
    }

    public void Interaction() => CanvasControl.Instance.UsingTheBuildCanvas(_thisBuildSpecifications, ListOfSpawnUnits, _buildSpawnPoint);

    public BoxCollider2D GetBoxCollider() { return _boxCollider; }
}
