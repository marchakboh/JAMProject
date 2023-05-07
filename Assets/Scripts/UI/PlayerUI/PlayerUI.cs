using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private Sprite SpriteLVL_1;
    [SerializeField] private Sprite SpriteLVL_2;
    [SerializeField] private Sprite SpriteLVL_3;

    private Slider xp_slider;
    private TMPro.TextMeshProUGUI lvl_text;
    private TMPro.TextMeshProUGUI money_text;
    private Image player_image;

    private void Start()
    {
        Transform canva = transform.Find("Canvas");
        player_image = canva.Find("PlayerImage").gameObject.GetComponent<Image>();
        xp_slider = canva.Find("ProgressBar").gameObject.GetComponent<Slider>();
        lvl_text = canva.Find("LvlText").gameObject.GetComponent<TMPro.TextMeshProUGUI>();
        money_text = canva.Find("MoneyText").gameObject.GetComponent<TMPro.TextMeshProUGUI>();

        xp_slider.maxValue = 1f;
    }

    public void SetXPValue(float value)
    {
        xp_slider.value = value;
    }

    public void SetLVL(int lvl)
    {
        lvl_text.text = lvl + " LVL";
        if (lvl == 1)
        {
            player_image.sprite = SpriteLVL_1;
        }
        else if (lvl == 2)
        {
            player_image.sprite = SpriteLVL_2;
        }
        else
        {
            player_image.sprite = SpriteLVL_3;
        }
    }

    public void SetMoneyCount(int count)
    {
        money_text.text = count + " $";
    }
}
