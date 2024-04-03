using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CanvasControl : MonoBehaviour
{
    [SerializeField] private GameObject _mainPanelGO;
    [Space]
    [SerializeField] private TextMeshProUGUI _objectNameText;
    [SerializeField] private TextMeshProUGUI _infoText;
    [Space]
    [SerializeField] private GameObject _spawnMenuButton;
    [SerializeField] private GameObject _spawnMenuButtonGOList;
    [SerializeField] private SpawnMenu _spawnMenuScript;
    [Space]
    [SerializeField] private GameObject _formationsButtonGOList;
    [SerializeField] private UnitControl _unitControlScript;
    [Space]
    [SerializeField] private Construction _constructionScript;
    [SerializeField] private GameObject _constructionsButtonGOList;

    public static CanvasControl Instance;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    #region[build]
    [HideInInspector] public bool UsedTheBuildCanvas = false;
    public void UsingTheBuildCanvas(SpecificationsBuild buildStats, List<string> listOfSpawnUnits = null, Transform buildSpawnPoint = null)
    {
        UsedTheBuildCanvas = true;

        _mainPanelGO.SetActive(true);

        _objectNameText.text = buildStats.buildType.ToString();
        _infoText.text = $"Energy = {buildStats.BuildCurrentEnergy}/{buildStats.BuildMaxEnergy}";

        if (listOfSpawnUnits != null)
        {
            _spawnMenuButton.SetActive(true);
            _spawnMenuScript.ButtonPlacement(listOfSpawnUnits, buildSpawnPoint);
        }
    }


    [HideInInspector] public bool UsedWaitingForEnergyCanvas = false;
    [HideInInspector] public GameObject UsedWaitingForEnergyCanvasGO = null;
    public void UsingWaitingForEnergyCanvas(string buildType, int currentUnitsCount, int requiredQuantityUnits, GameObject buildGO = null)
    {
        UsedWaitingForEnergyCanvasGO = buildGO;
        UsedWaitingForEnergyCanvas = true;

        _mainPanelGO.SetActive(true);

        _objectNameText.text = buildType;
        _infoText.text = $"В процессе постройки\nКол-во юнитов = {currentUnitsCount}/{requiredQuantityUnits}";
    }

    public void UsingTheSpawnMenuButton() => _spawnMenuButtonGOList.SetActive(!_spawnMenuButtonGOList.activeSelf);
    #endregion

    #region[unit]
    [HideInInspector] public bool UsedTheUnitCanvas = false;
    public void UsingTheUnitCanvas(SpecificationsUnit unitStats)
    {
        UsedTheUnitCanvas = true;

        _mainPanelGO.SetActive(true);

        _objectNameText.text = unitStats.UnitType.ToString();
        _infoText.text = $"Energy = {unitStats.UnitCurrentEnergy}/{unitStats.UnitMaxEnergy}";

        switch(unitStats.UnitType.ToString())
        {
            case "ClassicUnit":
                _constructionScript.ButtonPlacement(unitStats.UnitType.ToString());
                _constructionsButtonGOList.SetActive(true);
                return;
        }
    }

    [HideInInspector] public bool UsedTheUnitsCanvas = false;
    public void UsingTheUnitsCanvas(List<GameObject> selectedUnits)
    {
        UsedTheUnitsCanvas = true;

        _mainPanelGO.SetActive(true);

        _objectNameText.text = "Выделена группа";
        _infoText.text = $"Кол-во юнитов = {selectedUnits.Count}";

        _formationsButtonGOList.SetActive(true);
    }

    #endregion

    public void CloseAllCanvasMenu()
    {
        UsedTheBuildCanvas = false;
        UsedWaitingForEnergyCanvas = false;
        UsedTheUnitCanvas = false;
        UsedTheUnitsCanvas = false;

        _spawnMenuButton.SetActive(false);
        _spawnMenuButtonGOList.SetActive(false);

        _formationsButtonGOList.SetActive(false);
        _constructionsButtonGOList.SetActive(false);

        _mainPanelGO.SetActive(false);
    }
}
