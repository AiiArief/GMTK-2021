using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletHandler : MonoBehaviour
{
    public CollisionChecker[] bullets;

    public float bulletSpeedTilePerTurn = 3;

    public void Shoot(Vector3 position)
    {
        if (transform.childCount == 0)
            return;

        transform.LookAt(position);

        transform.GetChild(0).gameObject.SetActive(true);
        transform.GetChild(0).transform.parent = null;
    }

    public void PutBulletBack(CollisionChecker bullet)
    {
        bullet.gameObject.SetActive(false);
        bullet.transform.position = transform.position;
        bullet.transform.rotation = Quaternion.identity;
        bullet.transform.SetParent(transform, false);
    }

    private void Update()
    {
        if(PhaseManager.Instance.currentPhase == PhaseEnum.ProcessInput)
        {
            foreach (CollisionChecker bullet in bullets)
            {
                if (bullet.gameObject.activeSelf)
                    bullet.transform.Translate((Vector3.forward * bulletSpeedTilePerTurn / PhaseManager.Instance.processInput.minimumTimeBeforeNextPhase) * Time.deltaTime);
            }
        }

        if(PhaseManager.Instance.currentPhase == PhaseEnum.AfterInput)
        {
            foreach (CollisionChecker bullet in bullets)
            {
                var playerHits = bullet.triggerCollider.GetCollidersWithFilter<EntityPlayer>();
                foreach(EntityPlayer player in playerHits)
                {
                    if(player.deadTurnLeft == 0)
                    {
                        player.DeadOrRevive(true);
                        bullet.triggerCollider.m_colliders.Remove(player.characterController);
                    }
                }

                if(bullet.triggerCollider.CheckColliderHaveEntityTag<EntityPlayer>())
                {
                    PutBulletBack(bullet);
                }
            }
        }
    }
}
