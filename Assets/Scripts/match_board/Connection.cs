using System.Collections.Generic;
using UnityEngine;

public class Connection
{
    public Point BasePoint { set; get; }
    public ChipColor BaseColor { set; get; }
    private readonly List<Point> _connection;
    private ConnectionType _type;
    private List<List<Point>> _verticalLines;
    private List<List<Point>> _horizontalLines;
    private bool _typeDetecting;
    
    public Connection()
    {
        _connection = new List<Point>();
        _type = ConnectionType.NoneOrUndetected;
        EnableTypeDetecting();
    }

    public void UnableTypeDetecting()
    {
        _type = ConnectionType.NoneOrUndetected;
        _typeDetecting = false;
    }
    
    public void EnableTypeDetecting()
    {
        _typeDetecting = true;
    }

    public List<Point> List()
    {
        return _connection;
    }

    public bool Empty()
    {
        return _connection.Count <= 0;
    }

    public void AddPoints(IEnumerable<Point> newPoints)
    {
        foreach (var point in newPoints)
        {
            AddPoint(point);
        }
    }

    public void AddPoint(Point point)
    {
        bool doAdd = true;
        foreach (var pointInConnection in _connection)
        {
            if (point.x >= 0 && point.x < 7 && point.y >= 0 &&
                point.y < 7)
            {
                if (pointInConnection.Equals(point))
                {
                    doAdd = false;
                    break;
                }
            }
        }
        
        if (!doAdd) return;
        
        _connection.Add(point);
        
        if (_typeDetecting)
        {
            DetectType();
        }
    }

    private void SortByX()
    {
        _connection.Sort
        ((point1, point2) => 
            { 
                var xComparison = point1.x.CompareTo(point2.x);
                return xComparison != 0 ? xComparison : point1.y.CompareTo(point2.y);
            }
        );
    }
    
    private void SortByY()
    {
        _connection.Sort
        ((point1, point2) => 
            { 
                var yComparison = point1.y.CompareTo(point2.y);
                return yComparison != 0 ? yComparison : point1.x.CompareTo(point2.x);
            }
        );
    }
    
    public void Clear()
    {
        _connection.Clear();
        _type = ConnectionType.NoneOrUndetected;
    }

    public ConnectionType Type()
    {
        return _type;
    }

    private void DetectType()
    {
        SortByX();
        SetVerticalLines();
        SetHorizontalLines();
        
        if (IsDiamond())
        {
            _type = ConnectionType.Dimond;
        }
        else if (IsBomb())
        {
            _type = ConnectionType.Bomb;
        }
        else if (IsLineLeftRight())
        {
            _type = ConnectionType.LineLeftRight;
        }
        else if (IsLineUpDown())
        {
            _type = ConnectionType.LineUpDown;
        }
        else if (IsUsual())
        {
            _type = ConnectionType.Usual;
        }
        else
        {
            _type = ConnectionType.NoneOrUndetected;
        }
    }
    
    private void SetHorizontalLines()
    {
        SortByY();
        var horizontalLines = new List<List<Point>>();
        var currentY = _connection[0].x;
        var currentLine = 0;
        horizontalLines.Add(new List<Point>());
        foreach (var point in _connection)
        {
            if (point.y == currentY)
            {
                horizontalLines[currentLine].Add(point);
            }
            else
            {
                ++currentLine;
                horizontalLines.Add(new List<Point>{point});
                currentY = point.y;
            }
        }

        _horizontalLines = horizontalLines;
    }

    private void SetVerticalLines()
    {
        SortByX();
        var verticalLines = new List<List<Point>>();
        var currentX = _connection[0].x;
        var currentLine = 0;
        verticalLines.Add(new List<Point>());
        foreach (var point in _connection)
        {
            if (point.x == currentX)
            {
                verticalLines[currentLine].Add(point);
            }
            else
            {
                ++currentLine;
                verticalLines.Add(new List<Point>{point});
                currentX = point.x;
            }
        }

        _verticalLines = verticalLines;
    }
    
    private bool IsUsual()
    {
        foreach (var line in _verticalLines)
        {
            if (line.Count == 3)
                return true;
        }

        foreach (var line in _horizontalLines)
        {
            if (line.Count == 3)
                return true;
        }
        return false;
    }

    private bool IsLineUpDown()
    {
        foreach (var line in _horizontalLines)
        {
            if (line.Count == 4)
                return true;
        }

        return false;
    }
    
    private bool IsLineLeftRight()
    {
        foreach (var line in _verticalLines)
        {
            if (line.Count == 4)
                return true;
        }
        return false;
    }
    
    private bool IsBomb()
    {
        foreach (var line in _verticalLines)
        {
            if (line.Count == 2)
                return true;
        }

        foreach (var line in _horizontalLines)
        {
            if (line.Count == 2)
                return true;
        }
        return false;
    }
    
    private bool IsDiamond()
    {
        foreach (var line in _verticalLines)
        {
            if (line.Count == 5)
                return true;
        }

        foreach (var line in _horizontalLines)
        {
            if (line.Count == 5)
                return true;
        }
        
        return false;
    }
    
    public static Connection operator +(Connection a, Connection b)
    {
        var c = new Connection();
        c.AddPoints(a.List());
        c.AddPoints(b.List());
        c.BasePoint = a.BasePoint;
        c.BaseColor = a.BaseColor;
        return c;
    }
    
    // method for debugging
    public string Str()
    {
        var result = "";
        foreach (var point in _connection)
        {
            result += " " + point.Str();
        }
        return result;
    }
    
    // method for debugging
    public void LogState()
    {
        var linesX = "";
        foreach (var line in _verticalLines)
        {
            foreach (var point in line)
            {
                linesX += point.Str() + " ";
            }
            linesX += "\n";
        }

        var linesY = "";
        foreach (var line in _horizontalLines)
        {
            foreach (var point in line)
            {
                linesY += point.Str() + " ";
            }
            linesY += "\n";
        }

        var logMessage = "Killed (" + _connection.Count + ") Type (" + _type + "): " + Str() + "\n";

        Debug.Log(logMessage);
        // Debug.Log("Sorted by x:\n" + linesX);
        // Debug.Log("Sorted by y:\n" + linesY);
    }

}