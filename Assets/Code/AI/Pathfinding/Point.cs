using UnityEngine;
using UnityEngine.Tilemaps;

public class Point {

    public int m_x, m_y;

    public Point() {
        m_x = 0;
        m_y = 0;
    }
    public Point(int p_x, int p_y) {
        m_x = p_x;
        m_y = p_y;
    }

    public Point(Point p_point) {
        m_x = p_point.m_x;
        m_y = p_point.m_y;
    }

    public override int GetHashCode() {
        return m_x ^ m_y;
    }

    public override bool Equals(System.Object obj) {
        Point p = (Point) obj;

        if(ReferenceEquals(null, p)) return false;

        return (m_x == p.m_x) && (m_y == p.m_y);
    }

    public bool Equals(Point p_point) {
        if(ReferenceEquals(null, p_point)) return false;

        return (m_x == p_point.m_x) && (m_y == p_point.m_y);
    }

    public static bool operator ==(Point p_a, Point p_b) {
        if(System.Object.ReferenceEquals(p_a, p_b)) return true;
        if(ReferenceEquals(null, p_a)) return false;
        if(ReferenceEquals(null, p_b)) return false;
        
        return p_a.m_x == p_b.m_x && p_a.m_y == p_b.m_y;
    }

    public static bool operator !=(Point p_a, Point p_b) {
        return !(p_a == p_b);
    }

    public Point Set(int p_x, int p_y) {
        m_x = p_x;
        m_y = p_y;

        return this;
    }

    public Vector3 ConvertToWorld(Tilemap p_tilemap) {
        Vector3 pos = p_tilemap.CellToWorld(new Vector3Int(m_x + p_tilemap.origin.x, 
                                                           m_y + p_tilemap.origin.y, 0));

        return new Vector3(pos.x + 0.5f, pos.y + 0.5f, pos.z); // center it
    }
}
