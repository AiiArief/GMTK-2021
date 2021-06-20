using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerArea : MonoBehaviour
{
    [SerializeField] GameObject[] m_doors;

    [SerializeField] EntityEnemy[] m_enemyRequirements;

    bool m_isTriggered = false;

    // cek kalo semua musuh udah mati
    private void Update()
    {
        bool allEnemiesIsDead = true;
        foreach(EntityEnemy enemy in m_enemyRequirements)
        {
            if (enemy.deadTurnLeft == 0)
            {
                allEnemiesIsDead = false;
                break;
            }
        }

        foreach (GameObject door in m_doors)
            door.SetActive(!allEnemiesIsDead);
    }

    private void OnTriggerStay(Collider other)
    {
        EntityPlayer player = other.GetComponent<EntityPlayer>();
        if (!m_isTriggered && player && PhaseManager.Instance.currentPhase == PhaseEnum.AfterInput)
        {
            m_isTriggered = true;

            // bikin semua player unplayable kecuali yang kena
            PlayerManager.Instance.SetPlayerAsHost(player.playerId);
            for(int i=0; i<PlayerManager.Instance.players.Count; i++)
            {
                if (PlayerManager.Instance.players[i] == player)
                    continue;

                PlayerManager.Instance.players[i].DeadOrRevive(false);
                PlayerManager.Instance.SetPlayerPlayable(i, false);
            }

            foreach(EntityEnemy enemy in m_enemyRequirements)
            {
                enemy.DeadOrRevive(false);
            }
        }
    }
}
