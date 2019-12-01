public class Node {

    public bool m_walkable;
    public int m_gridX, m_gridY;
    public float m_cost;
    public int m_gCost;
    public int m_hCost;
    public Node m_parent;

    // 0.0f cost = non-walkable
    public Node(float p_cost, int p_gridX, int p_gridY) {
        m_walkable = p_cost != 0.0f;
        m_cost = p_cost;
        m_gridX = p_gridX;
        m_gridY = p_gridY;
    }
    
    public int m_fCost {
        get {
            return m_gCost + m_hCost;
        }
    }
}
