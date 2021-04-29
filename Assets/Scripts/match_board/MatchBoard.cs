using System;
using System.Collections.Generic;
using UnityEngine;

namespace match_board
{
    public class MatchBoard : MonoBehaviour
    {
        public ArrayLayout boardLayout;

        [Header("UI Elements")]
        public Sprite[] chipsSprites;
        public Sprite[] superChipsSprites;
    
        [Header("Game field")]
        public RectTransform matchBoard;

        [Header("Prefabs")]
        public GameObject chipPrefab;

        private const int Width = 7;
        private const int Height = 7;
        
        private Board _board;
        private MatchExtractor _matchExtractor;
        private Helpers _helpers;

        private Dictionary<ChipColor, Sprite> _colorSprites;
        private Dictionary<SuperColor, Sprite> _superSprites;

        private List<Chip> _listOfChipsToUpdate;
        private List<Chip> _finishedUpdatingList;
        private List<Chip> _deadChips;
        private List<FlippedChips> _flippedChipsList;

        private int[] _fills;

        private void Start()
        {
            InitializeFields();
            InitializeBoard();
            VerifyBoard();
            InstantiateBoard();
        }

        private void Update()
        {
            _finishedUpdatingList = new List<Chip>();
            foreach (var chip in _listOfChipsToUpdate)
            {
                var updating = chip.UpdateChipAndGetIfUpdated();
                if (!updating)
                {
                    _finishedUpdatingList.Add(chip);
                }
            }
            foreach (var updatedChip in _finishedUpdatingList)
            {
                FlippedChips flip = GetFlipForChip(updatedChip);
                bool wasFlipped = (flip != null);
            
                var x = updatedChip.index.x;
                _fills[x] = Mathf.Clamp(_fills[x] - 1, 0, Height);

                var connectionOne = new Connection();
                var connectionTwo = new Connection();
                Chip flippedChipForCurrent = null;

            
                if (wasFlipped)
                {
                    flippedChipForCurrent = flip.GetOtherChip(updatedChip);
                    connectionOne = _matchExtractor.Extract(updatedChip.index, flippedChipForCurrent.index);
                    connectionTwo = _matchExtractor.Extract(flippedChipForCurrent.index, updatedChip.index);
                }
                else
                {
                    if (updatedChip.color != ChipColor.Multicolor)
                    {
                        connectionOne = _matchExtractor.Extract(updatedChip.index);
                    }
                }

                // didn't make connections
                if (connectionOne.Empty() && connectionTwo.Empty())
                {
                    if (wasFlipped)
                    {
                        FlipChips(updatedChip.index, flippedChipForCurrent.index, false);
                    }
                }
                //made at least one connection
                else
                {
                    KillConnection(connectionOne);
                    KillConnection(connectionTwo);
                    ApplyGravity();
                }

                _flippedChipsList.Remove(flip);
                _listOfChipsToUpdate.Remove(updatedChip);
            }
        }

        private void InitializeFields()
        {
            _helpers = Helpers.instance;
            _board = new Board(Width, Height);
            _matchExtractor = new MatchExtractor(ref _board);

            _listOfChipsToUpdate = new List<Chip>();
            _finishedUpdatingList = new List<Chip>();
            _deadChips = new List<Chip>();
            _flippedChipsList = new List<FlippedChips>();

            _fills = new int[Width];

            _colorSprites = new Dictionary<ChipColor, Sprite>
            {
                {ChipColor.Orange, chipsSprites[0]},
                {ChipColor.Red, chipsSprites[1]},
                {ChipColor.Green, chipsSprites[2]},
                {ChipColor.Blue, chipsSprites[3]},
                {ChipColor.Purple, chipsSprites[4]}
                // ToDo: Add Unplayable Cell Sprite
                // {ChipColor.Hole, chipsSprites[5]};
            };
            _superSprites = new Dictionary<SuperColor, Sprite>
            {
                {SuperColor.LineUpDown, superChipsSprites[0]},
                {SuperColor.LineLeftRight, superChipsSprites[1]},
                {SuperColor.Bomb, superChipsSprites[2]},
                {SuperColor.Diamond, superChipsSprites[3]}
            };
        }

