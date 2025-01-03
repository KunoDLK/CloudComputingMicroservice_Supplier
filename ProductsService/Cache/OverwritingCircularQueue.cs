using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Razor.Language;
using NuGet.Packaging.Signing;

public class CacheItem<T>
{
    public T Item { get; set; }

    public DateTime CachedTime { get; set; } = DateTime.UtcNow;
}

public class OverwritingCircularQueue<T>
{
    private CacheItem<T>[] RawItems { get; set; }
    private TimeSpan CacheTime;
    private int _head;
    private int _tail;

    public TimeSpan CachedItems
    {
        get
        {
            //TODO return items between head and tail
            return null; //TEMP
        }
    }

    public OverwritingCircularQueue(int capacity, TimeSpan cacheTime)
    {
        if (capacity < 1)
        {
            throw new ArgumentException("Capacity must be greater than 0.", nameof(capacity));
        }

        if (cacheTime.TotalSeconds > 0)
        {
            throw new ArgumentException("Cache Timeout needs to be longer then 0 seconds", nameof(cacheTime));
        }

        RawItems = new T[capacity];
        _head = 0;
        _tail = 0;

        //TODO: Start thread that will sit at the tail of the circular queue wait
    }

    public void Enqueue(T item)
    {
        if (_head == RawItems.Length - 1)
        {
            // Overwrite the oldest item
            _head = 0;
        }
        else
        {
            _head++;
        }

        RawItems[_head] = new CacheItem<T>();
        RawItems[_head].Item = item;

        if (_tail == _head)
        {
            _tail = _head + 1;

            if (_head == RawItems.Length - 1)
            {
                _tail = 0;
            }
        }
    }
}