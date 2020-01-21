using UnityEngine;

namespace NestedFadeGroup
{
    [ExecuteInEditMode]
    [NestedFadeGroupBridge(typeof(ParticleSystem))]
    [RequireComponent(typeof(ParticleSystem))]
    public class NestedFadeGroupParticleSystem : NestedFadeGroupBase
    {
        private ParticleSystem system;
        private ParticleSystem.Particle[] particles;

        protected override void GetMissingReferences()
        {
            if (!system)
            {
                system = GetComponent<ParticleSystem>();

                // Max particles is often set very high, so calculate max particles instead if possible (can't if looping)
                var main = system.main;
                int maxParticles = 0;
                if (main.loop)
                    maxParticles = main.maxParticles;
                else
                {
                    var emission = system.emission;
                    float maxRate = 0;
                    switch (emission.rateOverTime.mode)
                    {
                        case ParticleSystemCurveMode.Constant:
                            maxRate = emission.rateOverTime.constant;
                            break;

                        case ParticleSystemCurveMode.TwoConstants:
                            maxRate = emission.rateOverTime.constantMax;
                            break;

                        default:
                            Debug.LogErrorFormat(this, "Can't calculate max particles for mode: {0}", emission.rateOverTime.mode.ToString());
                            break;
                    }

                    maxParticles = Mathf.CeilToInt(maxRate * main.duration);
                }

                if (maxParticles <= 1000)
                    particles = new ParticleSystem.Particle[maxParticles];
                else
                {
                    Debug.LogErrorFormat(this, "Max particle amount {0} is too high! ParticleSystem will not be faded.", maxParticles);
                    particles = null;
                }
            }
        }

        protected override void OnAlphaChanged(float alpha)
        {
            if (particles == null || system == null)
                return;

            var main = system.main;
            var startColor = main.startColor;
            var startColorValue = startColor.color;
            startColorValue.a = alpha;
            startColor.color = startColorValue;
            main.startColor = startColor;

            int particleCount = system.GetParticles(particles);
            if (particleCount > 0)
            {
                for (int i = 0; i < particleCount; i++)
                    particles[i].startColor = startColor.color;
            }
            system.SetParticles(particles, particleCount);
        }
    }
}