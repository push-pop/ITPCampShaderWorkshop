using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LGF_ParticleRenderSimple : MonoBehaviour
{

    public struct MeshData {
        public Vector3 gNorm;
        public Vector3 vert;
        public Vector2 uv;
        public int index;
        public Vector3 normal;
        //public Matrix4x4 mat;
        public const int stride = (11) * sizeof(float) + sizeof(int);
    }



    public LGF_ParticleSolverSimple Solver;

    ComputeBuffer meshBuffer;

    //public int ParticleCount;
    public Mesh InputMesh;

    MeshFilter meshFilter;
    Material mat;
    private Mesh dummyMesh;

    Vector3[] gNorm;
    // Use this for initialization
    void Start()
    {
      
        meshFilter = GetComponent<MeshFilter>();
        mat = GetComponent<MeshRenderer>().sharedMaterial;

        dummyMesh = CreateDummyMesh();

        CreateMeshBuffer(dummyMesh);
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
        mat.SetInt("MeshIndexCount", 12);// InputMesh.triangles.Length);
    }

    private void CreateMeshBuffer(Mesh InputMesh)
    {
        var meshDatarr = new MeshData[InputMesh.triangles.Length];

        for (int i = 0; i < InputMesh.vertices.Length; i++)
            meshDatarr[i].vert = InputMesh.vertices[i];

        //for (int i = 0; i < InputMesh.triangles.Length; i++)
        //    meshDatarr[i].mat = Matrix4x4.identity;

        for (int i = 0; i < InputMesh.uv.Length; i++)
            meshDatarr[i].uv = InputMesh.uv[i];

        for (int i = 0; i < InputMesh.triangles.Length; i++)
            meshDatarr[i].index = InputMesh.triangles[i];

        for (int i = 0; i < InputMesh.normals.Length; i++)
            meshDatarr[i].normal = InputMesh.normals[i];

        int q = 0;
        for (int i = 0; i < InputMesh.normals.Length; i++) {
            
            meshDatarr[i].gNorm = InputMesh.normals[q];
            if (i > 0 && i % 12 == 0)
                q += 12;

        }

        meshBuffer = new ComputeBuffer(meshDatarr.Length, MeshData.stride);
        meshBuffer.SetData(meshDatarr);
    }

    Vector3 GetNormal(Vector3 a, Vector3 b, Vector3 c) {

        Vector3 A = b - a;
        Vector3 B = c - a;
        return Vector3.Cross(A, B);
    }

    private Mesh CreateDummyMesh()
    {
        int vCount = InputMesh.triangles.Length * 4;// * Solver.ParticleCount;

       
        var verts = new Vector3[vCount];
        var norm = new Vector3[vCount];
        var uvs = new Vector2[vCount];
        var ind = new int[vCount];
        var colors = new Color[vCount];
        gNorm = new Vector3[vCount];

        dummyMesh = new Mesh();

        int c = 0;
        Vector3 avg = Vector3.zero;

        for (int i = 0; i < colors.Length; i++) {
            colors[i] = Random.ColorHSV();
        }

        int q = 0;
        for (int i = 0; i < InputMesh.triangles.Length; i+=3)
        {
           
            if (i % 3 == 0) {
                avg = GetAverage(InputMesh.vertices[InputMesh.triangles[c]], InputMesh.vertices[InputMesh.triangles[c + 1]], InputMesh.vertices[InputMesh.triangles[c + 2]]);
                Vector3 N = GetNormal(InputMesh.vertices[InputMesh.triangles[c]], InputMesh.vertices[InputMesh.triangles[c + 1]], InputMesh.vertices[InputMesh.triangles[c + 2]]);

                Vector3 v = avg;// + InputMesh.normals[InputMesh.triangles[c]]*.001f;
                c += 3;


                verts[q] = InputMesh.vertices[InputMesh.triangles[i]] - avg;
                norm[q] = N;// InputMesh.normals[InputMesh.triangles[i]];
                uvs[q] = Vector2.zero;
                ind[q] = q;
                gNorm[q] = N;
                q += 1;

                verts[q] = InputMesh.vertices[InputMesh.triangles[i+1]] - avg;
                norm[q] = N;// InputMesh.normals[InputMesh.triangles[i+1]];
                uvs[q] = Vector2.zero;
                ind[q] = q;
                gNorm[q] = N;
                q += 1;

                verts[q] = InputMesh.vertices[InputMesh.triangles[i+2]] - avg;
                norm[q] = N;// InputMesh.normals[InputMesh.triangles[i+2]];
                uvs[q] = Vector2.zero;
                ind[q] = q;
                gNorm[q] = N;
                q += 1;

                int p = 0;

                for (int j = 0; j < 3; j++) {

                    verts[q] = InputMesh.vertices[InputMesh.triangles[i+p]] - avg;
                    norm[q] = InputMesh.normals[InputMesh.triangles[i]];
                    uvs[q] = Vector2.zero;
                    ind[q] = q;
                    gNorm[q] = N;
                    p += 1;
                    if (p > 2)
                        p = 0;
                    q += 1;
                    verts[q] = InputMesh.vertices[InputMesh.triangles[i + p]] - avg;
                    norm[q] = InputMesh.normals[InputMesh.triangles[i]];
                    uvs[q] = Vector2.zero;
                    ind[q] = q;
                    gNorm[q] = N;
                    p += 1;
                    if (p > 2)
                        p = 0;
                    q += 1;
                    verts[q] = InputMesh.normals[InputMesh.triangles[i]]*-.05f;
                    norm[q] = InputMesh.normals[InputMesh.triangles[i]];
                    uvs[q] = Vector2.zero;
                    ind[q] = q;
                    gNorm[q] = N;

                    //reverse
                    Vector3 temp = verts[q - 1];

                    verts[q - 1] = verts[q - 2];
                    verts[q - 2] = temp;

                    q += 1;


                   

                    N = GetNormal(verts[q - 1], verts[q - 2], verts[q - 3]);
                    norm[q - 1] = N;
                    norm[q - 2] = N;
                    norm[q - 3] = N;


                }
            }

        }

        //var topology = InputMesh.GetTopology(0);

        dummyMesh.name = "Dummy Mesh with " + vCount + " Verts";
        dummyMesh.vertices = verts;
        dummyMesh.normals = norm;
        dummyMesh.colors = colors;
        dummyMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        dummyMesh.SetIndices(ind, MeshTopology.Triangles, 0);
        dummyMesh.bounds = new Bounds(Vector3.zero, new Vector3(1000, 1000, 1000));

        meshFilter.mesh = dummyMesh;
        return dummyMesh;
    }

    Vector3 GetAverage(Vector3 a, Vector3 b, Vector3 c) {
        Vector3 average = Vector3.zero;
        for (int j = 0; j < 3; j++) {
            average = a;
            average += b;
            average += c;
        }
        average = new Vector3(average.x / 3, average.y / 3, average.z / 3);
        return average;
    }

     public ComputeBuffer CreateInitialPositions(ComputeBuffer particleBuffer) {
        int ParticleCount = InputMesh.triangles.Length / 3;
        Particle[] particleArray = new Particle[ParticleCount];

        int c = 0;
        for (int i = 0; i < ParticleCount; ++i) {
            particleArray[i].position = GetAverage(InputMesh.vertices[InputMesh.triangles[c]], InputMesh.vertices[InputMesh.triangles[c+1]], InputMesh.vertices[InputMesh.triangles[c+2]]);
            c += 3;
            particleArray[i].velocity.x = 0;
            particleArray[i].velocity.y = 0;
            particleArray[i].velocity.z = 0;
        }

        particleBuffer = new ComputeBuffer(ParticleCount, Particle.stride);
        particleBuffer.SetData(particleArray);
        return particleBuffer;
    }
}
