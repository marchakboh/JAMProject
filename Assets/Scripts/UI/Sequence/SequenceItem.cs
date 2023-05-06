using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class SequenceItem : MonoBehaviour
{
    [SerializeField] private string action_key;

    private Transform red_circle;
    private Transform green_circle;
    private Transform main_image;

    private int red_size = 200;

    public event Action OnFailed;
    private bool isActive = false;
    private float min_size = 0.5f;
    private float max_size = 0.7f;
    private RectTransform red_transform;

    private void Start()
    {
        red_circle = transform.Find("RedImage");
        green_circle = transform.Find("GreenImage");
        main_image = transform.Find("MainImage");

        red_transform = red_circle.transform.GetComponent<RectTransform>();
        transform.gameObject.SetActive(false);
    }

    public void ActivateSequence()
    {
        transform.gameObject.SetActive(true);
        isActive = true;
    }

    public bool IsKey(string key)
    {
        if (red_transform.localScale.x <= max_size && red_transform.localScale.x >= min_size && action_key.Equals(key))
        {
            Deactivate();
            return true;
        }
        Deactivate();
        return false;
    }

    private void Update()
    {
        if (isActive)
        {
            float delta = 0.001f;
            red_transform.localScale = new Vector3(red_transform.localScale.x - delta, red_transform.localScale.y - delta, red_transform.localScale.z - delta);

            if (red_transform.localScale.x < min_size)
            {
                Deactivate();
                OnFailed();
            }
        }
    }

    private void Deactivate()
    {
        isActive = false;
        red_transform.localScale = Vector3.one;
        transform.gameObject.SetActive(false);
    }
}
