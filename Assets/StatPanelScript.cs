using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StatPanelScript : MonoBehaviour
{

    [SerializeField] public TMP_Text General;
    [SerializeField] public TMP_Text Stats;
    [SerializeField] public TMP_Text Moves;

    private void Start()
    {
        ClearText();
    }

    public void ClearText()
    {
        General.text = "";
        Stats.text = "";
        Moves.text = "";
    }
}
