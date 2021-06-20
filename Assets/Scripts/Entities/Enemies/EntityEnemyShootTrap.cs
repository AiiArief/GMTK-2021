using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityEnemyShootTrap : EntityEnemy
{
    [SerializeField] BulletHandler m_bulletHandler;

    int m_shootTurn = 0;

    public override void WaitInput()
    {
        if (deadTurnLeft > 0)
        {
            base.WaitInput();
            return;
        }

        m_shootTurn--;
        if (m_shootTurn < 0)
            m_shootTurn = Random.Range(4, 9);

        if(m_shootTurn > 0)
        {
            SkipTurnEntity();
            return;
        }

        // aim ke closest player
        // shoot
        var alivePlayers = PlayerManager.Instance.GetPlayerAliveList();
        float closestDistance = Mathf.Infinity;
        EntityPlayer target = null;
        foreach (EntityPlayer player in alivePlayers)
        {
            float distance = Vector3.Distance(transform.position, player.transform.position);
            if (distance < closestDistance)
            {
                target = player;
                closestDistance = distance;
            }
        }

        if (target)
            m_bulletHandler.Shoot(target.transform.position);
    }

    public override void DeadOrRevive(bool isDead)
    {
        base.DeadOrRevive(false);
    }
}
