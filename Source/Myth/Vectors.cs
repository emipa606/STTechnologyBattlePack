using System;
using UnityEngine;
using Verse;
using Random = UnityEngine.Random;

namespace Myth;

public class Vectors
{
    public static double EuclDist(IntVec3 a, IntVec3 b)
    {
        return Math.Sqrt(((a.x - b.x) * (a.x - b.x)) + ((a.y - b.y) * (a.y - b.y)) + ((a.z - b.z) * (a.z - b.z)));
    }

    public static double VectorSize(IntVec3 a)
    {
        return Math.Sqrt((a.x * a.x) + (a.y * a.y) + (a.z * a.z));
    }

    public static IntVec3 vecFromAngle(float angle1, float angle2, float r)
    {
        return new IntVec3((int)(r * Math.Sin(angle1) * Math.Cos(angle2)),
            (int)(r * Math.Sin(angle1) * Math.Sin(angle2)), (int)(r * Math.Cos(angle1)));
    }

    public static double vectorAngleA(IntVec3 a)
    {
        var num = VectorSize(a);
        return Math.Acos(a.z / num);
    }

    public static IntVec3 randomDirection(float r)
    {
        return vecFromAngle(Random.Range(0, 360), 0f, r);
    }

    public static Vector3 IntVecToVec(IntVec3 from)
    {
        return new Vector3(from.x, from.y, from.z);
    }

    public static IntVec3 VecToIntVec(Vector3 from)
    {
        return new IntVec3((int)from.x, (int)from.y, (int)from.z);
    }
}