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

    private void Resize(int width, int height)
    {
        _width  = Mathf.Max(width , 0);
        _height = Mathf.Max(height, 0);

        T[] values = new T[_width * _height];

            int min = Mathf.Min(_values.Length, values.Length);
            for (int i = 0; i < min; i++)
                values[i] = _values[i];

        _values = values;
    }

    public void Transpose()
    {
        Array2D<T> values = new Array2D<T>(Height, Width);
        
        for (int i = 0; i < Width; i++)
            for (int j = 0; j < Height; j++)
                values[j,i] = this[i,j];

        _values = values._values;
    }

    public void FlipHorizontally()
    {
        Array2D<T> values = new Array2D<T>(Width, Height);
        
        for (int i = 0; i < Width; i++)
            for (int j = 0; j < Height; j++)
                values[i,j] = this[(Width - (i + 1)),j];

        _values = values._values;
    }

    public void FlipVertically()
    {
        Array2D<T> values = new Array2D<T>(Width, Height);
        
        for (int i = 0; i < Width; i++)
            for (int j = 0; j < Height; j++)
                values[i,j] = this[i,(Height - (j + 1))];

        _values = values._values;
    }

    public void RotateClockwise()
    {
        Array2D<T> values = new Array2D<T>(Height, Width);
        
        for (int i = 0; i < Width; i++)
            for (int j = 0; j < Height; j++)
                values[j,i] = this[i,(Height - (j + 1))];

        _values = values._values;
    }

    public void RotateCounterClockwise()
    {
        Array2D<T> values = new Array2D<T>(Height, Width);
        
        for (int i = 0; i < Width; i++)
            for (int j = 0; j < Height; j++)
                values[j,i] = this[(Width - (i + 1)),j];

        _values = values._values;
    }
}