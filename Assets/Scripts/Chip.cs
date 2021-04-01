using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Chip : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public ChipColor color;
    public SuperColor superColor;
    public Point index;
    public ChipColor alternateColor;
    
    public Image image;
    private bool _updating;
    
    [HideInInspector] public Vector2 position;
    [HideInInspector] public RectTransform rectTransform;

    public void Initialize(ChipColor newColor, Point newIndex, Sprite sprite, SuperColor newSuperColor = SuperColor.None)
    {
        color = newColor;
        superColor = newSuperColor;
        index = newIndex;
        rectTransform = GetComponent<RectTransform>();
        image = GetComponent<Image>();
        image.sprite = sprite;
        // position = rectTransform.anchoredPosition;
    }

    public bool UpdateChipAndGetIfUpdated()
    {
        if (Vector3.Distance(rectTransform.anchoredPosition, position) > 1)
        {
            MoveTo(position);
            _updating = true;
            return true;
        }
        
        rectTransform.anchoredPosition = position;
        _updating = false;
        return false;
    }

    public void ResetPosition()
    {
        position = Helpers.instance.GetPositionFromPoint(index);
    }

    public void SetIndex(Point newIndex)
    {
        index = newIndex;
        ResetPosition();
        UpdateName();
    }

    public void MoveTo(Vector2 newPosition)
    {
        rectTransform.anchoredPosition = Vector2.Lerp(rectTransform.anchoredPosition, newPosition,
            Time.deltaTime * Helpers.CellSize / 8);
    }
    
    public void Move(Vector2 newPosition)
    {
        rectTransform.anchoredPosition += newPosition * Time.deltaTime * Helpers.CellSize / 4;
    }

    private void UpdateName()
    {
        transform.name = "Chip [" + index.x + ", " + index.y + "]";
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        if (_updating) return;
        ChipMovementController.Instance.MoveChip(this);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        ChipMovementController.Instance.DropChip();
    }
}