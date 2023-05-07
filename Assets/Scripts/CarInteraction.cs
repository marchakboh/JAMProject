using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.SceneManagement;
public class CarInteraction : MonoBehaviour
{
    [SerializeField] ParticleSystem rings;
    [SerializeField] ParticleSystem coins;
    [SerializeField] int SequenceHitCount;
    [SerializeField] int MoneyCount;
    [SerializeField] int XP_Count;
    [SerializeField] int LvlLock;

    [SerializeField] private bool isLastCar;

    private GameObject textObject;
    private bool canInteract = true;
    private int hit_count = 0;

    void Awake()
    {
        rings.Stop();
        coins.Stop();
    }

    void Start()
    {
        textObject = transform.Find("Text").gameObject;
        textObject.SetActive(false);
    }

    public bool TryHit()
    {
        rings.Play();
        coins.Play();

        hit_count++;
        if (hit_count == SequenceHitCount)
        {
            textObject.SetActive(false);
            if (isLastCar)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            }
            return true;
        }

        return false;
    }

    public void Restore()
    {
        hit_count = 0;
    }

    public void TryDestroy()
    {
        canInteract = false;
    }

    public int GetXPPrize()
    {
        return XP_Count;
    }

    public int GetMoneyPrize()
    {
        return MoneyCount;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && canInteract)
        {
            AlpacaCharacter character = other.gameObject.GetComponent<AlpacaCharacter>();
            character.SetCurrentInteractable(transform.gameObject);
            textObject.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            AlpacaCharacter character = other.gameObject.GetComponent<AlpacaCharacter>();
            character.RemoveCurrentInteractable();
            textObject.SetActive(false);
        }
    }
}
