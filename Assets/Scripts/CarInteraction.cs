using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
public class CarInteraction : MonoBehaviour
{
    public GameObject alpacaPlayer;
    [SerializeField] ParticleSystem successParticles;
    private InputAction interact;
    public ControlsInput playerControls;

    bool canInteract = false;
    // Start is called before the first frame update
    void Awake()
    {
        playerControls = new ControlsInput();
        interact = playerControls.Controls.Interact;
        successParticles.Stop();
        interact.Enable();
    }

    void TryInteract()
    {
        if(canInteract)
        {
            Debug.Log("Lama is here");
            successParticles.Play();
            canInteract = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            canInteract = true;
            interact.performed += ctx => {TryInteract();};
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Debug.Log("Lama away");
            interact.performed -= ctx => {TryInteract();};
        }
    }
}
