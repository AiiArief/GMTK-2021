using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    PhaseManager m_phaseManager;
    PlayerManager m_playerManager;
    EnemyManager m_enemyManager;
    LevelManager m_levelManager;

    [SerializeField] string nextLevelString;

    public void Win()
    {
        UIManager.Instance.Transition(Color.white, nextLevelString);
        m_phaseManager.SetPhase(PhaseEnum.None);
    }

    public void Lose()
    {
        UIManager.Instance.Transition(Color.black, UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        m_phaseManager.SetPhase(PhaseEnum.None);
    }

    private void Awake()
    {
        Instance = this;

        m_phaseManager = GetComponentInChildren<PhaseManager>();
        m_playerManager = GetComponentInChildren<PlayerManager>();
        m_enemyManager = GetComponentInChildren<EnemyManager>();
        m_levelManager = GetComponentInChildren<LevelManager>();
    }

    private void OnEnable()
    {

        // set cutscene, config all manager, idk
        m_playerManager.SetupPlayersOnLevelStart(1); // temp
        m_enemyManager.SetupEnemiesOnLevelStart();
        m_phaseManager.SetPhase(PhaseEnum.WaitInput);
        m_levelManager.SetupLevelPathfindingOnLevelStart();

        Cursor.lockState = CursorLockMode.Locked; // kalo pause munculin cursor;
        Application.targetFrameRate = 60;
    }

    private void Update()
    {
        if (Input.GetButtonDown("Restart"))
            Lose();

        // kalo win atau apapun ya jangan update atuh
        if (m_phaseManager.currentPhase != PhaseEnum.None)
            m_phaseManager.UpdateCurrentPhase();
    }
}
