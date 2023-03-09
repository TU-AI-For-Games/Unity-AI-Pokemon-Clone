using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


enum Screens
{
    Menu,
    Attack,
}
public class BattleUIScript : MonoBehaviour
{

    [SerializeField]
    public GameObject m_ChoiceUI;
    
    [SerializeField]
    public GameObject m_MovesUI;
    
    [SerializeField]
    public GameObject m_PlayerHealthBar;
    
    [SerializeField]
    public GameObject m_EnemyHealthBar;


    private ChoiceUIScript ChoiceUIScript;
    private MoveUIScript MoveUIScript;
    
    private void Awake()
    {
        GoToMenu();
        
        ChoiceUIScript = m_ChoiceUI.GetComponent<ChoiceUIScript>();
        MoveUIScript = m_MovesUI.GetComponent<MoveUIScript>();
        
        
        // Basic
        MoveUIScript._backButton.GetComponent<Button>().onClick.AddListener(GoToMenu);
        ChoiceUIScript._attack.GetComponent<Button>().onClick.AddListener(GoToAttack);
        ChoiceUIScript._escape.GetComponent<Button>().onClick.AddListener(EscapeBattle);
        
        // Moves
        
    }
    
    private void EscapeBattle()
    {
        //TODO: Escape Battle
        print("Escaped");
    }
    private void GoToMenu()
    {
        SetScreen(Screens.Menu);
    }
    private void GoToAttack()
    {
        SetScreen(Screens.Attack);
    }

    private void SetScreen(Screens screen)
    {

        switch (screen)
        {
            case Screens.Menu:
            {
                m_ChoiceUI.SetActive(true);
                m_MovesUI.SetActive(false);
            } break;
            case Screens.Attack:
            { 
                m_ChoiceUI.SetActive(false);
                m_MovesUI.SetActive(true);
            } break;

            default:
            { 
                m_ChoiceUI.SetActive(true);
                m_MovesUI.SetActive(false);
            } break;
        }
       
        
    }
    
}
