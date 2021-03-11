using System;
using UnityEngine;
internal class Utilities {
    internal static float GetExp(float u, float lambda) {
        return -Mathf.Log(1 - u) / lambda;
    }
}