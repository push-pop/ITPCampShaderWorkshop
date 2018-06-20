using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleForce : MonoBehaviour
{

    [System.Serializable]
    public struct ForceInfo
    {
        public Vector3 Center;
        public float Range;
        public float Falloff;
        public float Power;

        public static int stride = 6 * sizeof(float);
    }

    [SerializeField] float Range;
    [SerializeField] float Falloff;
    [SerializeField] float Power;

    // Update is called once per frame
    void Update()
    {
        _forceInfo.Center = transform.position;
        _forceInfo.Range = Range;
        _forceInfo.Falloff = Falloff;
        _forceInfo.Power = Power;
    }

    public ForceInfo Info
    {
        get { return _forceInfo; }
    }
    [SerializeField]
    ForceInfo _forceInfo;
}
