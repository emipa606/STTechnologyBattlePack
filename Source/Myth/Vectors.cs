using System;
using UnityEngine;
using Verse;

namespace Myth;

public class Vectors
{
    public static double EuclDist(IntVec3 a, IntVec3 b)
    {
        return Math.Sqrt(((a.x - b.x) * (a.x - b.x)) + ((a.y - b.y) * (a.y - b.y)) + ((a.z - b.z) * (a.z - b.z)));
    }

    public static Vector3 IntVecToVec(IntVec3 from)
    {
        return new Vector3(from.x, from.y, from.z);
    }
}