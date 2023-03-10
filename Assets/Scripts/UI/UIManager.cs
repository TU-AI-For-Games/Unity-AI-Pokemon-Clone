using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class UIManager : MonoBehaviour
{

    [SerializeField] public GameObject _battleUI;


    public void SetBattleUI(bool Active)
    {
        _battleUI.SetActive(Active);
    }

}
