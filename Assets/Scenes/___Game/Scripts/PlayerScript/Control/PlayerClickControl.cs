using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
public class PlayerClickControl : MonoCache
{
    private void Awake()
    {
        Enabled += Enable;
        Disabled += Disable;
    }

    private void Enable()
    {
        EventPlayerControl.s_playerLeftClick += OnLeftClick;
        EventPlayerControl.s_playerRightClick += OnRightClick;

        EventPlayerControl.s_playerCloseClick += OnCloseClick;
    }
    private void Disable()
    {
        EventPlayerControl.s_playerLeftClick -= OnLeftClick;
        EventPlayerControl.s_playerRightClick -= OnRightClick;

        EventPlayerControl.s_playerCloseClick -= OnCloseClick;
    }

    #region[������ ����]
    private Vector3 _startPos;
    private Rect _selectionRect;
    private bool _isSelecting = false;

    private bool _choiceTheRotation = false;
    private List<Vector3> _unitsPositions = new List<Vector3>();
    private List<Transform> _unitTargetTransforms = new List<Transform>();
    private Vector3 _theCenterOfTheFormation;

    public string CurrentMode = "Movement"; // Movement , Construction
    public Transform BuildTransform;

    public void OnLeftClick(InputAction.CallbackContext context)
    {
        if (!isLocalPlayer) return;

        switch(context.phase.ToString())
        {
            case "Started":
                //�������� �� ui � ������� �������
                List<RaycastResult> results = new List<RaycastResult>();
                EventSystem.current.RaycastAll(new PointerEventData(EventSystem.current) { position = Input.mousePosition }, results);
                if (results.Count > 0)
                    return;

                _startPos = Input.mousePosition;
                _isSelecting = true;

                RaycastHit2D _hit = ReturnOfTheHit();

                OnCloseClick();

                if (_hit)
                {
                    switch (_hit.transform.gameObject.tag)
                    {
                        case "Unit":
                            PerformingAHitOnAUnit(_hit.transform);
                            return;
                        case "Build":
                            PerformingAHitOnABuilding(_hit.transform);
                            return;
                    }
                }
                return;

            case "Canceled":
                PerformingAHitOnAUnitsInRect();
                _isSelecting = false;
                return;
        }
    }

    private RaycastHit2D ReturnOfTheHit()
    {
        Vector2 _MousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D Hit = Physics2D.Raycast(_MousePosition, Vector2.zero);

        return Hit;
    }

    private void OnGUI()
    {
        if (!isLocalPlayer) return;

        if (_isSelecting)
        {
            _selectionRect = new Rect(Mathf.Min(Input.mousePosition.x, _startPos.x),
                   Screen.height - Mathf.Max(Input.mousePosition.y, _startPos.y),
                   Mathf.Max(Input.mousePosition.x, _startPos.x) - Mathf.Min(Input.mousePosition.x, _startPos.x),
                   Mathf.Max(Input.mousePosition.y, _startPos.y) - Mathf.Min(Input.mousePosition.y, _startPos.y)
                   );

            Texture2D texture = new Texture2D(1, 1);
            texture.SetPixel(0, 0, new Color(0f, 1f, 0f, 0.5f)); //��������� ������� �����
            texture.Apply();

            GUI.Box(_selectionRect, texture);
        }
        return;
    }

    private void PerformingAHitOnAUnitsInRect()
    {
        for (int j = 0; j < UnitControl.Instance.AllUnits.Count; j++)
        {
            Vector2 tmp = new Vector2(Camera.main.WorldToScreenPoint(UnitControl.Instance.AllUnits[j].transform.position).x, Screen.height - Camera.main.WorldToScreenPoint(UnitControl.Instance.AllUnits[j].transform.position).y);

            if (_selectionRect.Contains(tmp))
                UnitControl.Instance.AllUnits[j].GetComponent<UnitInterface>().Interaction();
        }

        if (UnitControl.Instance.SelectedUnits.Count > 1)
            CanvasControl.Instance.UsingTheUnitsCanvas(UnitControl.Instance.SelectedUnits);
    }

    private void PerformingAHitOnAUnit(Transform _hitTransform) => _hitTransform.GetComponent<UnitInterface>().Interaction(); 

