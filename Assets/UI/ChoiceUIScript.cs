using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChoiceUIScript : MonoBehaviour
{
    public GameObject m_attack;
    public GameObject m_pokemon;
    public GameObject m_bag;
    public GameObject m_escape;

    private BattleUIScript m_battleUi;

    public void OnAttackPressed()
    {
        m_battleUi.GoToAttack();
    }

    public void OnPokemonPressed()
    {

    }

    public void OnBagPressed()
    {

    }

    public void OnRunAwayPressed()
    {
        m_battleUi.EscapeBattle();
    }

    public void SetBattleUI(BattleUIScript battleUI)
    {
        m_battleUi = battleUI;
    }
}
