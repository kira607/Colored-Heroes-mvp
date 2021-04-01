using UnityEngine;

public class ChipMovementController : MonoBehaviour
{
    public static ChipMovementController Instance;
    
    private Chip _currentMovingChip;
    private Point _newIndex;
    private Vector2 _mouseStart;
    private Game _game;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        _game = GetComponent<Game>();
    }

    private void Update()
    {
        if (_currentMovingChip == null) return;
        
        var movementVector = (Vector2) Input.mousePosition - _mouseStart;
        var normalizedMovementVector = movementVector.normalized;
        var absoluteMovementVector = new Vector2(Mathf.Abs(movementVector.x), Mathf.Abs(movementVector.y));

        _newIndex = Point.GetClone(_currentMovingChip.index);
        var newIndexAdd = Point.Zero;
        
        // make newIndexAdd either (1, 0) | (-1, 0) | (0, 1) | (0, -1) depending on the direction of the mouse point
        if (movementVector.magnitude > ((float)Helpers.CellSize / 2))
        {
            if (absoluteMovementVector.x > absoluteMovementVector.y)
            {
                newIndexAdd = new Point((normalizedMovementVector.x > 0) ? 1 : -1, 0);
            }
            else if (absoluteMovementVector.x < absoluteMovementVector.y)
            {
                newIndexAdd = new Point(0, (normalizedMovementVector.y > 0) ? -1 : 1);
            }
        }
        _newIndex.Add(newIndexAdd);

        Vector2 newPosition = Helpers.instance.GetPositionFromPoint(_currentMovingChip.index);
        if (!_newIndex.Equals(_currentMovingChip.index))
        {
            newPosition += Point.GetMultiplication(
                    new Point(newIndexAdd.x, -newIndexAdd.y), Helpers.CellSize / 4).GetVector();
        }

        _currentMovingChip.MoveTo(newPosition);
        
    }

    public void MoveChip(Chip chip)
    {
        if (_currentMovingChip != null)
        {
            return;
        }

        _currentMovingChip = chip;
        _mouseStart = Input.mousePosition;
        // Debug.Log("Grab " + _currentMovingChip.transform.heroName);
    }

    public void DropChip()
    {
        if (_currentMovingChip == null) return;
        if (_currentMovingChip.index.Equals(_newIndex))
        {
            _game.ResetChip(_currentMovingChip);
        }
        else
        {
            _game.FlipChips(_currentMovingChip.index, _newIndex, true);
        }
        _currentMovingChip = null;
        //Debug.Log("Drop " + _currentMovingChip.transform.heroName);
        
    }
}
