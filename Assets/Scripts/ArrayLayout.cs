using System;
using UnityEngine;

[Serializable]
public class ArrayLayout
{
    private int _width;
    private int _height;
    public Grid grid;
    public RowData[] rows = new RowData[7];

    [Serializable]
    public struct RowData
    {
        public bool[] row;
    }
}