using UnityEngine;

namespace MatchBoard
{
    public class MatchBoard : MonoBehaviour
    {
        public ArrayLayout boardLayout;

        [Header("Board common settings")] 
        public BoardSettings boardSettings;

        private Cell[,] _board;
        public GameObject[,] chips;

        private void Start()
        {
            InitializeFields();
            InstantiateBoard();
        }

        private void Update()
        {
        }

        private void InitializeFields()
        {
            _board = new Cell[boardSettings.width, boardSettings.height];
            chips = new GameObject[boardSettings.width, boardSettings.height];
        }

        private void InstantiateBoard()
        {
            for (var x = 0; x < boardSettings.width; ++x)
            for (var y = 0; y < boardSettings.height; ++y)
            {
                Vector2 newPosition = new Vector2(x, y);
                var newCell = Instantiate(boardSettings.cell, newPosition, Quaternion.identity);
                newCell.transform.parent = transform;
                newCell.name = "[" + x + "; " + y + "]";
                
                var newChipIndex = Random.Range(0, boardSettings.chips.Length);
                var newChip = Instantiate(boardSettings.chips[newChipIndex], newPosition, Quaternion.identity);
                //newChip.transform.parent = newCell.transform;
                newChip.name = "Chip " + gameObject.name;
            }
        }
    }
}