    private void PerformingAHitOnABuilding(Transform _hitTransform) => _hitTransform.GetComponent<BuildInterface>().Interaction();


    public void OnRightClick(InputAction.CallbackContext context) 
    {
        if (!isLocalPlayer) return;

        if (CurrentMode == "Movement")
        {
            switch (context.phase.ToString())
            {
                case "Started":
                    foreach (GameObject unit in UnitControl.Instance.SelectedUnits)
                        _unitTargetTransforms.Add(unit.GetComponent<UnitInterface>().GetUnitTarget());

                    _theCenterOfTheFormation = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    if (UnitControl.Instance.SelectedUnits.Count == 1)
                    {
                        PlacementByUnitPositions("Point", _theCenterOfTheFormation);
                        return;
                    }
                    else if (UnitControl.Instance.SelectedUnits.Count > 1)
                    {
                        PlacementByUnitPositions(UnitControl.Instance.CurrentFormationType, _theCenterOfTheFormation);
                        _choiceTheRotation = true;
                    }
                    return;

                case "Canceled":
                    if (UnitControl.Instance.SelectedUnits.Count == 1)
                    {
                        _choiceTheRotation = false;
                        _unitTargetTransforms.Clear();
                        UnitControl.Instance.UnitSetDestination(_unitsPositions[0]);
                        return;
                    }
                    else
                    {
                        _choiceTheRotation = false;
                        _unitTargetTransforms.Clear();
                        UnitControl.Instance.UnitsSetDestination(_unitsPositions);
                        return;
                    }
            }
            return;
        }

        if (CurrentMode == "Construction")
        {
            switch (context.phase.ToString())
            {
                case "Canceled":
                    Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    Construction.Instance.UnitSpawning(new Vector3(mousePosition.x, mousePosition.y, 0));
                    return;
            }
            return;
        }
    }

    public override void OnTick() => Allocation();

    private void Allocation() //������ �� ������������
    {
        if (!isLocalPlayer) return;

        Vector3 _currentMousePosition;
        switch (CurrentMode)
        {
            case "Movement":
                if (_choiceTheRotation)
                {
                    _currentMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                    Vector3 _direction = _currentMousePosition - _theCenterOfTheFormation;

                    float _distance = Vector2.Distance(_theCenterOfTheFormation, _currentMousePosition);
                    float _angle;

                    if (_distance > 0.5f)
                    _angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;
                    else
                    _angle = 0;

                    PlacementByUnitPositions(UnitControl.Instance.CurrentFormationType, _theCenterOfTheFormation, _angle, _distance);
                }
                return;


            case "Construction":
                _currentMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                BuildTransform.position = new Vector3(_currentMousePosition.x, _currentMousePosition.y, 0);
                return;
        }
    }

    private void PlacementByUnitPositions(string formationType, Vector3 theCenterOfTheFormation, float angle = 0, float radius = 0.5f)
    {
        switch (formationType)
        {
            case "Point":
                _unitsPositions = new List<Vector3>() { new Vector3(theCenterOfTheFormation.x, theCenterOfTheFormation.y, 0) };
                _unitTargetTransforms[0].position = _unitsPositions[0];
                return;

            case "Circle":
                _unitsPositions = new CircleFormationForUnits().GetPositions(UnitControl.Instance.SelectedUnits.Count, theCenterOfTheFormation, angle, radius);
                for (int i = 0; i < _unitTargetTransforms.Count; i++)
                    _unitTargetTransforms[i].position = _unitsPositions[i];
                return;

            case "Line":
                _unitsPositions = new LineFormationForUnits().GetPositions(UnitControl.Instance.SelectedUnits.Count, theCenterOfTheFormation, angle, radius);
                for (int i = 0; i < _unitTargetTransforms.Count; i++)
                    _unitTargetTransforms[i].position = _unitsPositions[i];
                return;
        }
    }

    #endregion

    public void OnCloseClick()
    { 
        CanvasControl.Instance.CloseAllCanvasMenu();
        UnitControl.Instance.ClearSelectedUnitList();

        Construction.Instance.OnCloseClick();
        CurrentMode = "Movement";
    }
}
