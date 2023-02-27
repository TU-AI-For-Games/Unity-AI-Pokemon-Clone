using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WildPocketMonster : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(StringConstants.PLAYER_TAG))
        {
            GameManager.Instance.StartBattle();
        }
    }
}
