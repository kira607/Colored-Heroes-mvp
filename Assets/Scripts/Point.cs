using System;
using UnityEngine;

[System.Serializable]
public class Point //: IComparable<Point>
{
    public int x;
    public int y;

    public string Str()
    {
        return "[" + x + ", " + y + "]";
    }
    public Point(int newX, int newY)
    {
        x = newX;
        y = newY;
    }

    public void Multiply(int multiplyer)
    {
        x *= multiplyer;
        y *= multiplyer;
    }

    public void Add(Point point)
    {
        x += point.x;
        y += point.y;
    }

    public Vector2 GetVector()
    {
        return new Vector2(x, y);
    }

    public bool Equals(Point p)
    {
        return (x == p.x && y == p.y);
    }

    public static Point GetFromVector(Vector2 v)
    {
        return new Point((int)v.x, (int)v.y);
    }

    public static Point GetFromVector(Vector3 v)
    {
        return new Point((int)v.x, (int)v.y);
    }

    public static Point GetMultiplication(Point point, int multiplyer)
    {
        return new Point(point.x * multiplyer, point.y * multiplyer);
    }

    public static Point GetAdd(Point point, Point other)
    {
        return new Point(point.x + other.x, point.y + other.y);
    }

    public static Point GetClone(Point point)
    {
        return new Point(point.x, point.y);
    }

    public static Point Zero
    {
        get { return new Point(0, 0); }
    }

    public static Point One
    {
        get { return new Point(1, 1); }
    }

    public static Point Up
    {
        get { return new Point(0, 1); }
    }

    public static Point Down
    {
        get { return new Point(0, -1); }
    }

    public static Point Left
    {
        get { return new Point(-1, 0); }
    }

    public static Point Right
    {
        get { return new Point(1, 0); }
    }

    // public int CompareTo(Point other)
    // {
    //     if (ReferenceEquals(this, other)) return 0;
    //     if (ReferenceEquals(null, other)) return 1;
    //     var xComparison = x.CompareTo(other.x);
    //     if (xComparison != 0) return xComparison;
    //     return y.CompareTo(other.y);
    // }
}
