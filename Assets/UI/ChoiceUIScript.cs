using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChoiceUIScript : MonoBehaviour
{
    private BattleUIScript m_battleUi;

    public void OnAttackPressed()
    {
        m_battleUi.GoToAttack();
    }

    public void OnPokemonPressed()
    {
        m_battleUi.GoToChoosePkmn();
    }

    public void OnBagPressed()
    {
        m_battleUi.GoToBag();
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
