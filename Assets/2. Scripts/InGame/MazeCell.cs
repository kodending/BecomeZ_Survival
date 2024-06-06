using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeCell : MonoBehaviour
{
    [SerializeField]
    private GameObject m_goLeftWall;

    [SerializeField]
    private GameObject m_goRightWall;

    [SerializeField]
    private GameObject m_goFrontWall;

    [SerializeField]
    private GameObject m_goBackWall;

    [SerializeField]
    private GameObject m_goUnvisitedBlock;

    [Tooltip("≈ª√‚ ¡ˆ¡°")]
    public GameObject m_goExitPoint;

    public bool m_bVisited { get; private set; }

    public void Visit()
    {
        m_bVisited = true;
        m_goUnvisitedBlock.SetActive(false);
    }

    public void ClearLeftWall()
    {
        m_goLeftWall.SetActive(false);
    }

    public void ClearRightWall()
    {
        m_goRightWall.SetActive(false);
    }

    public void ClearFrontWall()
    {
        m_goFrontWall.SetActive(false);
    }

    public void ClearBackWall()
    {
        m_goBackWall.SetActive(false);
    }
}
