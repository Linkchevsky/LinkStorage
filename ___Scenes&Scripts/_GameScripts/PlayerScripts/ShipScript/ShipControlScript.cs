using Mirror;
using Pathfinding;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ShipControlScript : NetworkBehaviour
{
    private GridGraph AstarGrid;
    public bool CanMove;

    private void Start()
    {
        AstarGrid = AstarPath.active.data.gridGraph;
    }

    private void Update()
    {
        if (!CanMove) return;
        float currentRotation = transform.rotation.eulerAngles.z;

        currentRotation += 10 * Time.deltaTime;

        if (currentRotation >= 360f)
        {
            currentRotation = 0f;
        }

        transform.rotation = Quaternion.Euler(0f, 0f, currentRotation);

        AstarGrid.rotation = new Vector3(-90 + transform.rotation.eulerAngles.z, 270f, 90f);
        transform.localPosition += new Vector3(1, 0, 0f) * 1 * Time.deltaTime;
        AstarGrid.center = new Vector3(transform.position.x - 0.6666f, transform.position.y, -1);
    }
}
