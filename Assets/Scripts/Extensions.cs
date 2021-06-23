using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extensions
{
    public static T Last<T> (this List<T> list)
    {
        return list[list.Count - 1];
    }
}
