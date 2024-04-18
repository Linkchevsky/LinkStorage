using UnityEngine;

[CreateAssetMenu(fileName = "UnitInfo", menuName = "Gameplay/New UnitInfo")]
public class UnitInfo : ScriptableObject
{
    [SerializeField] private string _id;
    [SerializeField] private GameObject _prefab;
    [SerializeField] private int _maxUnitEnergy;
    [SerializeField] private int _unitSpeed;

    public string Id => this._id;
    public GameObject Prefab => this._prefab;
    public int MaxUnitEnergy => this._maxUnitEnergy;
    public int UnitSpeed => this._unitSpeed;
}
