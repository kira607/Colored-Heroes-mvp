using System;
using TMPro;
using UnityEngine;

namespace MatchBoard
{
    public class Chip : MonoBehaviour
    {
        public int row;
        public int column;

        public int targetX;
        public int targetY;
        
        private MatchBoard _board;
        private GameObject _otherChip;
        
        private Vector2 _touchPosition;
        private Vector2 _dropPosition;
        public float swipeAngle = 0;

        private Vector2 tempPosition;

        private void Start()
        {
            _board = FindObjectOfType<MatchBoard>();
            var position = transform.position;
            targetX = (int) position.x;
            targetY = (int) position.y;
            row = targetY;
            column = targetX;
            
        }

        private void Update()
        {
            targetX = column;
            targetY = row;
            if (Mathf.Abs(targetX - transform.position.x) > .1)
            {
                // Move to target
                var position = transform.position;
                tempPosition = new Vector2(targetX, position.y);
                transform.position = Vector2.Lerp(position, tempPosition, .4f);
            }
            else
            {
                // Set position directly
                var transform1 = transform;
                tempPosition = new Vector2(targetX, transform1.position.y);
                transform1.position = tempPosition;
                _board.chips[column, row] = gameObject;
            }
            if (Mathf.Abs(targetY - transform.position.y) > .1)
            {
                // Move to target
                var position = transform.position;
                tempPosition = new Vector2(position.x, targetY);
                transform.position = Vector2.Lerp(position, tempPosition, .4f);
            }
            else
            {
                // Set position directly
                var transform1 = transform;
                tempPosition = new Vector2(transform1.position.x, targetY);
                transform1.position = tempPosition;
                _board.chips[column, row] = gameObject;
            }
        }

        private void OnMouseDown()
        {
            if (Camera.main is { }) _touchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }

        private void OnMouseUp()
        {
            if (Camera.main is { }) _dropPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            CalculateAngle();
        }

        private void CalculateAngle()
        {
            swipeAngle = Mathf.Atan2(_dropPosition.y - _touchPosition.y, _dropPosition.x - _touchPosition.x) * 180 / Mathf.PI;
            Debug.Log(swipeAngle);
            Move();
        }

        void Move()
        {
            if (swipeAngle > -45 && swipeAngle <= 45 && column < _board.boardSettings.width)
            {
                // Right swipe
                _otherChip = _board.chips[column + 1, row];
                _otherChip.GetComponent<Chip>().column -= 1;
                column += 1;
            }
            else if (swipeAngle > 45 && swipeAngle <= 135 && row < _board.boardSettings.height)
            {
                // Up swipe
                _otherChip = _board.chips[column, row + 1];
                _otherChip.GetComponent<Chip>().row -= 1;
                row += 1;
            }
            else if ((swipeAngle > 135 || swipeAngle <= -135) && column > 0)
            {
                // Left swipe
                _otherChip = _board.chips[column - 1, row];
                _otherChip.GetComponent<Chip>().column += 1;
                column -= 1;
            }
            else if (swipeAngle > -135 && swipeAngle <= -45 && row > 0)
            {
                // Down swipe
                _otherChip = _board.chips[column, row - 1];
                _otherChip.GetComponent<Chip>().row += 1;
                row -= 1;
            }
        }
    }
}