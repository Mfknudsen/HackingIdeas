using UnityEngine;

public static class Extensions
{
    public static int UniqueIndex(ref this int previous, int maxLength)
    {
        previous = (previous + Random.Range(1, maxLength - 1)) % maxLength;
        return previous;
    }
}