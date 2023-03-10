using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class HPBarScript : MonoBehaviour
{
    [SerializeField] private TMP_Text name_text;
    
    [SerializeField] private Image health_bar;

    private Slider m_healthbarSlider;
    
    [SerializeField]
    public TMP_Text hp_amount_text;

    private PocketMonster m_pokemon;

    private void Start()
    {
        m_healthbarSlider = health_bar.GetComponent<Slider>();
    }

    private void Update()
    {
        if (m_pokemon != null)
        {
            hp_amount_text.text = "<size=36><color=#3C3C3C>" +
                $"{m_pokemon.GetStats().HP}" +
                "<size=20><color=#ABABAB>/150";


            m_healthbarSlider.value = m_pokemon.GetStats().HP / m_pokemon.GetStats().BaseHP;
        }
    }

    public void SetPokemon(PocketMonster activeMon)
    {
        m_pokemon = activeMon;
        name_text.text = activeMon.Name;

    }
}
