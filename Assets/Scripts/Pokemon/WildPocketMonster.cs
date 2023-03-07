using UnityEngine;

public class WildPocketMonster : MonoBehaviour
{
    public PocketMonster Pokemon { get; private set; }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetPokemon(PocketMonster mon)
    {
        Pokemon = mon;
    }
}
