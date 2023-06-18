using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShuffleBag<T> : IList<T>
{
    private readonly List<T> _data = new List<T>();
    
    private int _cursor = 0;
    private T _last;
    
    public ShuffleBag() { }

    public T Next() 
    {
        if( _cursor < 1 ) 
        {
            _cursor = _data.Count - 1;
            return _data.Count < 1 ? default : _data[0];
        }
        
        var grab = Mathf.FloorToInt(Random.value * (_cursor + 1));
        var temp = _data[grab];
        
        _data[grab] = _data[_cursor];
        _data[_cursor] = temp;
        _cursor--;
        
        return temp;
    }

    public ShuffleBag( IEnumerable<T> initialValues )
    {
        foreach (var value in initialValues)
        {
           Add(value); 
        }
    }

    public int IndexOf(T item) {return _data.IndexOf( item );}

    public void Insert( int index, T item ) 
    {
        _cursor = _data.Count;
        _data.Insert( index, item );
    }

    public void RemoveAt( int index ) 
    {
        _cursor = _data.Count - 2;
        _data.RemoveAt( index );
    }

    public T this[int index]
    {
        get => _data[index];
        set => _data[index] = value;
    }
    
    IEnumerator<T> IEnumerable<T>.GetEnumerator() { return _data.GetEnumerator();}
    public void Add( T item )
    {
        _data.Add( item );
        _cursor = _data.Count - 1;
    }

    public int Count => _data.Count;

    public void Clear() => _data.Clear();

    public bool Contains( T item ) {return _data.Contains( item );}
  

    public void CopyTo( T[] array, int arrayIndex)
    {
        foreach( var item in _data ) 
        {
            array.SetValue(item, arrayIndex);
            arrayIndex += 1;
        }
    }

    public bool Remove( T item ) 
    {
        _cursor = _data.Count - 2;
        
        return _data.Remove( item );
    }
    public bool IsReadOnly => false;
    
    IEnumerator IEnumerable.GetEnumerator(){return _data.GetEnumerator();}
}
