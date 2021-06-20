using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityEnemyTrap: EntityEnemy
{
    [SerializeField] CollisionChecker m_spikeTrigger;

    public override void AfterInput()
    {
        if (deadTurnLeft > 0)
        {
            base.AfterInput();
            return;
        }

        var playerInSpike = m_spikeTrigger.triggerCollider.GetCollidersWithFilter<EntityPlayer>();
        foreach (EntityPlayer player in playerInSpike)
        {
            if(player.deadTurnLeft == 0)
            {
                player.DeadOrRevive(true);
                m_spikeTrigger.triggerCollider.m_colliders.Remove(player.characterController);
            }
        }

        base.AfterInput();
    }

    public override void DeadOrRevive(bool isDead)
    {
        base.DeadOrRevive(false);
    }
}
