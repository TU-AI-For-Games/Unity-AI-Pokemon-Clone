using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Range(0f, 100f)][SerializeField] private float m_speed;
    [Range(0f, 720f)][SerializeField] private float m_rotationSpeed;
    

    private Rigidbody m_rigidBody;

    // Start is called before the first frame update
    void Start()
    {
        m_rigidBody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        m_rigidBody.velocity = transform.forward * Input.GetAxisRaw(StringConstants.FORWARD) * m_speed;

        transform.Rotate(Vector3.up, m_rotationSpeed * Time.deltaTime * Input.GetAxisRaw(StringConstants.ROTATE));
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag(StringConstants.WILD_POKEMON_TAG))
        {
            GameManager.Instance.StartBattle();
        }
    }
}
