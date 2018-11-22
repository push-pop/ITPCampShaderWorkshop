using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LGF_ParticleSolverSimple : MonoBehaviour
{

    public int ParticleCount = 5000;

    public ComputeShader Kernels;
    public ComputeBuffer particleBuffer;
    public ComputeBuffer forceBuffer;

    public Vector3 ConstantVelocity = Vector3.zero;
    public Vector3 center;
    public float power;

    int UpdateParticleIndex;

    LGF_ParticleRenderSimple render;

    // Use this for initialization
    void Start()
    {
        render = GetComponent<LGF_ParticleRenderSimple>();
        ParticleCount = render.InputMesh.triangles.Length / 3;
        UpdateParticleIndex = Kernels.FindKernel("UpdateParticles");
        particleBuffer = render.CreateInitialPositions(particleBuffer);
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyUp(KeyCode.Space)) {
        //    CreateInitialPositions();
        //}
    
        Kernels.SetBuffer(UpdateParticleIndex, "particleBuffer", particleBuffer);
        Kernels.SetFloat("deltaTime", Time.deltaTime);
        Kernels.SetFloat("power", power);
        Kernels.SetVector("center", center);
        Kernels.SetVector("constantVelocity", ConstantVelocity);
        Kernels.Dispatch(UpdateParticleIndex, ParticleCount, 1, 1);
    }

    private void OnDestroy()
    {
        if (particleBuffer != null)
            particleBuffer.Release();
        if (forceBuffer != null)
            forceBuffer.Release();
    }
    
    //private void CreateInitialPositions()
    //{
    //    Particle[] particleArray = new Particle[ParticleCount];
    //    for (int i = 0; i < ParticleCount; ++i)
    //    {
    //        particleArray[i].position = 2 * Random.onUnitSphere;
    //        particleArray[i].velocity.x = 0;
    //        particleArray[i].velocity.y = 0;
    //        particleArray[i].velocity.z = 0;
    //    }

    //    particleBuffer = new ComputeBuffer(ParticleCount, Particle.stride);
    //    particleBuffer.SetData(particleArray);
    //}

}
