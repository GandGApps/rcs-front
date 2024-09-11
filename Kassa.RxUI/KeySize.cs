namespace Kassa.RxUI;

public readonly struct KeySize(double size)
{
    /// <summary>
    /// default size is 1 (96px)
    /// Max size in line is 13
    /// </summary>
    public double Size
    {
        get;
    } = size;
}