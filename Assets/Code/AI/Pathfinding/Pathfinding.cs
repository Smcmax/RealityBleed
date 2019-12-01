using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Pathfinding {

    public static TileGrid GenerateGridFromTilemaps(Tilemap p_groundTilemap) {
        int sizeX = p_groundTilemap.size.x, sizeY = p_groundTilemap.size.y;
        Vector3Int groundOrigin = p_groundTilemap.origin;
        bool[,] tiles = new bool[sizeX, sizeY];

        for(int x = 0; x < sizeX; x++)
            for(int y = 0; y < sizeY; y++)
                tiles[x, y] = p_groundTilemap.HasTile(new Vector3Int(x + groundOrigin.x, y + groundOrigin.y, 0));

        return new TileGrid(sizeX, sizeY, tiles);
    }

    public static List<Point> FindPath(TileGrid p_grid, Tilemap p_tilemap, Entity p_entity, Entity p_target) {
        List<Point> points = new List<Point>();

        Vector3Int entityLoc = p_tilemap.WorldToCell(p_entity.transform.position);
        Vector3Int targetLoc = p_tilemap.WorldToCell(p_target.transform.position);
        Point start = new Point(entityLoc.x - p_tilemap.origin.x, entityLoc.y - p_tilemap.origin.y);
        Point target = new Point(targetLoc.x - p_tilemap.origin.x, targetLoc.y - p_tilemap.origin.y);

        points = FindPath(p_grid, start, target);

        return points;
    }

    public static List<Point> FindPath(TileGrid p_grid, Point p_start, Point p_target) {
        List<Node> path = GetPath(p_grid, p_start, p_target);
        List<Point> points = new List<Point>();

        if(path != null)
            foreach(Node node in path)
                points.Add(new Point(node.m_gridX, node.m_gridY));

        return points;
    }

    private static List<Node> GetPath(TileGrid p_grid, Point p_start, Point p_target) {
        Node startNode = p_grid.m_nodes[p_start.m_x, p_start.m_y];
        Node targetNode = p_grid.m_nodes[p_target.m_x, p_target.m_y];

        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();
        openSet.Add(startNode);

        while(openSet.Count > 0) {
            Node currentNode = openSet[0];

            for(int i = 1; i < openSet.Count; i++) 
                if(openSet[i].m_fCost < currentNode.m_fCost || 
                    openSet[i].m_fCost == currentNode.m_fCost && 
                    openSet[i].m_hCost < currentNode.m_hCost)
                    currentNode = openSet[i];

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if(currentNode == targetNode) return RetracePath(startNode, targetNode);

            foreach(Node neighbour in p_grid.GetNeighbours(currentNode)) {
                if(!neighbour.m_walkable || closedSet.Contains(neighbour)) continue;

                int newMovementCostToNeighbour = currentNode.m_gCost + GetDistance(currentNode, neighbour) * 
                                                 (int) (10.0f * neighbour.m_cost);
                if(newMovementCostToNeighbour < neighbour.m_gCost || !openSet.Contains(neighbour)) {
                    neighbour.m_gCost = newMovementCostToNeighbour;
                    neighbour.m_hCost = GetDistance(neighbour, targetNode);
                    neighbour.m_parent = currentNode;

                    if(!openSet.Contains(neighbour)) openSet.Add(neighbour);
                }
            }
        }

        return null;
    }

    private static List<Node> RetracePath(Node p_start, Node p_end) {
        List<Node> path = new List<Node>();
        Node currentNode = p_end;

        while(currentNode != p_start) {
            path.Add(currentNode);
            currentNode = currentNode.m_parent;
        }

        path.Reverse();
        return path;
    }

    private static int GetDistance(Node p_nodeA, Node p_nodeB) {
        int distX = Mathf.Abs(p_nodeA.m_gridX - p_nodeB.m_gridX);
        int distY = Mathf.Abs(p_nodeA.m_gridY - p_nodeB.m_gridY);

        if(distX > distY) return 14 * distY + 10 * (distX - distY);
        return 14 * distX + 10 * (distY - distX);
    }
}