        private void InitializeBoard()
        {
            for(int y = 0; y < Height; ++y)
            {
                for(int x = 0; x < Width; ++x)
                {
                    var newColor = ChipColor.Hole;
                    if (!boardLayout.rows[y].row[x])
                    {
                        newColor = Helpers.instance.GetRandomColor();
                    }
                    _board[x, y] = new Cell(newColor, new Point(x, y));
                }
            }
        }

        private void VerifyBoard()
        {
            for(int y = 0; y < Height; ++y)
            {
                for(int x = 0; x < Width; ++x)
                {
                    var currentPoint = new Point(x, y);
                    var badColorsToRemove = new List<ChipColor>();
                    if (!_board[currentPoint].IsPlayable()) continue;
                    while (!_matchExtractor.Extract(new Point(x, y)).Empty())
                    {
                        badColorsToRemove.Add(_board.GetColorAtPoint(currentPoint));
                        _board.SetColorAtPoint(currentPoint, Helpers.instance.GetAvailableColor(ref badColorsToRemove));
                    }
                }
            }
        }

        private void InstantiateBoard()
        {
            for(int y = 0; y < Height; ++y)
            {
                for(int x = 0; x < Width; ++x)
                {
                    InstantiateChipAtPoint(new Point(x, y));
                }
            }
        }
    
        private void InstantiateChipAtPoint(Point point)
        {
            var cell = _board[point];

            if (!cell.IsPlayable())
            {
                return;
            }
        
            GameObject newGameObject = Instantiate(chipPrefab, matchBoard);
            Chip chip = newGameObject.GetComponent<Chip>();
            RectTransform rect = newGameObject.GetComponent<RectTransform>();

            rect.anchoredPosition = Helpers.instance.GetPositionFromPoint(point);
        
            var color = cell.color;
            var index = cell.index;
            var sprite = _colorSprites[color];
        
            chip.Initialize(color, index, sprite);
            cell.SetChip(chip);
        }
    
        public void FlipChips(Point one, Point two, bool main)
        {
            var cellOne = _board[one];
            var chipOne = cellOne.GetChip();

            if (chipOne.color == ChipColor.Hole || chipOne.color == ChipColor.Blank)
            {
                return;
            }
        
            if (!_helpers.CommonColors().Contains(_board.GetColorAtPoint(two)))
            {
                ResetChip(chipOne);
                return;
            }
        
            var cellTwo = _board[two.x, two.y];
            var chipTwo = cellTwo.GetChip();

            cellOne.SetChip(chipTwo);
            cellTwo.SetChip(chipOne);
        
            if (main)
            {
                _flippedChipsList.Add(new FlippedChips(chipOne, chipTwo));
            }
        
            _listOfChipsToUpdate.Add(chipOne);
            _listOfChipsToUpdate.Add(chipTwo);
        }
    
        public void ResetChip(Chip chip)
        {
            chip.ResetPosition();
            _listOfChipsToUpdate.Add(chip);
        }

        private FlippedChips GetFlipForChip(Chip chip)
        {
            FlippedChips flip = null;
            foreach (var flippedChips in _flippedChipsList)
            {
                if (flippedChips.GetOtherChip(chip) != null)
                {
                    flip = flippedChips;
                    break;
                }
            }
        
            return flip;
        }

        private void KillConnection(Connection connection)
        {
            if (connection == null) return;
            if (connection.Empty()) return;
            var connectionContainedMulticolor = false;
        
            // Logging
            connection.List().ForEach(point =>
            {
                var color = _board.GetColorAtPoint(point);
                if (color != ChipColor.Multicolor) return;
                // Debug.Log("Contained super chip: " + _board[point].GetChip().superColor);
                connectionContainedMulticolor = true;
            });
            // connection.LogState();

            // kill all chips in connection
            foreach (var point in connection.List())
            {
                var currentCell = _board[point];
                if (currentCell is null) continue;
                var currentChip = currentCell.GetChip();
                if (!(currentChip is null))
                {
                    if (currentChip.color == ChipColor.Multicolor)
                    {
                        currentChip.alternateColor = connection.BaseColor;
                    }
                    currentChip.gameObject.SetActive(false);
                    _deadChips.Add(currentChip);
                }

                currentCell.SetChip(null);
            }

            var connectionType = connection.Type();
            if (connectionType == ConnectionType.Usual || connectionType == ConnectionType.NoneOrUndetected)
            {
                return;
            }

            if (connectionContainedMulticolor)
            {
                return;
            }
        
            ReviveSuperChip(connection.BasePoint, connection.Type());
        }

