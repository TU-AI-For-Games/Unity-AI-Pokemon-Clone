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

    [SerializeField] private Slider m_healthbarSlider;

    [SerializeField] private TMP_Text m_hpText;

    private PocketMonster m_pokemon;

    private void Update()
    {
        if (m_pokemon == null)
            return;

        m_hpText.text = $"<size=36><color=#3C3C3C>{MathF.Max(0, m_pokemon.GetStats().HP)}</size></color>" +
                        "<size=20><color=#ABABAB>/150</size></color>";


        m_healthbarSlider.value = m_pokemon.GetStats().HP / (float)m_pokemon.GetStats().BaseHP;
    }

    public void SetPokemon(PocketMonster activeMon)
    {
        m_pokemon = activeMon;
        name_text.text = activeMon.Name;
    }
}
