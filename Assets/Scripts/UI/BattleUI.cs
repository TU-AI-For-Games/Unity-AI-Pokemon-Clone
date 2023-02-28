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
    [SerializeField] private Button m_move1Button;
    [SerializeField] private TextMeshProUGUI m_move1Text;
    [SerializeField] private Button m_move2Button;
    [SerializeField] private TextMeshProUGUI m_move2Text;
    [SerializeField] private Button m_move3Button;
    [SerializeField] private TextMeshProUGUI m_move3Text;
    [SerializeField] private Button m_move4Button;
    [SerializeField] private TextMeshProUGUI m_move4Text;

    [Header("Battle info UI")]
    [SerializeField] private GameObject m_battleInfo;
    [SerializeField] private TextMeshProUGUI m_battleInfoText;
    private bool m_displayedAllMessages = false;

    private void Awake()
    {
        OnPlayerSwitchPokemon();
    }

    private void Update()
    {
        // TODO: THINK OF A BETTER WAY TO DO THIS... It's 1am I want to go to sleep :'(
        PocketMonster.Stats playerMon = BattleManager.Instance.GetPlayerPokemon().GetStats();
        PocketMonster.Stats otherMon = BattleManager.Instance.GetOtherPokemon().GetStats();
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

        // TODO: There's DEFINITELY a much better way of doing this... Oh well, this will work for now...
        m_move1Text.text = activeMonMoves[0].Name;
        m_move2Text.text = activeMonMoves[1].Name;
        m_move3Text.text = activeMonMoves[2].Name;
        m_move4Text.text = activeMonMoves[3].Name;

        m_move1Button.GetComponent<Image>().color = m_typeColours[(int)activeMonMoves[0].Type];
        m_move2Button.GetComponent<Image>().color = m_typeColours[(int)activeMonMoves[1].Type];
        m_move3Button.GetComponent<Image>().color = m_typeColours[(int)activeMonMoves[2].Type];
        m_move4Button.GetComponent<Image>().color = m_typeColours[(int)activeMonMoves[3].Type];

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

    public void OnMove1Pressed()
    {
        SetPlayerChoice(0);
    }


    public void OnMove1Hover()
    {
        OnHover(0);
    }

    public void OnMove2Pressed()
    {
        SetPlayerChoice(1);
    }

    public void OnMove2Hover()
    {
        OnHover(1);
    }

    public void OnMove3Pressed()
    {
        SetPlayerChoice(2);
    }

    public void OnMove3Hover()
    {
        OnHover(2);
    }

    public void OnMove4Pressed()
    {
        SetPlayerChoice(3);
    }

    public void OnMove4Hover()
    {
        OnHover(3);
    }

    private void OnHover(int index)
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
        ShowNextBattleInfoText();
        m_displayedAllMessages = false;
    }

    public void HideBattleInfoUI()
    {
        m_battleInfo.SetActive(false);
    }

    public void SetBattleInfoText(string text)
    {
        m_battleInfoText.text = text;
    }

    private void SetPlayerChoice(int index)
    {
        PocketMonster playerMon = m_player.GetActivePokemon();
        playerMon.SetChosenMove(playerMon.GetMoves()[index]);

        // Once the player has selected their move, we want to inform the battle manager to start the attack cycle
        BattleManager.Instance.SetBattleState(BattleManager.BattleState.Attack);
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
            // TODO: For now, just randomly choose a pokemon
            do
            {
                int randomIndex = Random.Range(0, 6);
                Debug.Log("Choosing a random mon!");
                m_player.SetActivePokemonIndex(randomIndex);

            } while (m_player.GetActivePokemon().GetStats().HP <= 0);

            BattleManager.Instance.SetPlayerPokemon(m_player.GetActivePokemon());

            BattleManager.Instance.SetBattleState(BattleManager.BattleState.SelectMove);

            ShowChoiceUI();
        }
        else
        {
            GameManager.Instance.EndBattle(true);
        }
    }
}