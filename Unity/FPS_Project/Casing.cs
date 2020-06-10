using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Casing : MonoBehaviour
{
    [SerializeField]
    private Vector3 casingSpawnVelocity = new Vector3(1.0f, 1.2f, 0.3f);
    [SerializeField]
    private float casingSpin = 1.0f;

    private AudioSource audiosource;
    private Rigidbody rigidbody3D;

    // Start is called before the first frame update
    void Awake()
    {
        audiosource = this.GetComponent<AudioSource>();
        rigidbody3D = this.GetComponent<Rigidbody>();

        rigidbody3D.velocity = casingSpawnVelocity;
        rigidbody3D.angularVelocity = new Vector3(Random.Range(-casingSpin, casingSpin),
                                                                            Random.Range(-casingSpin, casingSpin),
                                                                            Random.Range(-casingSpin, casingSpin));
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        audiosource.Play(); //탄피가 어딘가에 부딪힐 때마다 OnCollisionEnter()에 의해 소리가 난다.
    }
    
}
