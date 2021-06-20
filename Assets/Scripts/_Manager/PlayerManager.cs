using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;

    public List<EntityPlayer> players { get; private set; } = new List<EntityPlayer>();

    public int playerIdHost { get; private set; } = 0;

    public void SetupPlayersOnLevelStart(int onLevelStartPlayerCount)
    {
        _AssignAllPlayers();
        _SetPlayersIsActive(onLevelStartPlayerCount);
    }

    public void SetupAllPlayersWaitInput()
    {
        foreach (EntityPlayer player in players)
        {
            if (player.isPlayable)
                player.SetupWaitInput();
        }
    }

    public bool CheckAllPlayersHasDoneInput()
    {
        //panggil semua player buat liat input
        foreach (EntityPlayer player in players)
        {
            if (player.isPlayable && player.storedActions.Count == 0)
                player.WaitInput();
        }

        // kalo semua udah ada stored action return true
        foreach (EntityPlayer player in players)
        {
            if (player.isPlayable && player.storedActions.Count == 0)
                return false;
        }

        return true;
    }

    public void SetupAllPlayersProcessInput()
    {
        foreach (EntityPlayer player in players)
        {
            player.SetupProcessInput();
        }
    }

    public bool CheckAllPlayersHasDoneProcess()
    {
        // panggil semua player process
        foreach (EntityPlayer player in players)
        {
            if (player.isPlayable && !player.CheckAllActionHasDone())
            {
                foreach (StoredAction storedAction in player.storedActions)
                    if (!storedAction.actionHasDone)
                        storedAction.action.Invoke();
            }
        }

        // return true kalo semua udah selesai process
        foreach (EntityPlayer player in players)
        {
            if (player.isPlayable && !player.CheckAllActionHasDone())
                return false;
        }

        return true;
    }

    public void SetupAllPlayersAfterInput()
    {
        foreach (EntityPlayer player in players)
        {
            player.SetupAfterInput();
        }
    }

    public bool CheckAllPlayersHasDoneAfterInput()
    {
        // panggil semua player after input
        foreach (EntityPlayer player in players)
        {
            if (player.isPlayable)
                player.AfterInput();
        }

        // return true kalo semua udah selesai after input
        foreach (EntityPlayer player in players)
        {
            if (player.isPlayable && !player.afterActionHasDone)
                return false;
        }


        _UpdateAllPlayerLasers();
        return true;
    }

    public List<EntityPlayer> GetPlayerPlayableList()
    {
        var playablePlayers = new List<EntityPlayer>();
        foreach (EntityPlayer player in players)
        {
            if (player.isPlayable)
                playablePlayers.Add(player);
        }

        return playablePlayers;
    }

    public List<EntityPlayer> GetPlayerAliveList()
    {
        var playablePlayers = GetPlayerPlayableList();
        var alivePlayers = new List<EntityPlayer>();
        foreach(EntityPlayer player in playablePlayers)
        {
            if (player.deadTurnLeft == 0)
                alivePlayers.Add(player);
        }

        return alivePlayers;
    }

    public void SetPlayerPlayable(int playerId, bool set)
    {
        players[playerId].SetIsPlayable(set);
        _SetupAllPlayersCamera(GetPlayerPlayableList());
        //_UpdateAllPlayerLasers();
    }

    public void SetPlayerAsHost(int playerId)
    {
        playerIdHost = playerId;
    }

    private void Awake()
    {
        Instance = this;
    }

    private void _AssignAllPlayers()
    {
        foreach (Transform child in transform)
        {
            EntityPlayer player = child.GetComponent<EntityPlayer>();
            if (player) players.Add(player);
        }
    }

    private void _SetPlayersIsActive(int playerCount)
    {
        for (int i = 0; i < playerCount; i++)
            players[i].SetIsPlayable(true);

        for (int i = playerCount; i < players.Count; i++)
            players[i].SetIsPlayable(false);

        _SetupAllPlayersCamera(GetPlayerPlayableList());
    }

    private void _SetupAllPlayersCamera(List<EntityPlayer> playablePlayers)
    {
        int playerCount = playablePlayers.Count;
        switch(playerCount)
        {
            case 1:
                playablePlayers[0].playerCameraLook.playerCamera.rect = new Rect(0, 0, 1, 1);
                break;
            case 2:
                playablePlayers[0].playerCameraLook.playerCamera.rect = new Rect(0, 0, 0.5f, 1);
                playablePlayers[1].playerCameraLook.playerCamera.rect = new Rect(0.5f, 0, 0.5f, 1);
                break;
            case 3:
                playablePlayers[0].playerCameraLook.playerCamera.rect = new Rect(0, 0.5f, 1, 0.5f);
                playablePlayers[1].playerCameraLook.playerCamera.rect = new Rect(0, 0, 0.5f, 0.5f);
                playablePlayers[2].playerCameraLook.playerCamera.rect = new Rect(0.5f, 0, 0.5f, 0.5f);
                break;
            case 4:
                playablePlayers[0].playerCameraLook.playerCamera.rect = new Rect(0, 0.5f, 0.5f, 0.5f);
                playablePlayers[1].playerCameraLook.playerCamera.rect = new Rect(0.5f, 0.5f, 0.5f, 0.5f);
                playablePlayers[2].playerCameraLook.playerCamera.rect = new Rect(0, 0, 0.5f, 0.5f);
                playablePlayers[3].playerCameraLook.playerCamera.rect = new Rect(0.5f, 0, 0.5f, 0.5f);
                break;
        }
    }
    public void _UpdateAllPlayerLasers()
    {
        //var alivePlayers = GetPlayerPlayableList();
        var alivePlayers = GetPlayerAliveList();
        // cek kalo udah mati juga yak
        for (int i = 0; i < alivePlayers.Count; i++)
        {
            int iNext = (int)Mathf.Repeat(i + 1, alivePlayers.Count);
            if (alivePlayers.Count > 1 && alivePlayers[iNext].laserChain.laserTo != alivePlayers[i])
            {
                alivePlayers[i].laserChain.gameObject.SetActive(true);
                alivePlayers[i].laserChain.laserTo = alivePlayers[iNext];
            }
            else
            {
                alivePlayers[i].laserChain.gameObject.SetActive(false);
                alivePlayers[i].laserChain.laserTo = null;
            }
        }

        for (int i=0; i < players.Count; i++)
        {
            if(!players[i].isPlayable || players[i].deadTurnLeft > 0)
            {
                players[i].laserChain.gameObject.SetActive(false);
                players[i].laserChain.laserTo = null;
            }
        }
    }
}
