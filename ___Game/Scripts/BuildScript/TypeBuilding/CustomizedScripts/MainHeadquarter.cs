using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class MainHeadquarter : BasisOfTheBuilding
{
    [Space]
    [SerializeField] private MainHeadquarter _thisMainHeadquarterScriptFromInspector;

    public int ChargingPower = 0;
    public int FreeChargingPower = 0;

    public class ElectricalSystem
    {
        public List<BuildingInterface> ElectricalSystemList = new List<BuildingInterface>();
        public List<BuildingInterface> MainHeadquartersInElectricalList = new List<BuildingInterface>();
        public List<BuildingInterface> GeneratorsInElectricalList = new List<BuildingInterface>();
        public List<BuildingInterface> ElectricPolesInElectricalList = new List<BuildingInterface>();

        public int GeneratedEnergy = 0;
    }
    public ElectricalSystem ElectricalSystemInfo = new ElectricalSystem();


    public void SetChargingPower(int power)
    {
        ChargingPower += power;
        FreeChargingPower += power;

        SetBuildingChargingPower(1);
    }

    private void Start()
    {
        _buildingCharacteristics.ListOfSpawnUnits = new List<string>() { "ClassicUnit", "WarriorUnit" };
        _buildingCharacteristics.listOfAdditionalFunctionality = new List<string> { "spawnUnits" };

        if (BuildCurrentEnergy == 0)
            BuildCurrentEnergy = _buildingCharacteristics.ThisBuildingInfo.MaxBuildingEnergy;

        if (_buildingCharacteristics.TheMainScriptOfTheElectricalNetwork == null)
        {
            _buildingCharacteristics.TheMainScriptOfTheElectricalNetwork = _thisMainHeadquarterScriptFromInspector;
            AddInElectricalSystemList(_buildingCharacteristics.ThisScriptFromInspector);
        }
    }

    public void AddInElectricalSystemList(BuildingInterface buildingInterface)
    {
        ElectricalSystemInfo.ElectricalSystemList.Add(buildingInterface);

        switch (buildingInterface.GetBuildingCharacteristics().ThisBuildingInfo.Id)
        {
            case "Main Headquarter":
                ElectricalSystemInfo.MainHeadquartersInElectricalList.Add(buildingInterface);
                break;

            case "Electric Pole":
                ElectricalSystemInfo.ElectricPolesInElectricalList.Add(buildingInterface);
                break;

            case "Generator":
                ElectricalSystemInfo.GeneratedEnergy += 3;
                ElectricalSystemInfo.GeneratorsInElectricalList.Add(buildingInterface);
                TryingToGetPath(ElectricalSystemInfo.ElectricalSystemList.IndexOf(buildingInterface), 0, true);
                break;
        }
    }



    private void TryingToGetPath(int startIndex, int endIndex, bool forEnergy = false)//�� ��� ������ ������������ ������� ��� ���������� ���������� ����� ����
    {
        List<Vector3> path;

        try
        {
            path = FindPath(InitializeNodes(), Storage.Instance.AllBuildingsGO[0].transform.position, Storage.Instance.AllBuildingsGO[Storage.Instance.AllBuildingsGO.Count - 1].transform.position);
            path.Insert(0, Storage.Instance.AllBuildingsGO[0].transform.position);
        }
        catch
        {
            TryingToGetPath(startIndex, endIndex, forEnergy);
            return;
        }


        if (path != null && forEnergy)
        {
            for (int i = 0; i < path.Count - 1; i++)
            {
                Debug.Log(path[i]);

                foreach (GameObject go in Storage.Instance.AllBuildingsGO)
                {
                    if (go.transform.position == path[i])
                    {
                        Debug.Log(1);
                        List<ConnectedWire> connectedWiresList = Storage.Instance.AllBuildingsInterface[Storage.Instance.AllBuildingsGO.IndexOf(go)].GetConnectedWiresList();

                        foreach (ConnectedWire wire in connectedWiresList)
                        {
                            if (wire.CoordinatesOfTheTarget == path[i + 1])
                            {
                                Debug.Log(2);
                                Storage.Instance.WiresDictionary[wire.WirePosition].Used(3, "Red");
                                break;
                            }
                        }

                        continue;
                    }
                }
            }
        }
    }

    public List<Vector3> FindPath(List<Node> nodes, Vector3 start, Vector3 target)
    {
        Node startNode = GetClosestNode(nodes, start); // �������� ����, ��������� � ��������� �������
        Node targetNode = GetClosestNode(nodes, target); // �������� ����, ��������� � ������� �������

        List<Node> openSet = new List<Node>(); // �������� ������ ����� ��� ��������
        HashSet<Node> closedSet = new HashSet<Node>(); // �������� ������ �����, ������� ��� ���������

        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            Node currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].FCost < currentNode.FCost || (openSet[i].FCost == currentNode.FCost && openSet[i].HCost < currentNode.HCost))
                {
                    currentNode = openSet[i]; // ������� ���� � ���������� ���������� F
                }
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode == targetNode)
            {
                return RetracePath(startNode, targetNode); // ���� �������� �������� ����, ������ ����
            }

            foreach (Node neighbor in GetNeighbors(nodes, currentNode))
            {
                if (!neighbor.Walkable || closedSet.Contains(neighbor))
                {
                    continue; // ���������� ������������ ��� ��� ����������� ����
                }

                float newMovementCostToNeighbor = currentNode.GCost + Vector3.Distance(currentNode.Position, neighbor.Position);
                if (newMovementCostToNeighbor < neighbor.GCost || !openSet.Contains(neighbor))
                {
                    neighbor.GCost = newMovementCostToNeighbor;
                    neighbor.HCost = Vector3.Distance(neighbor.Position, targetNode.Position);
                    neighbor.Parent = currentNode;

                    if (!openSet.Contains(neighbor))
                    {
                        openSet.Add(neighbor); // ��������� ���� � �������� ������ ��� ���������� ��������
                    }
                }
            }
        }

        return null; // ���������� null, ���� ���� �� ������
    }

    // ����� ��� ��������� ���������� ���� � �������� �������
    private Node GetClosestNode(List<Node> nodes, Vector3 position)
    {
        Node closestNode = null;
        float minDistance = Mathf.Infinity;

        foreach (Node node in nodes)
        {
            float distance = Vector3.Distance(position, node.Position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestNode = node;
            }
        }

        return closestNode;
    }

    // ����� ��� ��������� �������� �����
    private List<Node> GetNeighbors(List<Node> nodes, Node node, float neighborRadius = 5f)
    {
        List<Node> neighbors = new List<Node>();

        foreach (var potentialNeighbor in nodes)
        {
            if (potentialNeighbor != node && Vector3.Distance(node.Position, potentialNeighbor.Position) <= neighborRadius)
            {
                neighbors.Add(potentialNeighbor); // ��������� ���� � ������ �������, ���� �� ��������� � �������� �������
            }
        }

        return neighbors;
    }

    // ����� ��� ���������� ����, ���������� ������ �������
    private List<Vector3> RetracePath(Node startNode, Node endNode)
    {
        List<Vector3> path = new List<Vector3>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode.Position);
            currentNode = currentNode.Parent; // ��������� � ������������� ����
        }

        path.Reverse(); // ������������� ����, ����� �������� ��� �� ������ �� ����
        return path;
    }
    List<Node> InitializeNodes()
    {
        List<Node> nodes = new List<Node>();

        // ��������� ������ ����� �� ������ ������ �������
        foreach (GameObject GO in Storage.Instance.AllBuildingsGO)
            nodes.Add(new Node(GO.transform.position, true)); // ����� ��� ��� ���� ���������

        return nodes;
    }
}

public class Node
{
    public Vector3 Position; // ������� ���� � ������������
    public bool Walkable; // ����� �� ������ ����� ���� ����
    public float GCost; // ��������� �� ������ �� ����� ����
    public float HCost; // �������������� ��������� �� ����� ���� �� ����
    public float FCost => GCost + HCost; // ����� ��������� (G + H)
    public Node Parent; // ������������ ����

    public Node(Vector3 position, bool walkable)
    {
        Position = position;
        Walkable = walkable;
    }
}