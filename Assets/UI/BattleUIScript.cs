using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class BattleUIScript : MonoBehaviour
{
    [SerializeField] private ChoiceUIScript m_ChoiceUI;

    [SerializeField] private MoveUIScript m_MovesUI;

    [SerializeField] private HPBarScript m_PlayerHealthBar;

    [SerializeField] private HPBarScript m_EnemyHealthBar;

    [Header("Battle info UI")]
    [SerializeField] private GameObject m_battleInfo;
    [SerializeField] private TextMeshProUGUI m_battleInfoText;
    private bool m_displayedAllMessages = false;

    public enum Screens
    {
        Menu,
        Attack,
        BattleInfo,
    }

    private void Awake()
    {
        GoToMenu();

        m_ChoiceUI.SetBattleUI(this);
        m_MovesUI.SetBattleUI(this);
    }

    public void EscapeBattle()
    {
        //TODO: Escape Battle
        print("Escaped");
    }

    public void GoToMenu()
    {
        SetScreen(Screens.Menu);
    }

    public void GoToAttack()
    {
        SetScreen(Screens.Attack);
    }

    public void OnPlayerSwitchPokemon()
    {
        PocketMonster activeMon = GameManager.Instance.GetPlayerController().GetActivePokemon();
        Move[] activeMonMoves = activeMon.GetMoves();

        m_MovesUI.SetMovesText(activeMonMoves);

        m_PlayerHealthBar.SetPokemon(activeMon);
    }

    private void SetScreen(Screens screen)
    {

        switch (screen)
        {
            case Screens.Menu:
                {
                    m_ChoiceUI.gameObject.SetActive(true);
                    m_MovesUI.gameObject.SetActive(false);
                }
                break;
            case Screens.Attack:
                {
                    m_ChoiceUI.gameObject.SetActive(false);
                    m_MovesUI.gameObject.SetActive(true);
                }
                break;
            case Screens.BattleInfo:
                {
                    m_MovesUI.gameObject.SetActive(false);
                    m_battleInfo.SetActive(true);
                    ShowNextBattleInfoText();
                    m_displayedAllMessages = false;
                }
                break;
            default:
                {
                    m_ChoiceUI.gameObject.SetActive(true);
                    m_MovesUI.gameObject.SetActive(false);
                }
                break;
        }
    }

    private bool ShowNextBattleInfoText()
    {
        string nextBattleMessage = BattleManager.Instance.ConsumeNextMessage();

        if (nextBattleMessage == null)
        {
            return false;
        }

        m_battleInfoText.text = nextBattleMessage;

        return true;
    }

    public void OnBattleInfoPressed()
    {
        if (ShowNextBattleInfoText())
        {
            SetScreen(Screens.BattleInfo);
        }
        else
        {
            m_displayedAllMessages = true;

            m_battleInfo.gameObject.SetActive(false);

            // Go back to selecting the move
            BattleManager.Instance.NextTurn();
        }
    }

}
