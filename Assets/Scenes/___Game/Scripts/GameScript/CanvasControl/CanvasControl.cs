using Mirror;
using NSubstitute.ReceivedExtensions;
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
    public void UsingTheBuildCanvas(SpecificationsBuild buildStats, List<string> listOfSpawnUnits = null, Transform buildSpawnPoint = null)
    {
        _mainPanelGO.SetActive(true);

        _objectNameText.text = buildStats.buildType.ToString();
        _infoText.text = $"Energy = {buildStats.BuildCurrentEnergy}/{buildStats.BuildMaxEnergy}";

        if (listOfSpawnUnits != null)
        {
            _spawnMenuButton.SetActive(true);
            _spawnMenuScript.ButtonPlacement(listOfSpawnUnits, buildSpawnPoint);
        }
    }

    public void UsingWaitingForEnergyCanvas(string buildType, int currentUnitsCount, int requiredQuantityUnits)
    {
        _mainPanelGO.SetActive(true);

        _objectNameText.text = buildType;
        _infoText.text = $"В процессе постройки\nКол-во юнитов = {currentUnitsCount}/{requiredQuantityUnits}";
    }

    public void UsingTheSpawnMenuButton() => _spawnMenuButtonGOList.SetActive(!_spawnMenuButtonGOList.activeSelf);
    #endregion

    #region[unit]
    public void UsingTheUnitCanvas(SpecificationsUnit unitStats, List<string> listOfConstructions = null)
    {
        _mainPanelGO.SetActive(true);

        _objectNameText.text = unitStats.UnitType.ToString();
        _infoText.text = $"Energy = {unitStats.UnitCurrentEnergy}/{unitStats.UnitMaxEnergy}";

        switch(unitStats.UnitType.ToString())
        {
            case "ClassicUnit":
                _constructionScript.ButtonPlacement(listOfConstructions);
                _constructionsButtonGOList.SetActive(true);
                return;
        }
    }

    public void UsingTheUnitsCanvas(List<GameObject> selectedUnits)
    {
        _mainPanelGO.SetActive(true);

        _objectNameText.text = "Выделена группа";
        _infoText.text = $"Кол-во юнитов = {selectedUnits.Count}";

        _formationsButtonGOList.SetActive(true);
    }

    #endregion

    public void CloseAllCanvasMenu()
    {
        _spawnMenuButton.SetActive(false);
        _spawnMenuButtonGOList.SetActive(false);

        _formationsButtonGOList.SetActive(false);
        _constructionsButtonGOList.SetActive(false);

        _mainPanelGO.SetActive(false);
    }
}
