using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class BattleCamera : MonoBehaviour
{
    [SerializeField] private float m_dollySpeed;

    [SerializeField] private Transform m_playerPokemonPosition;
    [SerializeField] private Transform m_otherPokemonPosition;

    private CinemachineVirtualCamera m_camera;
    private CinemachineTrackedDolly m_dollyCam;
    private float m_pathLength;

    private void Start()
    {
        m_camera = GetComponent<CinemachineVirtualCamera>();
        m_dollyCam = m_camera.GetCinemachineComponent<CinemachineTrackedDolly>();

        m_pathLength = m_dollyCam.m_Path.PathLength;

        m_camera.Follow = null;
        m_camera.LookAt = m_otherPokemonPosition;
    }


    private void Update()
    {
        m_dollyCam.m_PathPosition += m_dollySpeed * Time.deltaTime;

        if (m_dollyCam.m_PathPosition / m_pathLength > 1.0)
        {
            m_camera.LookAt = m_otherPokemonPosition;
        }
        else if (m_dollyCam.m_PathPosition / m_pathLength > 0.5)
        {
            m_camera.LookAt = m_playerPokemonPosition;
        }

        if (m_dollyCam.m_PathPosition > m_pathLength)
        {
            m_dollyCam.m_PathPosition -= m_pathLength;
        }
    }
}
