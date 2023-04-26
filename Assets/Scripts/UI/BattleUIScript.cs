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

    [SerializeField] private ItemUIScript m_bagUi;

    // TODO: GameObject for now, create a proper script!
    [SerializeField] private GameObject m_switchPokemonUi;

    [Header("Battle info UI")]
    [SerializeField] private GameObject m_battleInfo;
    [SerializeField] private TextMeshProUGUI m_battleInfoText;
    private bool m_displayedAllMessages = false;

    [Header("Pokeballs remaining UI")]
    [SerializeField] private GameObject m_trainerPokemonRemaining;
    [SerializeField] private GameObject m_playerPokemonRemaining;
    [SerializeField] private Sprite m_pokeballSprite;
    [SerializeField] private Sprite m_emptyPokeballSprite;


    public enum Screens
    {
        Menu,
        Attack,
        BattleInfo,
        ChoosePokemon,
        Bag
    }

    private void Awake()
    {
        SetScreen(Screens.Menu);

        m_ChoiceUI.SetBattleUI(this);
    }

    public void OnPlayerSwitchPokemon()
    {
        PocketMonster activeMon = GameManager.Instance.GetPlayerController().GetActivePokemon();
        Move[] activeMonMoves = activeMon.GetMoves();

        m_MovesUI.SetMovesText(activeMonMoves);

        m_PlayerHealthBar.SetPokemon(activeMon);
    }

    public void OnOtherSwitchPokemon(PocketMonster mon)
    {
        m_EnemyHealthBar.SetPokemon(mon);
    }

    public void SetScreen(Screens screen)
    {
        HideAll();

        switch (screen)
        {
            case Screens.Menu:
                {
                    m_ChoiceUI.gameObject.SetActive(true);
                }
                break;
            case Screens.Attack:
                {
                    m_MovesUI.gameObject.SetActive(true);
                }
                break;
            case Screens.BattleInfo:
                {
                    if (ShowNextBattleInfoText())
                    {
                        m_battleInfo.SetActive(true);
                    }
                    else
                    {
                        SetScreen(Screens.Menu);

                        // Go back to selecting the move
                        BattleManager.Instance.NextTurn();
                    }
                }
                break;
            case Screens.ChoosePokemon:
                {
                    m_switchPokemonUi.SetActive(true);
                }
                break;
            case Screens.Bag:
                {
                    m_bagUi.gameObject.SetActive(true);
                }
                break;
            default:
                {
                    m_ChoiceUI.gameObject.SetActive(true);
                }
                break;
        }
    }

    private bool ShowNextBattleInfoText()
    {
        string nextBattleMessage = BattleManager.Instance.ConsumeNextMessage();

        if (nextBattleMessage == null)
        {
            m_displayedAllMessages = true;
            return false;
        }

        m_battleInfoText.text = nextBattleMessage;
        m_displayedAllMessages = false;
        return true;
    }

    public void OnBattleInfoPressed()
    {
        SetScreen(Screens.BattleInfo);
    }

    public bool DisplayedAllMessages()
    {
        return m_displayedAllMessages;
    }

    private void HideAll()
    {
        m_ChoiceUI.gameObject.SetActive(false);
        m_MovesUI.gameObject.SetActive(false);

        m_switchPokemonUi.gameObject.SetActive(false); ;

        m_battleInfo.gameObject.SetActive(false); ;
        m_bagUi.gameObject.SetActive(false);
    }

    public void ShowPokeballs(PocketMonster[] pokemon, bool isPlayer)
    {
        int amountOfPokemon = pokemon.Length - 1;
        for (int i = 0; i < 6; ++i)
        {
            GameObject pokeballUiObject = isPlayer ? m_playerPokemonRemaining : m_trainerPokemonRemaining;

            if (i > amountOfPokemon || pokemon[i].HasFainted())
            {
                pokeballUiObject.transform.GetChild(i).gameObject.GetComponent<Image>().sprite = m_emptyPokeballSprite;
            }
            else
            {
                pokeballUiObject.transform.GetChild(i).gameObject.GetComponent<Image>().sprite = m_pokeballSprite;
            }
        }
    }

    public void SetTrainerBalls(bool activeState)
    {
        m_trainerPokemonRemaining.SetActive(activeState);
    }
}
