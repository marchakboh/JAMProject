using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
public class CarInteraction : MonoBehaviour
{
    public GameObject alpacaPlayer;
    [SerializeField] ParticleSystem rings;
    [SerializeField] ParticleSystem coins;
    private InputAction interact;
    public ControlsInput playerControls;
    bool onceEntered = false;

    bool canInteract = false;
    // Start is called before the first frame update
    void Awake()
    {
        playerControls = new ControlsInput();
        interact = playerControls.Controls.Interact;
        interact.Enable();
        rings.Stop();
        coins.Stop();
    }

    void TryInteract()
    {
        if(canInteract)
        {
            Debug.Log("Lama is here");
            rings.Play();
            coins.Play();
            canInteract = false;
            onceEntered = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && !onceEntered)
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
