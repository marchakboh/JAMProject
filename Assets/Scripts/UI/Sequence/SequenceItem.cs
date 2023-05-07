using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Timers;

public class SequenceItem : MonoBehaviour
{
    [SerializeField] private string action_key;

    private Transform red_circle;
    private Transform green_circle;
    private Transform main_image;

    private int red_size = 200;
    private float CircleDelta;

    public event Action OnFailed;
    private bool isActive = false;
    private float min_size = 0.5f;
    private float max_size = 0.7f;
    private RectTransform red_transform;
    private bool CanScale = false;

    private Timer timer;

    private void Start()
    {
        red_circle = transform.Find("RedImage");
        green_circle = transform.Find("GreenImage");
        main_image = transform.Find("MainImage");

        red_transform = red_circle.transform.GetComponent<RectTransform>();
        transform.gameObject.SetActive(false);

        timer = new Timer(30);
        timer.AutoReset = true;
        timer.Elapsed += AnimateElements;
    }

    private void AnimateElements(object sender, ElapsedEventArgs e)
    {
        if (isActive)
        {
            CanScale = true;
        }
    }

    public void ActivateSequence()
    {
        transform.gameObject.SetActive(true);
        isActive = true;
        timer.Start();
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
            if (CanScale)
            {
                red_transform.localScale = new Vector3(red_transform.localScale.x - CircleDelta, red_transform.localScale.y - CircleDelta, red_transform.localScale.z - CircleDelta);
                CanScale = false;
            }

            if (red_transform.localScale.x < min_size)
            {
                Deactivate();
                OnFailed();
            }
        }
    }

    public void SetCircleDelta(float delta)
    {
        CircleDelta = delta;
    }

    public void ResetPosition(int x, int y)
    {
        RectTransform tr = transform.GetComponent<RectTransform>();
        tr.anchoredPosition = new Vector2((float)x, (float)y);
    }

    private void Deactivate()
    {
        isActive = false;
        red_transform.localScale = Vector3.one;
        transform.gameObject.SetActive(false);
        timer.Stop();
    }
}
