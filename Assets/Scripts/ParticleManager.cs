using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ParticleSet
{
    public string name;
    public bool playOnce = true;
    public ParticleSystem Particle;
}
public class ParticleManager : Singleton<ParticleManager>
{
    public ParticleSet[] ParticleSets;
    public void PlayEffect(string name, Vector2 location)
    {
        foreach (ParticleSet ps in ParticleSets)
        {
            if (ps.name == name)
            {
                GameObject go = Instantiate(ps.Particle.gameObject, location, Quaternion.identity);
                if (ps.playOnce) Destroy(go, 3);
            }
        }
    }
}