        private void ApplyGravity()
        {
            for (int x = 0; x < Width; ++x)
            {
                for (int y = (Height - 1); y >= 0; --y)
                {
                    var bottomPoint = new Point(x, y);
                    var bottomCell = _board[bottomPoint];
                    var bottomColor = _board.GetColorAtPoint(bottomPoint);
                
                    if (bottomColor != ChipColor.Blank) continue;
                    for (int ny = (y - 1); ny >= -1; --ny)
                    {
                        var topPoint = new Point(x, ny);
                        var topColor = _board.GetColorAtPoint(topPoint);

                        if (topColor == ChipColor.Blank) continue;
                        if (topColor != ChipColor.Hole)
                        {
                            Cell topCell = _board[topPoint];
                            Chip topChip = topCell.GetChip();

                            // move chip to bottom cell
                            bottomCell.SetChip(topChip);
                            _listOfChipsToUpdate.Add(topChip);
                            topCell.SetChip(null);
                        }
                        else // Hit an end (hit a hole actually)
                        {
                            ReviveChip(x, bottomPoint);
                        }

                        break;
                    }
                }
            }
        }

        private void ReviveChip(int column, Point point, Point pointOfRevive = null)
        {
            var fallPoint = new Point(column, (-1 - _fills[column]));
            var newColor = _helpers.GetRandomColor();
            Chip newChip;
            if (_deadChips.Count > 0)
            {
                var revivedChip = _deadChips[0];
                revivedChip.gameObject.SetActive(true);
                revivedChip.rectTransform.anchoredPosition = _helpers.GetPositionFromPoint(fallPoint);
                revivedChip.Initialize(newColor, point, _colorSprites[newColor]);
                newChip = revivedChip;
                            
                _deadChips.RemoveAt(0);
            }
            else
            {
                GameObject newGameObject = Instantiate(chipPrefab, matchBoard);
                Chip chip = newGameObject.GetComponent<Chip>();
                RectTransform rect = newGameObject.GetComponent<RectTransform>();
                rect.anchoredPosition = _helpers.GetPositionFromPoint(fallPoint);
                newChip = chip;
            }
            newChip.Initialize(newColor, point, _colorSprites[newColor]);
                        
            var emptyCell = _board[point];
            emptyCell.SetChip(newChip);
            ResetChip(newChip);
            ++_fills[column];
        }

        private void ReviveSuperChip(Point revivePoint, ConnectionType connectionType)
        {
            Chip newSuperChip = _deadChips[0];

            newSuperChip.gameObject.SetActive(true);
            newSuperChip.rectTransform.anchoredPosition = _helpers.GetPositionFromPoint(revivePoint);
            var color = ChipColor.Multicolor;

            SuperColor superColor;
        
            switch (connectionType)
            {
                case ConnectionType.LineUpDown:
                    superColor = SuperColor.LineUpDown;
                    break;
                case ConnectionType.LineLeftRight:
                    superColor = SuperColor.LineLeftRight;
                    break;
                case ConnectionType.Bomb:
                    superColor = SuperColor.Bomb;
                    break;
                case ConnectionType.Dimond:
                    superColor = SuperColor.Diamond;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        
            newSuperChip.Initialize(color, revivePoint, _superSprites[superColor], superColor);
            var emptyCell = _board[revivePoint];
            emptyCell.SetChip(newSuperChip);
            ResetChip(newSuperChip);
        
            _deadChips.RemoveAt(0);
        }
    }
}
