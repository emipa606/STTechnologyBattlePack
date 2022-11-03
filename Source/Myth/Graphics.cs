using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace Myth;

public static class Graphics
{
    private static Mesh CircleMesh_cache;

    public static Mesh CircleMesh
    {
        get
        {
            if (CircleMesh_cache != null)
            {
                return CircleMesh_cache;
            }

            var num = 1.0;
            var list = new List<Vector2> { new Vector2(0f, 0f) };
            for (var i = 0; i <= 360; i += 4)
            {
                var num2 = i / 180f * Math.PI;
                list.Add(new Vector2(0f, 0f)
                {
                    x = (float)(num * Math.Cos(num2)),
                    y = (float)(num * Math.Sin(num2))
                });
            }

            var array = new Vector3[list.Count];
            for (var j = 0; j < array.Length; j++)
            {
                array[j] = new Vector3(list[j].x, 0f, list[j].y);
            }

            var triangles = new Triangulator(list.ToArray()).Triangulate();
            CircleMesh_cache = new Mesh
            {
                vertices = array,
                uv = new Vector2[list.Count],
                triangles = triangles
            };
            CircleMesh_cache.RecalculateNormals();
            CircleMesh_cache.RecalculateBounds();
            return CircleMesh_cache;
        }
    }
}