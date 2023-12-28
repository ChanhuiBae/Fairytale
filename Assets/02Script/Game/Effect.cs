
using UnityEngine;

public class Effect : MonoBehaviour
{
    private ParticleSystem particle;
    private bool doPlay;

    private void Awake()
    {
        if (!TryGetComponent<ParticleSystem>(out particle))
            Debug.Log("Effect - Init - ParticleSystem");
    }
    public void IniEffect(GameObject pool, string name)
    {
        doPlay = false;
    }
    public void PlayEffect()
    {
        particle.Play();
        doPlay = true;
    }

    private void Update()
    {
        if (doPlay && !particle.isPlaying)
        {
            doPlay = false;
        }
    }

    public void StopEffect()
    {
        particle.Stop();
        doPlay = false;
    }
}
