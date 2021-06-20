using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityEnemyGumba : EntityEnemy
{
    [SerializeField] CollisionChecker m_bladeTrigger;

    public override void WaitInput()
    {
        if(deadTurnLeft > 0)
        {
            SkipTurnEntity();
            return;
        }


        var alivePlayers = PlayerManager.Instance.GetPlayerAliveList();
        float closestDistance = Mathf.Infinity;
        EntityPlayer target = null;
        foreach(EntityPlayer player in alivePlayers)
        {
            float distance = Vector3.Distance(transform.position, player.transform.position);
            if (distance < closestDistance)
            {
                target = player;
                closestDistance = distance;
            }
        }

        if (target)
            SimpleMove(target.transform.position);
    }

    public override void AfterInput()
    {
        if (deadTurnLeft > 0)
        {
            base.AfterInput();
            return;
        }

        var playerInBlade = m_bladeTrigger.triggerCollider.GetCollidersWithFilter<EntityPlayer>();
        foreach (EntityPlayer player in playerInBlade)
        {
            if (player.deadTurnLeft == 0)
            {
                player.DeadOrRevive(true);
                m_bladeTrigger.triggerCollider.m_colliders.Remove(player.characterController);
            }
        }

        base.AfterInput();
    }

    public override void SkipTurnEntity(int turns = 1)
    {
        storedActions.Add(new StoredActionMove(this));
        base.SkipTurnEntity(turns);
    }

    public override void TurnToDegreeEntity(float degree)
    {
        storedActions.Add(new StoredActionMove(this));
        base.TurnToDegreeEntity(degree);
    }

    public void SimpleMove(Vector3 point)
    {
        if (Vector3.Distance(transform.position, point) > 1.0f)
        {
            float angle = Mathf.Atan2(point.x - transform.position.x, point.z - transform.position.z) * Mathf.Rad2Deg;

            storedActions.Add(new StoredActionTurn(this, angle, false));
            storedActions.Add(new StoredActionMove(this, Vector3.forward));
        }
        else
        {
            SkipTurnEntity();
        }
    }
}
