using System;
using System.Collections.Generic;

public class CircularQueue<T>
{
    private readonly T[] _items;
    private int _head;
    private int _tail;
    private int _count;

    public CircularQueue(int capacity)
    {
        if (capacity < 1)
        {
            throw new ArgumentException("Capacity must be greater than 0.", nameof(capacity));
        }

        _items = new T[capacity];
        _head = 0;
        _tail = -1;
        _count = 0;
    }

    public void Enqueue(T item)
    {
        if (_count == _items.Length)
        {
            // Overwrite the oldest item
            _head = (_head + 1) % _items.Length;
        }
        else
        {
            _count++;
        }

        _tail = (_tail + 1) % _items.Length;
        _items[_tail] = item;
    }

    public IEnumerable<T> GetItems()
    {
        for (int i = 0; i < _count; i++)
        {
            yield return _items[(_head + i) % _items.Length];
        }
    }

    public int Count => _count;

    public T PeekHead()
    {
        if (_count == 0)
        {
            throw new InvalidOperationException("The queue is empty.");
        }

        return _items[_head];
    }

    public T PeekTail()
    {
        if (_count == 0)
        {
            throw new InvalidOperationException("The queue is empty.");
        }

        return _items[_tail];
    }
}