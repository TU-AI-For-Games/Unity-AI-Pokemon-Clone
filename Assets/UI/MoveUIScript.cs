using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MoveUIScript : MonoBehaviour
{
    [Header("Move UI")]
    [SerializeField] private GameObject m_moveInfo;
    [SerializeField] private TextMeshProUGUI m_moveDescription;
    [SerializeField] private GameObject m_moveUI;

    [SerializeField] private List<MoveButtonContainer> moveButtons;

    private class MoveButtonContainer
    {
        public Button MoveButton;
        public TextMeshProUGUI MoveText;
    }

    public void OnMoveHover(int moveNumber)
    {
        m_moveInfo.SetActive(true);

        Move move = GameManager.Instance.GetPlayerController().GetActivePokemon().GetMoves()[moveNumber];

        m_moveDescription.text = $"{move.Description}\nEFFECT: {MoveManager.EffectToString(move.MoveEffect)}\nDAMAGE: {move.Damage}\nACCURACY: {move.Accuracy}%\nTYPE: {PocketMonster.TypeToString(move.Type)}";
    }

    public void SetBattleUI(BattleUIScript battleUIScript)
    {
    }

    public void OnMoveHoverExit()
    {
        m_moveInfo.SetActive(false);
    }

    public void OnMovePressed(int moveNumber)
    {
        PocketMonster playerMon = GameManager.Instance.GetPlayerController().GetActivePokemon();
        playerMon.SetChosenMove(playerMon.GetMoves()[moveNumber]);

        // Once the player has selected their move, we want to inform the battle manager to start the attack cycle
        BattleManager.Instance.SetBattleState(BattleManager.BattleState.Attack);
    }

    public void SetMovesText(Move[] activeMonMoves)
    {
        for (int i = 0; i < 4; ++i)
        {
            moveButtons[i].MoveButton.GetComponent<Image>().color = BattleManager.Instance.TypeColours[(int)activeMonMoves[0].Type];
            moveButtons[i].MoveText.text = activeMonMoves[i].Name;
        }
    }
}
