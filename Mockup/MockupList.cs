namespace Data.Mockup;

using System.Reflection;

public class MockupList<T> : List<T> where T : class
{

    private int _identity = 0;
    private readonly PropertyInfo _idProperty;

    public MockupList(string idPropertyName)
    {
        var keyProp = typeof(T).GetProperty(idPropertyName);
        if (keyProp == null) throw new InvalidOperationException($"Identity property '{idPropertyName}'");
        _idProperty = keyProp;
    }

    /// <summary>
    /// Adds new item to mockup list.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public new T Add(T item)
    {
        _identity++;
        // set id
        _idProperty.SetValue(item, _identity);
        base.Add(item);
        return item;
    }

    /// <summary>
    /// Not implemented.
    /// </summary>
    /// <param name="items"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public new IEnumerable<T> AddRange(IEnumerable<T> items)
    {
        items.ToList().ForEach(i =>
        {
            _identity++;
            _idProperty.SetValue(i, _identity);
        });
        base.AddRange(items);

        return items;
    }
}