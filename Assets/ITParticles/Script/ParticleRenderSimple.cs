using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleRenderSimple : MonoBehaviour
{
    public ParticleSolverSimple Solver;

    ComputeBuffer meshBuffer;

    //public int ParticleCount;
    public Mesh PerParticleMesh;

    MeshFilter meshFilter;
    Material mat;
    private Mesh dummyMesh;

    // Use this for initialization
    void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        mat = GetComponent<MeshRenderer>().sharedMaterial;

        CreateDummyMesh();

        CreateMeshBuffer();
    }

    private void Update()
    {
        UpdateMaterialProperties();
    }

    private void OnDestroy()
    {
        if (meshBuffer != null)
            meshBuffer.Release();
    }

    private void UpdateMaterialProperties()
    {
        mat.SetBuffer("particleBuffer", Solver.particleBuffer);
        mat.SetBuffer("meshDataBuffer", meshBuffer);
        mat.SetInt("MeshIndexCount", PerParticleMesh.triangles.Length);
    }

    private void CreateMeshBuffer()
    {
        var meshDatarr = new MeshData[PerParticleMesh.triangles.Length];

        for (int i = 0; i < PerParticleMesh.vertices.Length; i++)
            meshDatarr[i].vert = PerParticleMesh.vertices[i];

        for (int i = 0; i < PerParticleMesh.uv.Length; i++)
            meshDatarr[i].uv = PerParticleMesh.uv[i];

        for (int i = 0; i < PerParticleMesh.triangles.Length; i++)
            meshDatarr[i].index = PerParticleMesh.triangles[i];

        for (int i = 0; i < PerParticleMesh.normals.Length; i++)
            meshDatarr[i].normal = PerParticleMesh.normals[i];

        meshBuffer = new ComputeBuffer(meshDatarr.Length, MeshData.stride);
        meshBuffer.SetData(meshDatarr);
    }

    private void CreateDummyMesh()
    {
        int vCount = PerParticleMesh.vertexCount * Solver.ParticleCount;

        var verts = new Vector3[vCount];
        var uvs = new Vector2[vCount];
        var ind = new int[vCount];

        dummyMesh = new Mesh();
        for (int i = 0; i < vCount; i++)
        {
            verts[i] = Vector3.zero;
            uvs[i] = Vector2.zero;
            ind[i] = i;
        }

        var topology = PerParticleMesh.GetTopology(0);

        dummyMesh.name = "Dummy Mesh with " + vCount + " Verts";
        dummyMesh.vertices = verts;
        dummyMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        dummyMesh.SetIndices(ind, MeshTopology.Triangles, 0);
        dummyMesh.bounds = new Bounds(Vector3.zero, new Vector3(1000, 1000, 1000));

        meshFilter.mesh = dummyMesh;
    }
}
