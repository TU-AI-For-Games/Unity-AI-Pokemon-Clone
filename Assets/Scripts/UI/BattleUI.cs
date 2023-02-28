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
        m_choiceUI.SetActive(false);
        m_moveUI.SetActive(true);
    }

    public void AttackBack()
    {
        m_choiceUI.SetActive(true);
        m_moveUI.SetActive(false);
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

    public void OnMove1Pressed()
    {
        BattleManager.Instance.PlayerAttack(0);
    }

    public void OnMove2Pressed()
    {
        BattleManager.Instance.PlayerAttack(1);
    }

    public void OnMove3Pressed()
    {
        BattleManager.Instance.PlayerAttack(2);
    }

    public void OnMove4Pressed()
    {
        BattleManager.Instance.PlayerAttack(3);
    }

    public void OnBattleInfoPressed()
    {

    }
}
