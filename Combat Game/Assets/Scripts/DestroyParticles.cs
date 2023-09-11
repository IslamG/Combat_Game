using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyParticles : MonoBehaviour
{
    public float _timeDelay = 2.0f;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("DestroyParticleSystem");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator DestroyParticleSystem()
    {
        yield return new WaitForSeconds( _timeDelay );
        Destroy(gameObject);
    }
}
