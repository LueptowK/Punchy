using System;

public class ContextItem<T>
{
    private T value;

    public ContextItem(T input)
    {
        value = input;
    }

    public void Set(T input)
    {
        value = input;
    }

    public T Get()
    {
        return value;
    }
}