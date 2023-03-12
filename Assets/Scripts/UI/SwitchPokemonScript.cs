using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SwitchPokemonScript : MonoBehaviour
{
    [SerializeField] private GameObject m_pokemonInfo;
    [SerializeField] private TextMeshProUGUI m_pokemonInfoText;

    [SerializeField] private BattleUIScript m_battleUiScript;

    [SerializeField] private List<Button> m_buttons;
    private bool m_playerFainted;

    private void OnEnable()
    {
        if (GameManager.Instance.GetPlayerController().HasUsablePokemon())
        {
            InitPkmnMenu();
        }
        else
        {
            GameManager.Instance.EndBattle(true);
        }
    }

    private void InitPkmnMenu()
    {
        PocketMonster[] playerMon = GameManager.Instance.GetPlayerController().GetPokemon();

        for (int i = 0; i < 6; ++i)
        {
            PocketMonster currentMon = playerMon[i];

            if (currentMon.GetStats().HP <= 0)
            {
                SetPkmnButtonColourText(m_buttons[i], Color.gray, currentMon.Name + " (FAINTED)");
            }
            else
            {
                SetPkmnButtonColourText(m_buttons[i], BattleManager.Instance.TypeColours[(int)currentMon.Type],
                    currentMon.Name);
            }
        }
    }

    public void OnChoosePokemon(int index)
    {
        PlayerController player = GameManager.Instance.GetPlayerController();
        // Don't choose a fainted mon!
        if (player.GetPokemon()[index].GetStats().HP <= 0)
        {
            return;
        }

        player.SetActivePokemonIndex(index);

        BattleManager.Instance.SetPlayerPokemon(player.GetActivePokemon());

        // Tell the BattleManager that the player switched out
        BattleManager.Instance.PlayerSwitchedOut();

        m_playerFainted = false;

        OnBackPressed();
    }

    public void OnHoverPokemon(int index)
    {
        PocketMonster playerMon = GameManager.Instance.GetPlayerController().GetPokemon()[index];

        m_pokemonInfoText.text = $"<b><u>{playerMon.Name}</u></b>\n" +
                                 $"Type: <color=#{ColorUtility.ToHtmlStringRGB(BattleManager.Instance.TypeColours[(int)playerMon.Type])}>{PocketMonster.TypeToString(playerMon.Type)}</color>\n" +
                                 $"HP: {MathF.Max(0, playerMon.GetStats().HP)}\n" +
                                 $"Attack: {playerMon.GetStats().GetAttackStatBeforeBurn()}\n" +
                                 $"Defense: {playerMon.GetStats().GetDefense()}\n" +
                                 $"Speed: {playerMon.GetStats().GetSpeedStatBeforeParalyze()}\n" +
                                 "<b><u>Moves:</u></b>\n" +
                                 $"<color=#{ColorUtility.ToHtmlStringRGB(BattleManager.Instance.TypeColours[(int)playerMon.GetMoves()[0].Type])}>{playerMon.GetMoves()[0].Name}</color>, " +
                                 $"<color=#{ColorUtility.ToHtmlStringRGB(BattleManager.Instance.TypeColours[(int)playerMon.GetMoves()[1].Type])}>{playerMon.GetMoves()[1].Name}</color>, " +
                                 $"<color=#{ColorUtility.ToHtmlStringRGB(BattleManager.Instance.TypeColours[(int)playerMon.GetMoves()[2].Type])}>{playerMon.GetMoves()[2].Name}</color>, " +
                                 $"<color=#{ColorUtility.ToHtmlStringRGB(BattleManager.Instance.TypeColours[(int)playerMon.GetMoves()[3].Type])}>{playerMon.GetMoves()[3].Name}</color>";

        if (playerMon.GetStatusEffect() != PocketMonster.StatusType.None)
        {
            string hexColour = "";
            string condition = "";

            if (playerMon.GetStatusEffect() == PocketMonster.StatusType.Asleep)
            {
                hexColour = ColorUtility.ToHtmlStringRGB(
                    BattleManager.Instance.TypeColours[(int)PocketMonster.Element.Normal]);
                condition = "SLP";
            }
            else if (playerMon.GetStatusEffect() == PocketMonster.StatusType.Burned)
            {
                hexColour = ColorUtility.ToHtmlStringRGB(
                    BattleManager.Instance.TypeColours[(int)PocketMonster.Element.Fire]);
                condition = "BRN";
            }
            else if (playerMon.GetStatusEffect() == PocketMonster.StatusType.Frozen)
            {
                hexColour = ColorUtility.ToHtmlStringRGB(
                    BattleManager.Instance.TypeColours[(int)PocketMonster.Element.Ice]);
                condition = "FRZ";
            }
            else if (playerMon.GetStatusEffect() == PocketMonster.StatusType.Paralyzed)
            {
                hexColour = ColorUtility.ToHtmlStringRGB(
                    BattleManager.Instance.TypeColours[(int)PocketMonster.Element.Electric]);
                condition = "PAR";
            }
            else if (playerMon.GetStatusEffect() == PocketMonster.StatusType.Poisoned)
            {
                hexColour = ColorUtility.ToHtmlStringRGB(
                    BattleManager.Instance.TypeColours[(int)PocketMonster.Element.Poison]);
                condition = "PSN";
            }

            m_pokemonInfoText.text += $"\nStatus: <color=#{hexColour}>{condition}</color>";
        }

        m_pokemonInfo.SetActive(true);
    }

    public void SetPlayerFainted()
    {
        // We don't want the player to be able to cancel if their previous pokemon fainted
        m_playerFainted = true;
    }

    public void OnHoverPokemonExit()
    {
        m_pokemonInfo.SetActive(false);
    }

    public void OnBackPressed()
    {
        if (!m_playerFainted)
        {
            m_battleUiScript.SetScreen(BattleUIScript.Screens.Menu);
        }
    }

    private void SetPkmnButtonColourText(Button button, Color colour, string text)
    {
        button.GetComponent<Image>().color = colour;
        button.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = text;
    }

}