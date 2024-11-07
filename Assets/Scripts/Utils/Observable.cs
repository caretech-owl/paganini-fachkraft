using System;
using UnityEngine.Events;

public class Observable<T>
{
    // Wrapped data
    private T _data;

    // Event to notify listeners of data changes
    public UnityEvent OnDataChanged { get; private set; } = new UnityEvent();

    // Accessor for the wrapped data
    public T Data
    {
        get => _data;
        set
        {
            _data = value;
            //NotifyChanges();
        }
    }

    // Constructor to initialize with data
    public Observable(T initialData)
    {
        _data = initialData;
    }

    public Observable()
    {
        _data = default(T);
    }   

    public void Reset()
    {
        _data = default(T);
        NotifyChanges();
    } 

    // Method to trigger change notifications
    public void NotifyChanges()
    {
        OnDataChanged?.Invoke();
    }
}
