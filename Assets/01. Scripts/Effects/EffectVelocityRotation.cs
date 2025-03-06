using UnityEngine;

[ExecuteInEditMode]
public class EffectVelocityRotation : MonoBehaviour
{
    public float angleOffset;

    private ParticleSystem.Particle[] particles;

    private void LateUpdate()
    {
        int particleCount = GetComponent<ParticleSystem>().particleCount;
        if (particleCount > 0)
        {
            particles = new ParticleSystem.Particle[particleCount];
            GetComponent<ParticleSystem>().GetParticles(particles);

            for (int i = 0; i < particleCount; i++)
            {
                Vector2 velocity = particles[i].velocity;
                if (velocity.sqrMagnitude > 0.01f)
                {
                    float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
                    particles[i].rotation = -angle + angleOffset;
                }
            }

            GetComponent<ParticleSystem>().SetParticles(particles, particleCount);
        }
    }
}
