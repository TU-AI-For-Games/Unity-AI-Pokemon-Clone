using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class MoveButtonScript : MonoBehaviour
{

    [SerializeField] public TMP_Text MoveName;

    private Button Button;

    private void Awake()
    {
        Button = GetComponent<Button>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
