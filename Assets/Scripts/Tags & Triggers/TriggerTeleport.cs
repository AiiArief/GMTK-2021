using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerTeleport : MonoBehaviour
{
    bool m_isTriggered = false;

    private void OnTriggerStay(Collider other)
    {
        EntityPlayer player = other.GetComponent<EntityPlayer>();
        if (!m_isTriggered && player && PhaseManager.Instance.currentPhase == PhaseEnum.AfterInput)
        {
            m_isTriggered = true;

            GameManager.Instance.Win();
        }
    }
}
