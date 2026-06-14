using System;
using UnityEngine;

[Serializable]
public class Array2D<T>
{
    [SerializeField] private int _width  = 2;
    [SerializeField] private int _height = 2;

    [SerializeField] private T[] _values = new T[4];

    public int Width
    {
        get => _width;
        set => Resize(value, _height);
    }

    public int Height
    {
        get => _height;
        set => Resize(_width, value);
    }

    public T[] Values
    {
        get => _values;
        set
        {
            T[] values = new T[_width * _height];

            int min = Mathf.Min(value.Length, values.Length);
            for (int i = 0; i < min; i++)
                values[i] = value[i];

             _values = values;
        }
    }

    public Array2D(int width = 2, int height = 2)
    {
        _width  = width;
        _height = height;
        _values = new T[_width * height];
    }

    public T this[int x, int y]
    {
        get => _values[x + (y * _width)];
        set => _values[x + (y * _width)] = value;
    }

    public (int x, int y) this[int index]
    {
        get
        {
            int x = index % _width;
            int y = index / _width;

            return (x, y);
        }
    }

    private void Resize(int width, int height)
    {
        int minWidth  = Mathf.Min(_width , Mathf.Max(width , 0));
        int minHeight = Mathf.Min(_height, Mathf.Max(height, 0));

        int oldWidth  = _width;
        int oldHeight = _height;

         _width  = width;
         _height = height;

        T[] values = new T[_width * _height];

        for (int x = 0; x < minWidth; x++)
            for (int y = 0; y < minHeight; y++)
                values[x + (y * _width)] = _values[x + (y * oldWidth)];

        _values = values;
    }

    public void Transpose()
    {
        Array2D<T> values = new Array2D<T>(Height, Width);
        
        for (int x = 0; x < Width; x++)
            for (int y = 0; y < Height; y++)
                values[y, x] = this[x, y];

        _values = values._values;
    }

    public void FlipHorizontally()
    {
        Array2D<T> values = new Array2D<T>(Width, Height);
        
        for (int x = 0; x < Width; x++)
            for (int y = 0; y < Height; y++)
                values[x, y] = this[(Width - (x + 1)), y];

        _values = values._values;
    }

    public void FlipVertically()
    {
        Array2D<T> values = new Array2D<T>(Width, Height);
        
        for (int x = 0; x < Width; x++)
            for (int y = 0; y < Height; y++)
                values[x, y] = this[x, (Height - (y + 1))];

        _values = values._values;
    }

    public void RotateClockwise()
    {
        Array2D<T> values = new Array2D<T>(Height, Width);
        
        for (int x = 0; x < Width; x++)
            for (int y = 0; y < Height; y++)
                values[y, x] = this[x, (Height - (y + 1))];

        _values = values._values;
    }

    public void RotateCounterClockwise()
    {
        Array2D<T> values = new Array2D<T>(Height, Width);
        
        for (int x = 0; x < Width; x++)
            for (int y = 0; y < Height; y++)
                values[y, x] = this[(Width - (x + 1)), y];

        _values = values._values;
    }
}