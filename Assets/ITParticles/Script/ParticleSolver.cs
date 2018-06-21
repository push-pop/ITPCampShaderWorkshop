using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ParticleSolver : MonoBehaviour
{

    public int ParticleCount = 5000;

    public ComputeShader Kernels;
    public ComputeBuffer particleBuffer;
    public ComputeBuffer forceBuffer;

    public Vector3 ConstantVelocity = Vector3.zero;
    public float NoiseAmplitude = 0f;
    public ParticleForce[] Forces;


    int UpdateParticleIndex;

    // Use this for initialization
    void Start()
    {
        UpdateParticleIndex = Kernels.FindKernel("UpdateParticles");

        CreateInitialPositions();
    }

    // Update is called once per frame
    void Update()
    {
        Kernels.SetBuffer(UpdateParticleIndex, "particleBuffer", particleBuffer);
        Kernels.SetFloat("deltaTime", Time.deltaTime);
        Kernels.SetVector("constantVelocity", ConstantVelocity);
        CollectForces();

        Kernels.Dispatch(UpdateParticleIndex, ParticleCount, 1, 1);
    }

    private void OnDestroy()
    {
        if (particleBuffer != null)
            particleBuffer.Release();
        if (forceBuffer != null)
            forceBuffer.Release();
    }

    private void CollectForces()
    {
        if (forceBuffer == null)
            forceBuffer = new ComputeBuffer(Forces.Length, ParticleForce.ForceInfo.stride);

        forceBuffer.SetData(Forces.Select(p => p.Info).ToArray());

        Kernels.SetInt("forceCount", forceBuffer.count);
        Kernels.SetBuffer(UpdateParticleIndex, "forceBuffer", forceBuffer);
    }

    private void CreateInitialPositions()
    {
        Particle[] particleArray = new Particle[ParticleCount];
        for (int i = 0; i < ParticleCount; ++i)
        {
            particleArray[i].position = 2 * Random.onUnitSphere;
            //particleArray[i].emission = particleArray[i].position;

            particleArray[i].velocity.x = 0;
            particleArray[i].velocity.y = 0;
            particleArray[i].velocity.z = 0;

            //particleArray[i].life = Random.Range(3, 6);
        }

        particleBuffer = new ComputeBuffer(ParticleCount, Particle.stride);
        particleBuffer.SetData(particleArray);
    }

}
