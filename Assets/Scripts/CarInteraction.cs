using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
public class CarInteraction : MonoBehaviour
{
    [SerializeField] ParticleSystem rings;
    [SerializeField] ParticleSystem coins;
    private bool canInteract = true;

    void Awake()
    {
        rings.Stop();
        coins.Stop();
    }

    public void TryHit()
    {
        rings.Play();
        coins.Play();
    }

    public void TryDestroy()
    {
        canInteract = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && canInteract)
        {
            AlpacaCharacter character = other.gameObject.GetComponent<AlpacaCharacter>();
            character.SetCurrentInteractable(this);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            AlpacaCharacter character = other.gameObject.GetComponent<AlpacaCharacter>();
            character.RemoveCurrentInteractable();
        }
    }
}
