using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemUIScript : MonoBehaviour
{
    [SerializeField] private BattleUIScript m_battleUiScript;

    public void OnItemPressed(int item)
    {
        BattleManager.Instance.UseItem(
            (BattleManager.Item)item,
            BattleManager.Instance.GetPlayerPokemon()
        );

        m_battleUiScript.SetScreen(BattleUIScript.Screens.BattleInfo);
    }

    public void OnBackPressed()
    {
        gameObject.SetActive(false);
        m_battleUiScript.SetScreen(BattleUIScript.Screens.Menu);
    }
}
