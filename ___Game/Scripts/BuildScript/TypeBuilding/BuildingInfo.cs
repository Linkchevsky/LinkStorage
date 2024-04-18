using UnityEngine;

[CreateAssetMenu(fileName = "UnitInfo", menuName = "Gameplay/New BuildingInfo")]
public class BuildingInfo : ScriptableObject
{
    [SerializeField] private string _id;
    [SerializeField] private GameObject _prefab;
    [SerializeField] private int _maxBuildingEnergy;
    [SerializeField] private int _numberOfUnitsToBuild;

    public string Id => this._id;
    public GameObject Prefab => this._prefab;
    public int MaxBuildingEnergy => this._maxBuildingEnergy;
    public int NumberOfUnitsToBuild => this._numberOfUnitsToBuild;
}
