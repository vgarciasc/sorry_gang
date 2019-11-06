public struct Flag
{
    private readonly bool value;

    public Flag(bool value)
    {
        this.value = value;
    }

    public bool Value { get { return value; } }

    public static implicit operator Flag(bool b)
    {
        return new Flag(b);
    } 

    public static implicit operator bool(Flag p)
    {
        return p.Value;
    }
}