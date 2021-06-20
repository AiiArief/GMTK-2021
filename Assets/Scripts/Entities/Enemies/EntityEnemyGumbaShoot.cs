using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityEnemyGumbaShoot : EntityEnemy
{
    [SerializeField] BulletHandler m_bulletHandler;
    int m_shootTurn = 0;
    bool m_hasWaitInputShoot = false;

    public override void SetupWaitInput()
    {
        base.SetupWaitInput();
        m_hasWaitInputShoot = false;
    }
    public override void WaitInput()
    {
        if (deadTurnLeft > 0)
        {
            SkipTurnEntity();
            return;
        }

        var hostPlayer = PlayerManager.Instance.players[PlayerManager.Instance.playerIdHost];
        if (hostPlayer)
            PlayerMove(hostPlayer);

        if(!m_hasWaitInputShoot)
        {
            m_hasWaitInputShoot = true;
            m_shootTurn--;
            if (m_shootTurn < 0)
                m_shootTurn = Random.Range(4, 9);

            if (m_shootTurn > 0)
            {
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

    public void PlayerMove(EntityPlayer player)
    {
        float moveH = Input.GetAxisRaw("Horizontal" + " #" + player.playerId);
        float moveV = Input.GetAxisRaw("Vertical" + " #" + player.playerId);
        bool moveMod = Input.GetButton("Move Modifier" + " #" + player.playerId);
        bool camMod = Input.GetButton("Camera Modifier 1" + " #" + player.playerId) || Input.GetButton("Camera Modifier 2" + " #" + player.playerId);
        bool skipTurn = _CheckDoubleInput("Move Modifier" + " #" + player.playerId, 0.5f);
        bool shoot = Input.GetButtonUp("Shoot" + " #" + player.playerId);

        if (skipTurn)
        {
            storedActions.Add(new StoredActionMove(this));
            storedActions.Add(new StoredActionSkip());
            return;
        }

        bool isMoving = Mathf.Abs(moveH) > 0.0f || Mathf.Abs(moveV) > 0.0f;
        if (isMoving && !camMod)
        {
            float moveRange = moveMod ? 2 : 1;
            Vector3 moveDir = (Mathf.Abs(_FCInput(moveH) + _FCInput(moveV)) != 1.0f) ? new Vector3(0.0f, 0.0f, _FCInput(moveV)) : new Vector3(_FCInput(moveH), 0.0f, _FCInput(moveV));

            storedActions.Add(new StoredActionTurn(this, player.playerCameraLook.currentCameraRot.y));
            storedActions.Add(new StoredActionMove(this, moveDir, moveRange));
            return;
        }

        if (shoot)
        {
            storedActions.Add(new StoredActionMove(this));
            storedActions.Add(new StoredActionSkip());
        }
    }

    [HideInInspector] float m_buttonDownCount = 0;
    [HideInInspector] float m_buttonDownTime = 0;
    private bool _CheckDoubleInput(string inputName, float buttonDownDelay)
    {
        if (Input.GetButtonDown(inputName))
        {
            m_buttonDownCount++;
            if (m_buttonDownCount == 1) m_buttonDownTime = Time.time;
        }
        if (m_buttonDownCount > 1 && Time.time - m_buttonDownTime < buttonDownDelay)
        {
            m_buttonDownCount = 0;
            m_buttonDownTime = 0;
            return true;
        }
        else if (m_buttonDownCount > 2 || Time.time - m_buttonDownTime > 1) m_buttonDownCount = 0;
        return false;
    }

    private float _FCInput(float input)
    {
        return (input < 0.0f) ? Mathf.Floor(input) : Mathf.Ceil(input);
    }
}
