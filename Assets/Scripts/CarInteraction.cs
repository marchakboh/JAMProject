using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarInteraction : MonoBehaviour
{
    public GameObject alpacaPlayer;
    [SerializeField] ParticleSystem successParticles;
    // Start is called before the first frame update
    void onAwake()
    {
        alpacaPlayer = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Player")
        {
            Debug.Log("lama is here");
            successParticles.Play();
        }
    }
}
