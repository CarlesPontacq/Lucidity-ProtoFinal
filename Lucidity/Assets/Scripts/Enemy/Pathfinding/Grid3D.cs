using NUnit.Framework;
using UnityEngine;
using Lucidity;
using System.Collections.Generic;
using System.Collections;

public class Grid3D : MonoBehaviour
{
    public bool displayGridGizmos;
    public LayerMask unwalkableMask;
    public Vector2 gridWorldSize; // x es Ancho, y es Profundidad (Z)
    public float nodeRadius;
    Node[,] grid;

    public Node[,] GridArray
    {
        get { return grid; }
    }

    float nodeDiameter;
    int gridSizeX, gridSizeY; // gridSizeY representará el eje Z

    //Estas variables son para hacer dinamica los obstaculos de la grid
    [Header("Actualización Dinámica")]
    public bool updateGridDynamic = true;   // Activa/Desactiva la actualización
    public float updateInterval = 0.2f;     // Cada cuánto tiempo comprobamos (0.2s = 5 veces por seg)


    private void Awake()
    {
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        CreateGrid();

        StartCoroutine(UpdateGridRoutine());
    }



    public int MaxSize
    {
        get { return gridSizeX * gridSizeY; }
    }

    private void CreateGrid()
    {
        grid = new Node[gridSizeX, gridSizeY];


        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++) // 
            {

                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);


                bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask));

                grid[x, y] = new Node(walkable, worldPoint, x, y);
            }
        }
    }

    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++) // y es en realidad la z
            {
                if (x == 0 && y == 0)
                    continue;

                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                {
                    neighbours.Add(grid[checkX, checkY]);
                }
            }
        }
        return neighbours;
    }

    public Node NodeFromWorldPoint(Vector3 worldPosition)
    {
        float percentX = (worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x;

        float percentY = (worldPosition.z + gridWorldSize.y / 2) / gridWorldSize.y;

        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);
        return grid[x, y];
    }

    public void ResetGrid()
    {
        foreach (Lucidity.Node n in grid)
        {
            n.gCost = int.MaxValue;
            n.hCost = 0;
            n.parent = null;
            // Si quereis añadir una variable de visitado al nodo, resetéadla aquí si vosotros Carles y Joana xD
        }
    }


    IEnumerator UpdateGridRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(updateInterval);
            UpdateObstacles();
        }
    }

    // Esta funcion actualiza el estado a "walkable"
    public void UpdateObstacles()
    {
        if (grid == null) return;

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Node node = grid[x, y];


                // Si Physics.CheckSphere devuelve true, es que hay obstáculo
                bool isObstructed = Physics.CheckSphere(node.worldPosition, nodeRadius, unwalkableMask);

                // Actualizamos la propiedad del nodo existente
                node.walkable = !isObstructed;
            }
        }
    }


#if UNITY_EDITOR
    private void OnDrawGizmos()
    {

        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));

        if (grid != null && displayGridGizmos)
        {
            foreach (Lucidity.Node n in grid)
            {
                Gizmos.color = n.walkable ? Color.white : Color.red;

                Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - .1f));
            }
        }
    }
#endif
}
