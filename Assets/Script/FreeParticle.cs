using UnityEngine;
using System.Collections;

public class FreeParticle : MonoBehaviour
{
    /// <summary>
    /// Particle data structure used by the shader and the compute shader.
    /// </summary>
    private struct Particle
    {
        public Vector3 position;
        public Vector3 velocity;
    }

    /// <summary>
    /// Size in octet of the Particle struct.
    /// </summary>
    private const int SIZE_PARTICLE = 24;

    /// <summary>
    /// Number of Particle created in the system.
    /// </summary>
    public int particleCount = 1000;

    /// <summary>
    /// Material used to draw the Particle on screen.
    /// </summary>
    public Material material;

    /// <summary>
    /// Compute shader used to update the Particles.
    /// </summary>
    public ComputeShader computeShader;

    /// <summary>
    /// Id of the kernel used.
    /// </summary>
    private int updateKernelID;

    /// <summary>
    /// Buffer holding the Particles.
    /// </summary>
    ComputeBuffer particleBuffer;

    ComputeBuffer argsBuffer;

    /// <summary>
    /// Number of particle per warp.
    /// </summary>
    private const int WARP_SIZE = 256;

    /// <summary>
    /// Number of warp needed.
    /// </summary>
    private int mWarpCount;

    public Mesh mesh;

    void Start()
    {
        // Calculate the number of warp needed to handle all the particles
        if (particleCount <= 0)
            particleCount = 1;
        mWarpCount = Mathf.CeilToInt((float)particleCount / WARP_SIZE);

        // Initialize the Particle at the start
        Particle[] particleArray = new Particle[particleCount];
        for (int i = 0; i < particleCount; ++i)
        {
            particleArray[i].position = 5 * Random.insideUnitSphere;
            //particleArray[i].position.x = Random.value * 2 - 1.0f;
            //particleArray[i].position.y = Random.value * 2 - 1.0f;
            //particleArray[i].position.z = 0;

            particleArray[i].velocity.x = 0;
            particleArray[i].velocity.y = 0;
            particleArray[i].velocity.z = 0;
        }

        // Create the ComputeBuffer holding the Particles
        particleBuffer = new ComputeBuffer(particleCount, SIZE_PARTICLE);
        particleBuffer.SetData(particleArray);

        // Find the id of the kernel
        updateKernelID = computeShader.FindKernel("CSMain");

        // Bind the ComputeBuffer to the shader and the compute shader
        computeShader.SetBuffer(updateKernelID, "particleBuffer", particleBuffer);
        material.SetBuffer("particleBuffer", particleBuffer);
    }

    void OnDestroy()
    {
        if (particleBuffer != null)
            particleBuffer.Release();
    }

    // Update is called once per frame
    void Update()
    {
        // Send datas to the compute shader
        computeShader.SetFloat("deltaTime", Time.deltaTime);

        // Update the Particles
        computeShader.Dispatch(updateKernelID, mWarpCount, 1, 1);
    }

    void OnRenderObject()
    {
        material.SetPass(0);
        Graphics.DrawProcedural(MeshTopology.Points, 1, particleCount);
        //Graphics.DrawMeshInstanced(mesh, 0, material, )
        var arr = new int[4];
        arr[0] = particleCount;

        argsBuffer = new ComputeBuffer(1, 4);
        argsBuffer.SetData(arr);

        //Graphics.DrawMeshInstancedIndirect(mesh, 0, material, new Bounds(new Vector3(0, 0, 0), new Vector3(1000, 1000, 1000)), argsBuffer);        //Graphics.DrawMeshInstancedIndirect(mesh, 0, material, new Bounds(new Vector3(0, 0, 0), new Vector3(1000, 1000, 1000)), argsBuffer);
    }


}
