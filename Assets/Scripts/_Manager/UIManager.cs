using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [SerializeField] RectTransform[] m_playersPanel;
    Text[] m_playersText;

    public void Transition(Color transitionColor, string sceneName)
    {
        transform.Find("Transition").GetComponent<Image>().color = transitionColor;
        GetComponent<Animator>().SetTrigger("transition");
        StartCoroutine(TransitionCoroutine(sceneName));
    }

    private void Awake()
    {
        Instance = this;

        m_playersText = new Text[m_playersPanel.Length];
        for(int i=0; i<m_playersPanel.Length; i++)
        {
            m_playersText[i] = m_playersPanel[i].GetChild(1).GetComponent<Text>();
        }
    }

    private void Update()
    {
        _SetupAllPlayersCamera(PlayerManager.Instance.GetPlayerPlayableList());
        HandlePanelContent();
    }

    private void _SetupAllPlayersCamera(List<EntityPlayer> playablePlayers)
    {
        foreach (RectTransform playerPanel in m_playersPanel)
            playerPanel.gameObject.SetActive(false);

        int playerCount = playablePlayers.Count;
        for(int i=0; i<playerCount; i++)
        {
            m_playersPanel[playablePlayers[i].playerId].gameObject.SetActive(true);
            switch (playerCount)
            {
                case 1:
                    m_playersPanel[playablePlayers[i].playerId].anchoredPosition = new Vector2(640, -360);
                    m_playersPanel[playablePlayers[i].playerId].sizeDelta = new Vector2(1280, 720);
                    break;
                case 2:
                    m_playersPanel[playablePlayers[i].playerId].anchoredPosition = new Vector2(640 / 2 + (640 * (i % 2)), -360);
                    m_playersPanel[playablePlayers[i].playerId].sizeDelta = new Vector2(1280 / 2, 720);
                    break;
                case 3:
                    if(i==0)
                    {
                        m_playersPanel[playablePlayers[i].playerId].anchoredPosition = new Vector2(640, -360 / 2);
                        m_playersPanel[playablePlayers[i].playerId].sizeDelta = new Vector2(1280, 720 / 2);
                    } else
                    {
                        m_playersPanel[playablePlayers[i].playerId].anchoredPosition = new Vector2(640 / 2 + (640 * (i == 1 ? 0 : 1)), -360 / 2 - (360 * (i >= 1 ? 1 : 0)));
                        m_playersPanel[playablePlayers[i].playerId].sizeDelta = new Vector2(1280 / 2, 720 / 2);
                    }
                    break;
                case 4:
                    m_playersPanel[playablePlayers[i].playerId].anchoredPosition = new Vector2(640 / 2 + (640 * (i % 2)), -360 / 2 - (360 * (i >= 2 ? 1 : 0)));
                    m_playersPanel[playablePlayers[i].playerId].sizeDelta = new Vector2(1280 / 2, 720 / 2);
                    break;
            }
        }
    }

    private void HandlePanelContent()
    {
        var pm = PlayerManager.Instance;
        for(int i=0; i<m_playersText.Length; i++)
        {
            m_playersText[i].color = Color.white;
            if (pm.players[i].deadTurnLeft > 0)
            {
                m_playersPanel[i].GetChild(0).gameObject.SetActive(true);
                m_playersText[i].text = "This clone is obliterated. Wait for " + pm.players[i].deadTurnLeft + " turn(s) left";
                continue;
            }

            m_playersPanel[i].GetChild(0).gameObject.SetActive(false);
            if(pm.players[i].playerId == pm.playerIdHost)
            {
                m_playersText[i].text = "Player " + (pm.players[i].playerId + 1);
                m_playersText[i].color = Color.red;
            } else
            {
                m_playersText[i].text = "Clone " + (pm.players[i].playerId + 1);
            }

        }
    }

    private IEnumerator TransitionCoroutine(string sceneName)
    {
        yield return new WaitForSeconds(1.0f);
        SceneManager.LoadScene(sceneName);
    }
}
