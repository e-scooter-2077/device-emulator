using System;

namespace DeviceEmulator.Extensions
{
    public static class ComparerExtensions
    {
        public static T Min<T>(T a, T b) where T : IComparable<T> => a.CompareTo(b) < 0 ? a : b;

        public static T Max<T>(T a, T b) where T : IComparable<T> => a.CompareTo(b) > 0 ? a : b;
    }
}
