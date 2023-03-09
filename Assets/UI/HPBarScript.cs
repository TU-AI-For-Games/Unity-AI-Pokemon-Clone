using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class HPBarScript : MonoBehaviour
{
    [SerializeField] 
    public TMP_Text name_text;
    [SerializeField]
    public Image health_bar;
    [SerializeField]
    public TMP_Text hp_amount_text;
    

    private void Update()
    {
        name_text.text = "Pikachu";
        hp_amount_text.text = "<size=36><color=#3C3C3C>150<size=20><color=#ABABAB>/150";

        var slider = health_bar.GetComponent<Slider>();

        slider.value = 0.5f;

    }
}
