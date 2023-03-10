using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChoiceUIScript : MonoBehaviour
{
    private BattleUIScript m_battleUi;

    public void OnAttackPressed()
    {
        m_battleUi.SetScreen(BattleUIScript.Screens.Attack);
    }

    public void OnPokemonPressed()
    {
        m_battleUi.SetScreen(BattleUIScript.Screens.ChoosePokemon);
    }

    public void OnBagPressed()
    {
        m_battleUi.SetScreen(BattleUIScript.Screens.Bag);
    }

    public void OnRunAwayPressed()
    {
    }

    public void SetBattleUI(BattleUIScript battleUI)
    {
        m_battleUi = battleUI;
    }
}
