using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoveUIScript : MonoBehaviour
{
    
    [SerializeField]
    public GameObject _move1;
    public GameObject _move2;
    public GameObject _move3;
    public GameObject _move4;
    
    [SerializeField]
    public GameObject _backButton;
    

    private void Awake()
    {
        _move1.GetComponent<Button>().onClick.AddListener(DoMove1);
        _move2.GetComponent<Button>().onClick.AddListener(DoMove2);
        _move3.GetComponent<Button>().onClick.AddListener(DoMove3);
        _move4.GetComponent<Button>().onClick.AddListener(DoMove4);
        
        _backButton.GetComponent<Button>().onClick.AddListener(Back);
    }


    private void DoMove1()
    {
        
    }
    private void DoMove2()
    {
        
    }
    private void DoMove3()
    {
        
    }
    private void DoMove4()
    {
        
    }

    private void Back()
    {
        
        
    }
    
    
}
