using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleUI : MonoBehaviour
{
    [SerializeField] private PlayerController m_player;

    [SerializeField] private List<Color> m_typeColours;

    [SerializeField] private GameObject m_choiceUI;
    [SerializeField] private GameObject m_monsterUI;
    [SerializeField] private GameObject m_bagUI;

    [Header("HP UI")]
    [SerializeField] private TextMeshProUGUI m_playerPkmnName;
    [SerializeField] private Slider m_playerHpSlider;
    [SerializeField] private TextMeshProUGUI m_otherPkmnName;
    [SerializeField] private Slider m_otherHpSlider;

    [Header("Move UI")]
    [SerializeField] private GameObject m_moveInfo;
    [SerializeField] private TextMeshProUGUI m_moveDescription;
    [SerializeField] private GameObject m_moveUI;
    [SerializeField] private List<Button> m_moveButtons;

    [Header("Battle info UI")]
    [SerializeField] private GameObject m_battleInfo;
    [SerializeField] private TextMeshProUGUI m_battleInfoText;
    private bool m_displayedAllMessages = false;

    [Header("Switch Pokemon UI")]
    [SerializeField] private List<Button> m_pokemonButtons;

    private void Awake()
    {
        OnPlayerSwitchPokemon();
    }

    private void Update()
    {
        // TODO: THINK OF A BETTER WAY TO DO THIS... It's 1am I want to go to sleep :'(
        Stats playerMon = BattleManager.Instance.GetPlayerPokemon().GetStats();
        Stats otherMon = BattleManager.Instance.GetOtherPokemon().GetStats();
        m_playerHpSlider.value = playerMon.HP / (float)playerMon.BaseHP;
        m_otherHpSlider.value = otherMon.HP / (float)otherMon.BaseHP;
    }


    public void Attack()
    {
        HideChoiceUI();
        ShowMoveUI();
    }

    public void AttackBack()
    {
        m_choiceUI.SetActive(true);
        HideMoveUI();
    }

    public void OnPlayerSwitchPokemon()
    {
        PocketMonster activeMon = m_player.GetActivePokemon();
        Move[] activeMonMoves = activeMon.GetMoves();

        for (int i = 0; i < activeMonMoves.Length; ++i)
        {
            SetButtonColourText(m_moveButtons[i], m_typeColours[(int)activeMonMoves[i].Type], activeMonMoves[i].Name);
        }

        m_playerPkmnName.text = activeMon.Name;

        m_playerHpSlider.value = (float)activeMon.GetStats().HP / activeMon.GetStats().BaseHP;
    }

    public void OnOtherSwitchPokemon(PocketMonster mon)
    {
        m_otherPkmnName.text = mon.Name;
        m_otherHpSlider.value = (float)mon.GetStats().HP / mon.GetStats().BaseHP;
    }

    public void OnRunAwayPressed()
    {
        GameManager.Instance.EndBattle(true);
    }

    private void SetMoveDescription(Move move)
    {
        m_moveDescription.text =
            $"{move.Description}\nEFFECT: {MoveManager.EffectToString(move.MoveEffect)}\nDAMAGE: {move.Damage}\nACCURACY: {move.Accuracy}%\nTYPE: {PocketMonster.TypeToString(move.Type)}";
    }

    public void OnMoveHoverExit()
    {
        m_moveInfo.SetActive(false);
    }

    public void OnMoveHover(int index)
    {
        m_moveInfo.SetActive(true);
        SetMoveDescription(m_player.GetActivePokemon().GetMoves()[index]);
    }

    public void ShowMoveUI()
    {
        m_moveUI.SetActive(true);
    }

    public void HideMoveUI()
    {
        m_moveUI.SetActive(false);
    }

    public void ShowChoiceUI()
    {
        m_choiceUI.SetActive(true);
    }

    public void HideChoiceUI()
    {
        m_choiceUI.SetActive(false);
    }

    public void ShowBattleInfoUI()
    {
        HideMoveUI();
        m_battleInfo.SetActive(true);
        m_displayedAllMessages = false;

        if (m_battleInfoText.text == "")
        {
            ShowNextBattleInfoText();
        }
    }

    public void HideBattleInfoUI()
    {
        m_battleInfo.SetActive(false);
    }

    public void SetBattleInfoText(string text)
    {
        m_battleInfoText.text = text;
    }

    public void OnMoveButtonPressed(int index)
    {
        PocketMonster playerMon = m_player.GetActivePokemon();
        playerMon.SetChosenMove(playerMon.GetMoves()[index]);

        // Once the player has selected their move, we want to inform the battle manager to start the attack cycle
        BattleManager.Instance.SetBattleState(BattleManager.BattleState.Attack);
    }

    private void ResetBattleInfoText()
    {
        m_battleInfoText.text = "";
    }

    public void OnBattleInfoPressed()
    {
        if (ShowNextBattleInfoText())
        {
            ShowBattleInfoUI();
        }
        else
        {
            m_displayedAllMessages = true;

            HideBattleInfoUI();
            ResetBattleInfoText();

            // Go back to selecting the move
            BattleManager.Instance.NextTurn();
        }
    }

    private bool ShowNextBattleInfoText()
    {
        string nextBattleMessage = BattleManager.Instance.ConsumeNextMessage();

        if (nextBattleMessage == null)
        {
            return false;
        }

        SetBattleInfoText(nextBattleMessage);

        return true;
    }

    public bool DisplayedAllMessages()
    {
        return m_displayedAllMessages;
    }

    public void ShowChoosePkmnMenu()
    {
        if (m_player.HasUsablePokemon())
        {
            SetChoosePkmnMenuUI();
            m_monsterUI.SetActive(true);
        }
        else
        {
            GameManager.Instance.EndBattle(true);
        }
    }

    private void SetChoosePkmnMenuUI()
    {
        PocketMonster[] playerMon = m_player.GetPokemon();

        for (int i = 0; i < 6; ++i)
        {
            PocketMonster currentMon = playerMon[i];

            if (currentMon.GetStats().HP <= 0)
            {
                SetButtonColourText(m_pokemonButtons[i], Color.gray, currentMon.Name + " (FAINTED)");
            }
            else
            {
                SetButtonColourText(m_pokemonButtons[i], m_typeColours[(int)currentMon.Type], currentMon.Name);
            }
        }
    }

    public static void SetButtonColourText(Button button, Color colour, string text)
    {
        button.GetComponent<Image>().color = colour;
        button.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = text;
    }

    public void OnChoosePokemon(int index)
    {
        // Don't choose a fainted mon!
        if (m_player.GetPokemon()[index].GetStats().HP <= 0)
        {
            return;
        }

        m_player.SetActivePokemonIndex(index);

        BattleManager.Instance.SetPlayerPokemon(m_player.GetActivePokemon());

        BattleManager.Instance.SetBattleState(BattleManager.BattleState.SelectMove);

        OnChoosePkmnBack();
    }

    public void OnChoosePkmnBack()
    {
        m_monsterUI.SetActive(false);
        m_choiceUI.SetActive(true);
    }
}