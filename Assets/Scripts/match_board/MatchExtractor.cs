using System;
using System.Collections.Generic;

public class CommonConnectionsExtractor
{
    private readonly Board _boardRef;
    private readonly Point[] _basicDirections;
    private Point _basePoint;
    private ChipColor _baseColor;
    private Connection _connection;

    public CommonConnectionsExtractor(ref Board board)
    {
        _boardRef = board;
        _basicDirections = Helpers.instance.BasicDirections();
    }

    public Connection Extract(Point point)
    {
        _basePoint = point;
        _baseColor = _boardRef.GetColorAtPoint(_basePoint);
        _connection = new Connection {BasePoint = _basePoint, BaseColor = _baseColor};
        _connection += FindLineConnections();
        _connection += FindMiddleConnections();
        _connection += Find2X2Connections();
        
        return _connection;
    }

    private Connection FindLineConnections()
    {
        var connection = new Connection();
        foreach (var direction in _basicDirections)
        {
            var sameColors = 0;
            var connectedPointsList = new List<Point>();
            connectedPointsList.Add(_basePoint);
            for (var i = 1; i < 3; ++i)
            {
                var checkPoint = Point.GetAdd(_basePoint, Point.GetMultiplication(direction, i));
                var checkColor = _boardRef.GetColorAtPoint(checkPoint);
                if (_baseColor.Equals(checkColor))
                {
                    ++sameColors;
                    connectedPointsList.Add(checkPoint);
                }
                if (sameColors < 2) continue;
                connection.AddPoints(connectedPointsList);
                // Debug.Log("CONNECTION LINE! Point: " + _basePoint.Str());
            }
        }

        return connection;
    }
    
    private Connection FindMiddleConnections()
    {
        var connection = new Connection();
        // checking if we are in the middle of two of the same shapes
        for (var i = 0; i < 2; ++i)
        {
            var sameColors = 0;
            var checkList = new List<Point>();
            checkList.Add(_basePoint);
            Point[] pointsToCheck =
            {
                Point.GetAdd(_basePoint, _basicDirections[i]), 
                Point.GetAdd(_basePoint, _basicDirections[i + 2])
            };
            // check both sides of the piece, of they are the same deleteColor, add them to the list
            foreach (Point checkPoint in pointsToCheck)
            {
                var checkColor = _boardRef.GetColorAtPoint(checkPoint);
                if (_baseColor.Equals(checkColor))
                {
                    ++sameColors;
                    checkList.Add(checkPoint);
                }
            }

            if (sameColors < 2) continue;
            connection.AddPoints(checkList);
            // Debug.Log("CONNECTION MIDDLE! Point: " + _basePoint.Str());
        }

        return connection;
    }

    private Connection Find2X2Connections()
    {
        var connection = new Connection();
        for(var i = 0; i < _basicDirections.Length; ++i)
        {
            Point[] pointsToCheck = new Point[3];
            var nextDirectionIndex = i + 1;
            if (nextDirectionIndex >= _basicDirections.Length)
            {
                nextDirectionIndex -= _basicDirections.Length;
            }
            pointsToCheck[0] = Point.GetAdd(_basePoint, _basicDirections[i]);
            pointsToCheck[1] = Point.GetAdd(_basePoint, _basicDirections[nextDirectionIndex]);
            pointsToCheck[2] = Point.GetAdd(_basePoint, 
                Point.GetAdd(_basicDirections[i], _basicDirections[nextDirectionIndex]));

            var checkList = new List<Point>();
            checkList.Add(_basePoint);
            var sameColors = 0;
            foreach (var checkPoint in pointsToCheck)
            {
                var checkColor = _boardRef.GetColorAtPoint(checkPoint);
                
                if (_baseColor.Equals(checkColor))
                {
                    ++sameColors;
                    checkList.Add(checkPoint);
                }
                
                if (sameColors < 3) continue;
                connection.AddPoints(checkList);
                // Debug.Log("CONNECTION 2X2! Point: " + _basePoint.Str());
            }
        }

        return connection;
    }

}

public class SuperConnectionsExtractor
{
    private readonly Board _boardRef;
    private readonly Point[] _basicDirections;
    private Connection _connection;
    
    public SuperConnectionsExtractor(ref Board board)
    {
        _boardRef = board;
        _basicDirections = Helpers.instance.BasicDirections();
    }

    public Connection Extract(Point basePoint, Point flippedPoint = null)
    {
        _connection = new Connection {BasePoint = basePoint, BaseColor = _boardRef.GetColorAtPoint(flippedPoint)};

        if (!_boardRef.GetColorAtPoint(basePoint).Equals(ChipColor.Multicolor) || flippedPoint is null)
            return _connection;

        _connection.UnableTypeDetecting();
        _connection += DetectSuperAndExtract(basePoint, flippedPoint);
        _connection += ExtractSubConnections();
        
        return _connection;
    }

