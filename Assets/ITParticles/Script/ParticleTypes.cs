using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct MeshData
{
    public Vector3 vert;
    public Vector2 uv;
    public int index;
    public Vector3 normal;

    public const int stride = 8 * sizeof(float) + sizeof(int);
}

public struct Particle
{
    public Vector3 emission;
    public Vector3 position;
    public Vector3 velocity;
    public float life;

    public const int stride = 10 * sizeof(float);
}