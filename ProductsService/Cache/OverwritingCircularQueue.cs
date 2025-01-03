using System;
using System.Collections.Generic;

public class CacheItem<T>
{
    public T Item { get; set; }
    public DateTime CachedTime { get; set; } = DateTime.UtcNow;
}

public class OverwritingCircularQueue<T>
{
    private CacheItem<T>[] RawItems { get; set; }
    private readonly TimeSpan CacheTime;
    private int _head; // Points to the most recently added item
    private int _tail; // Points to the oldest item

    public OverwritingCircularQueue(int capacity, TimeSpan cacheTime)
    {
        if (capacity < 1)
        {
            throw new ArgumentException("Capacity must be greater than 0.", nameof(capacity));
        }

        if (cacheTime.TotalSeconds <= 0)
        {
            throw new ArgumentException("Cache timeout must be greater than 0 seconds.", nameof(cacheTime));
        }

        RawItems = new CacheItem<T>[capacity];
        CacheTime = cacheTime;
        _head = -1; // Indicates that no item has been added yet
        _tail = 0;  // Points to the first item to be overwritten
    }

    public void Enqueue(T item)
    {
        // Move the head pointer to the next slot
        _head = (_head + 1) % RawItems.Length;

        // Add the new cache item at the head position
        RawItems[_head] = new CacheItem<T> { Item = item, CachedTime = DateTime.UtcNow };

        // If the queue is full (head catches up with tail), move the tail to the next position
        if (_head == _tail)
        {
            _tail = (_tail + 1) % RawItems.Length;
        }
    }

    public IList<T> GetCachedItems()
    {
        var result = new List<T>();

        // Traverse from tail to head, collecting non-expired items
        int index = _tail;
        while (true)
        {
            var currentItem = RawItems[index];

            if (currentItem != null && (DateTime.UtcNow - currentItem.CachedTime) <= CacheTime)
            {
                result.Add(currentItem.Item);
            }

            if (index == _head)
            {
                break;
            }

            index = (index + 1) % RawItems.Length; // Move to the next item in circular fashion
        }

        return result;
    }

    public int Count
    {
        get
        {
            int count = 0;

            // Traverse from tail to head, counting non-expired items
            int index = _tail;
            while (true)
            {
                var currentItem = RawItems[index];

                if (currentItem != null && (DateTime.UtcNow - currentItem.CachedTime) <= CacheTime)
                {
                    count++;
                }

                if (index == _head)
                {
                    break;
                }

                index = (index + 1) % RawItems.Length; // Move to the next item in circular fashion
            }

            return count;
        }
    }
}