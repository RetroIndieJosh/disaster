using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utility {
    public static void Shuffle<T>(this IList<T> list) {
        var n = list.Count;
        if (n == 0)
            return;
        while (n > 1) {
            n--;
            var k = Random.Range(0, n + 1);
            var value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}
