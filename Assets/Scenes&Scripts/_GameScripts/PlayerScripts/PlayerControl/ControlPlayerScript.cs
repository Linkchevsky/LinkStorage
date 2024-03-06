using UnityEngine;
using Mirror;
using System.Collections.Generic;

public class ControlPlayerScript : NetworkBehaviour
{
    public List<GameObject> UnitSelected = new List<GameObject>(); // массив выделенных юнитов
    public List<GameObject> PlayerUnitSelected = new List<GameObject>();

    public GUISkin skin;
    private Rect rect;
    private bool draw;
    private Vector2 startPos;
    private Vector2 endPos;

    private void Update()
    {
        if (!isLocalPlayer) return;

        if (Input.GetMouseButtonDown(1))
        {
            if (PlayerUnitSelected.Count > 0)
            {
                Vector2 _MousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                RaycastHit2D[] Hits = Physics2D.RaycastAll(_MousePosition, Vector2.zero, 9);
                foreach (RaycastHit2D Hit in Hits)
                {
                    if (Hit.transform.gameObject.layer == 9)
                    {
                        foreach (GameObject unit in PlayerUnitSelected)
                        {
                            unit.GetComponent<UnitEngineer>().Walk(Hit.transform.gameObject.GetComponent<CompartmentScript>());
                        }
                        return;
                    }
                }
            }
        }
    }

    private void OnGUI()
    {
        GUI.skin = skin;
        GUI.depth = 99;

        if (Input.GetMouseButtonDown(0))
        {
            Deselect();

            startPos = Input.mousePosition;

            SingleClick(startPos);

            draw = true;
        }

        if (Input.GetMouseButtonUp(0))
        {
            draw = false;
            Select();
        }

        if (draw)
        {
            UnitSelected.Clear();
            endPos = Input.mousePosition;
            if (startPos == endPos) return;

            rect = new Rect(Mathf.Min(endPos.x, startPos.x),
                            Screen.height - Mathf.Max(endPos.y, startPos.y),
                            Mathf.Max(endPos.x, startPos.x) - Mathf.Min(endPos.x, startPos.x),
                            Mathf.Max(endPos.y, startPos.y) - Mathf.Min(endPos.y, startPos.y)
                            );

            GUI.Box(rect, "");

            for (int j = 0; j < UnitManagerScript.Instance.AllUnit.Count; j++)
            {
                Vector2 tmp = new Vector2(Camera.main.WorldToScreenPoint(UnitManagerScript.Instance.AllUnit[j].transform.position).x, Screen.height - Camera.main.WorldToScreenPoint(UnitManagerScript.Instance.AllUnit[j].transform.position).y);

                if (rect.Contains(tmp))
                {
                    if (UnitSelected.Count == 0)
                    {
                        UnitSelected.Add(UnitManagerScript.Instance.AllUnit[j]);
                    }
                    else if (!CheckUnitInUnitSelected(UnitManagerScript.Instance.AllUnit[j]))
                    {
                        UnitSelected.Add(UnitManagerScript.Instance.AllUnit[j]);
                    }
                }
            }
        }
    }

    private void SingleClick(Vector2 PointOfSpace)
    {
        RaycastHit2D Hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(PointOfSpace), Vector2.zero, LayerMask.GetMask("Bot"));

        if (Hit.collider != null)
        {
            UnitSelected.Add(Hit.transform.gameObject);
            Select();
        }
    }

    // проверка, добавлен объект или нет
    private bool CheckUnitInUnitSelected(GameObject unit)
    {
        bool result = false;
        foreach (GameObject u in UnitSelected)
        {
            if (u == unit) result = true;
        }
        return result;
    }

    private void Select()
    {
        if (UnitSelected.Count > 0)
        {
            for (int j = 0; j < UnitSelected.Count; j++)
            {
                if (UnitManagerScript.Instance.PlayerUnit.Contains(UnitSelected[j]))
                {
                    UnitSelected[j].GetComponent<SpriteRenderer>().color = Color.green;
                    if (PlayerUnitSelected.Count == 0)
                        PlayerUnitSelected.Add(UnitSelected[j]);
                    else if (!PlayerUnitSelected.Contains(UnitSelected[j]))
                        PlayerUnitSelected.Add(UnitSelected[j]);
                }
            }
        }
    }

    private void Deselect()
    {
        if (PlayerUnitSelected.Count > 0)
        {
            for (int j = 0; j < PlayerUnitSelected.Count; j++)
            {
                PlayerUnitSelected[j].GetComponent<SpriteRenderer>().color = Color.white;
            }
            PlayerUnitSelected.Clear();
        }
    }
}
