using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Construction : MonoBehaviour
{
    public static Construction Instance;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void ChoseABuilding() => EventPlayerControl.s_playerRightClick += RightClick;
    private void OnDisable() => EventPlayerControl.s_playerRightClick -= RightClick;
    private void OnDestroy() => EventPlayerControl.s_playerRightClick -= RightClick;

    private void RightClick(InputAction.CallbackContext context)
    {
        Debug.Log("Нажал");
    }



    private List<GameObject> _buttonsUsed = new List<GameObject>();

    [SerializeField] private GameObject _mainHeadquartersButton;
    [SerializeField] private GameObject _testBuildingButton;
    public void ButtonPlacement(List<string> listOfConstructions)
    {
        _buttonsUsed.Clear();
        TurningOffAllButtons();

        foreach (string UnitName in listOfConstructions)
        {
            switch (UnitName)
            {
                case "MainHeadquarters":
                    _mainHeadquartersButton.SetActive(true);
                    _buttonsUsed.Add(_mainHeadquartersButton);
                    break;

                case "TestBuild":
                    _testBuildingButton.SetActive(true);
                    _buttonsUsed.Add(_testBuildingButton);
                    break;
            }
        }

        for (int i = 0; i < _buttonsUsed.Count; i++)
        {
            _buttonsUsed[i].transform.localPosition = new Vector3(0 - 110 * i, 0, 0);
        }
    }

    private void TurningOffAllButtons()
    {
        _mainHeadquartersButton.SetActive(false);
        _testBuildingButton.SetActive(false);
    }
}
