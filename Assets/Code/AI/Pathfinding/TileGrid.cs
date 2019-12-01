using System.Collections.Generic;

public class TileGrid {

    public Node[,] m_nodes;

    private int m_sizeX, m_sizeY;

    public TileGrid(int p_width, int p_height, float[,] p_tileCosts) {
        m_sizeX = p_width;
        m_sizeY = p_height;
        m_nodes = new Node[p_width, p_height];

        for(int x = 0; x < p_width; x++)
            for(int y = 0; y < p_height; y++)
                m_nodes[x, y] = new Node(p_tileCosts[x, y], x, y);
    }

    public TileGrid(int p_width, int p_height, bool[,] p_walkableTiles) {
        m_sizeX = p_width;
        m_sizeY = p_height;
        m_nodes = new Node[p_width, p_height];

        for(int x = 0; x < p_width; x++)
            for(int y = 0; y < p_height; y++)
                m_nodes[x, y] = new Node(p_walkableTiles[x, y] ? 1.0f : 0.0f, x, y);
    }

    public List<Node> GetNeighbours(Node node) {
        List<Node> neighbours = new List<Node>();

        for(int x = -1; x <= 1; x++) {
            for(int y = -1; y <= 1; y++) {
                if(x == 0 && y == 0) continue;

                int checkX = node.m_gridX + x;
                int checkY = node.m_gridY + y;

                if(checkX >= 0 && checkX < m_sizeX && checkY >= 0 && checkY < m_sizeY) {
                    if(x != 0 && y != 0) {
                        Node xNeighbour = m_nodes[checkX, node.m_gridY];
                        Node yNeighbour = m_nodes[node.m_gridX, checkY];

                        if(!xNeighbour.m_walkable || !yNeighbour.m_walkable) continue;
                    }

                    neighbours.Add(m_nodes[checkX, checkY]);
                }
            }
        }

        return neighbours;
    }
}
