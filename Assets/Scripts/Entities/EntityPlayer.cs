using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityPlayer : Entity
{
    public bool isPlayable { get { return gameObject.activeSelf; } }
    public int playerId { get { return transform.GetSiblingIndex(); } }

    [SerializeField] CameraLook m_playerCameraLook;
    public CameraLook playerCameraLook { get { return m_playerCameraLook; } }

    [SerializeField] Transform m_crosshair_shoot;

    [SerializeField] LaserChain m_laserChain;
    public LaserChain laserChain { get { return m_laserChain; } }

    public override void WaitInput()
    {
        if (deadTurnLeft == 0)
        {
            Collider[] hits = _HandleCrosshair(30.0f);

            float moveH = Input.GetAxisRaw("Horizontal" + " #" + playerId);
            float moveV = Input.GetAxisRaw("Vertical" + " #" + playerId);
            bool moveMod = Input.GetButton("Move Modifier" + " #" + playerId);
            bool camMod = Input.GetButton("Camera Modifier 1" + " #" + playerId) || Input.GetButton("Camera Modifier 2" + " #" + playerId);
            bool skipTurn = _CheckDoubleInput("Move Modifier" + " #" + playerId, 0.5f);
            bool shoot = Input.GetButtonUp("Shoot" + " #" + playerId);
            bool dismiss = _CheckHoldInput("Shoot" + " #" + playerId, 1.0f);

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

                storedActions.Add(new StoredActionTurn(this, m_playerCameraLook.currentCameraRot));
                storedActions.Add(new StoredActionMove(this, moveDir, moveRange));
                return;
            }

            if (shoot)
            {
                storedActions.Add(new StoredActionMove(this));
                storedActions.Add(new StoredActionSkip());

                if (m_crosshair_shoot.gameObject.activeSelf)
                {
                    EntityPlayer playerHit = null;
                    foreach (Collider collider in hits)
                        if (collider.GetComponent<EntityPlayer>()) { playerHit = collider.GetComponent<EntityPlayer>(); break; }

                    if (playerHit)
                    {
                        if (!dismiss)
                            PlayerManager.Instance.SetPlayerAsHost(playerHit.playerId);
                        else
                            PlayerManager.Instance.SetPlayerPlayable(playerHit.playerId, false);
                        return;
                    }

                    if (PlayerManager.Instance.GetPlayerPlayableList().Count < PlayerManager.Instance.players.Count)
                    {
                        for (int i = 0; i < PlayerManager.Instance.players.Count; i++)
                        {
                            if (!PlayerManager.Instance.players[i].isPlayable)
                            {
                                PlayerManager.Instance.players[i].transform.position = m_crosshair_shoot.position;
                                PlayerManager.Instance.players[i].m_playerCameraLook.currentCameraRot = m_playerCameraLook.currentCameraRot;
                                PlayerManager.Instance.SetPlayerPlayable(i, true);
                                break;
                            }
                        }
                    }
                }
            }
        }
        else
        {
            storedActions.Add(new StoredActionSkip());
            deadTurnLeft--;

            if (deadTurnLeft == 0)
            {
                DeadOrRevive(false);
                PlayerManager.Instance.SetPlayerPlayable(playerId, false);
            }
        }
    }

    public override void SetupProcessInput()
    {
        base.SetupProcessInput();
        m_crosshair_shoot.gameObject.SetActive(false);
    }

    public override void AfterInput()
    {
        base.AfterInput();
    }

    public override void DeadOrRevive(bool isDead) {
        base.DeadOrRevive(isDead);

        if(isDead)
        {
            PlayerManager.Instance._UpdateAllPlayerLasers();
            var alivePlayer = PlayerManager.Instance.GetPlayerAliveList();

            if(alivePlayer.Count == 0)
            {
                GameManager.Instance.Lose();
                return;
            }

            if(PlayerManager.Instance.playerIdHost == playerId)
                PlayerManager.Instance.SetPlayerAsHost(alivePlayer[0].playerId);
        }
    }
    public void SetIsPlayable(bool newIsPlayable) { 
        gameObject.SetActive(newIsPlayable);
        storedActions.Clear();

        storedActions.Add(new StoredActionSkip());
    }

    protected override void Awake()
    {
        base.Awake();
        m_playerCameraLook.SetupCameraLook(this);
        m_laserChain.SetupLaserChain(this);
    }

    private float _FCInput(float input)
    {
        return (input < 0.0f) ? Mathf.Floor(input) : Mathf.Ceil(input);
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

    [HideInInspector] float m_buttonHoldTime = 0;
    private bool _CheckHoldInput(string inputName, float holdTime)
    {
        if(Input.GetButtonDown(inputName))
            m_buttonHoldTime = 0;

        if (Input.GetButton(inputName))
        {
            m_buttonHoldTime += Time.deltaTime;
        }

        return m_buttonHoldTime >= holdTime;
    }

    private Collider[] _HandleCrosshair(float range)
    {
        RaycastHit hit;
        Collider[] hitPointUpColliders = null;
        Vector3 hitPoint = Vector3.zero;
        bool isHost = playerId == PlayerManager.Instance.playerIdHost;
        bool isHit = Physics.Raycast(m_playerCameraLook.transform.position + m_playerCameraLook.transform.forward, m_playerCameraLook.transform.forward, out hit, range, -1, QueryTriggerInteraction.Ignore);
        if (isHit)
        {
            hitPoint = hit.point;
            hitPointUpColliders = Physics.OverlapBox(hit.point + Vector3.up, new Vector3(0.5f, 0.95f, 0.5f), Quaternion.identity, -1, QueryTriggerInteraction.Ignore);
            bool isCollidingSomething = hitPointUpColliders.Length > 0;
            foreach (Collider collider in hitPointUpColliders)
                if (collider.GetComponent<EntityPlayer>()) { isCollidingSomething = false; break; }

            if (isCollidingSomething || hit.collider.GetComponent<TagUnshootable>())
                isHit = false;
        }

        m_crosshair_shoot.gameObject.SetActive(isHost && isHit);
        m_crosshair_shoot.position = new Vector3(Mathf.Round(hitPoint.x), Mathf.Round(hitPoint.y), Mathf.Round(hitPoint.z));
        return hitPointUpColliders;
    }
}