    private Connection ExtractSubConnections()
    {
        var subConnections = new List<Connection>();
        var resultingSubConnections = new List<Connection>();
        var checkedPoints = new List<Point> {_connection.BasePoint};
        subConnections.Add(_connection);

        while (subConnections.Count > 0)
        {
            resultingSubConnections.AddRange(subConnections);
            
            var pointsToExtract = new List<Point>();
            
            // get list of multicolor points from each subConnections
            foreach (var subConnection in subConnections)
            {
                var multicolorPoints = GetMulticolorPoints(subConnection);
                
                // include each multicolor point in pointsToExtract, only if it is not in checkedPoints list
                foreach (var multicolorPoint in multicolorPoints)
                {
                    bool doAdd = true;
                    foreach (var checkedPoint in checkedPoints)
                    {
                        if (!multicolorPoint.Equals(checkedPoint)) continue;
                        doAdd = false;
                        break;
                    }

                    if (doAdd)
                    {
                        pointsToExtract.Add(multicolorPoint);
                    }
                }
            }
            // clear subConnections
            subConnections.Clear();
            // for each point in pointsToExtract extract connection
            foreach (var point in pointsToExtract)
            {
                // add extracted connection in subConnections
                subConnections.Add(DetectSuperAndExtract(point, null));
                // add point in checkedPoints
                checkedPoints.Add(point);
            }
        }
        
        Connection resultingConnection = new Connection(); 

        foreach (var connection in resultingSubConnections)
        {
            resultingConnection += connection;
        }

        return resultingConnection;
    }

    private List<Point> GetMulticolorPoints(Connection connection)
    {
        var points = new List<Point>();
        foreach (var point in connection.List())
        {
            if (_boardRef.GetColorAtPoint(point).Equals(ChipColor.Multicolor))
            {
                points.Add(point);
            }
        }

        return points;
    }

    private Connection DetectSuperAndExtract(Point basePoint, Point flippedPoint)
    {
        Connection connection = null;
        switch (_boardRef[basePoint].GetChip().superColor)
        {
            case SuperColor.LineUpDown:
                connection = ExtractForLineUpDown(basePoint);
                break;
            case SuperColor.LineLeftRight:
                connection = ExtractForLineLeftRight(basePoint);
                break;
            case SuperColor.Bomb:
                connection = ExtractForBomb(basePoint);
                break;
            case SuperColor.Dimond:
                connection = ExtractForDimond(basePoint, flippedPoint is null ? Helpers.instance.GetRandomColor() : 
                                                                                _boardRef.GetColorAtPoint(flippedPoint));
                break;
            case SuperColor.None:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        return connection;
    }

    private Connection ExtractForDimond(Point basePoint, ChipColor deleteColor)
    {
        var connection = new Connection();
        connection.AddPoint(basePoint);
        for (var x = 0; x < _boardRef.Width; ++x)
        {
            for (var y = 0; y < _boardRef.Height; ++y)
            {
                var currentPoint = new Point(x, y);
                if (_boardRef[currentPoint].color.Equals(deleteColor))
                {
                    connection.AddPoint(currentPoint);
                }
            }
        }

        return connection;
    }
    
    private Connection ExtractForBomb(Point basePoint)
    {
        var connection = new Connection();
        connection.AddPoint(basePoint);
        for(var i = 0; i < _basicDirections.Length; ++i)
        {
            var nextDirectionIndex = i + 1;
            if (nextDirectionIndex >= _basicDirections.Length)
            {
                nextDirectionIndex -= _basicDirections.Length;
            }
            connection.AddPoint(Point.GetAdd(basePoint, _basicDirections[i]));
            connection.AddPoint(Point.GetAdd(basePoint, 
                Point.GetAdd(_basicDirections[i], _basicDirections[nextDirectionIndex])));
        }
        return connection;
    }
    
    private Connection ExtractForLineUpDown(Point basePoint)
    {
        var connection = new Connection();

        for(var y = 0; y < _boardRef.Height; ++y)
        {
            var pointToAdd = new Point(basePoint.x, y);
            connection.AddPoint(pointToAdd);
        }
        
        return connection;
    }
    
    private Connection ExtractForLineLeftRight(Point basePoint)
    {
        var connection = new Connection();

        for(var x = 0; x < _boardRef.Width; ++x)
        {
            var pointToAdd = new Point(x, basePoint.y);
            connection.AddPoint(pointToAdd);
        }
        
        return connection;
    }
}

public class MatchExtractor
{
    private readonly CommonConnectionsExtractor _commonConnectionsExtractor;
    private readonly SuperConnectionsExtractor _superConnectionsExtractor;
    private readonly Board _boardRef;
    
    public MatchExtractor(ref Board board)
    {
        _commonConnectionsExtractor = new CommonConnectionsExtractor(ref board);
        _superConnectionsExtractor = new SuperConnectionsExtractor(ref board);
        _boardRef = board;
    }

    public Connection Extract(Point basePoint, Point flippedPoint = null)
    {
        var connection = _boardRef.GetColorAtPoint(basePoint) == ChipColor.Multicolor ? 
                         _superConnectionsExtractor.Extract(basePoint, flippedPoint) : 
                         _commonConnectionsExtractor.Extract(basePoint);
        return connection;
    }
}