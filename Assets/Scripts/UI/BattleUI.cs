using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleUI : MonoBehaviour
{
    [SerializeField] private GameObject m_choiceUI;
    [SerializeField] private GameObject m_moveUI;
    [SerializeField] private GameObject m_monsterUI;
    [SerializeField] private GameObject m_bagUI;

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

}
