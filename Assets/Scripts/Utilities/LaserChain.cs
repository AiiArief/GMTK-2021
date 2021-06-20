using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserChain : MonoBehaviour
{
    private Entity m_entity;

    [SerializeField] CollisionChecker m_laserTrigger;
    public CollisionChecker laserTrigger { get { return m_laserTrigger; } }

    LineRenderer m_laserLine;
    Vector3 m_laserLineDefaultPos;
    BoxCollider m_childCollider;

    [HideInInspector] public Entity laserTo;
    [HideInInspector] public RaycastHit laserHit { get; private set; }

    public void SetupLaserChain(Entity entity)
    {
        // setup player id, setup string input
        m_laserLine = m_laserTrigger.GetComponent<LineRenderer>();
        m_childCollider = m_laserTrigger.GetComponentInChildren<BoxCollider>();

        m_entity = entity;

        m_laserLineDefaultPos = m_laserLine.GetPosition(0);
    }

    private void Update()
    {
        if(laserTo)
        {
            transform.LookAt(laserTo.transform.position);

            // setel range,
            float range = Vector3.Distance(m_laserTrigger.transform.position, laserTo.transform.position);
            m_childCollider.size = new Vector3(m_childCollider.size.x, m_childCollider.size.y, range);
            m_childCollider.center = new Vector3(m_childCollider.center.x, m_childCollider.center.y, range / 2);

            // setel line renderer
            RaycastHit hit;
            var dir = (laserTo.transform.position - transform.position).normalized;
            if (Physics.Raycast(m_childCollider.transform.position, dir, out hit, range, -1, QueryTriggerInteraction.Ignore))
            {
                laserHit = hit;
                m_laserLine.SetPosition(1, new Vector3(m_laserLineDefaultPos.x, m_laserLineDefaultPos.y, Vector3.Distance(transform.position, laserHit.point)));

                Entity entity = hit.collider.GetComponent<Entity>();
                if (entity && entity != laserTo) //&& PhaseManager.Instance.currentPhase == PhaseEnum.AfterInput)
                {
                    entity.DeadOrRevive(true);
                }
            }
            else
            {
                m_laserLine.SetPosition(1, new Vector3(m_laserLineDefaultPos.x, m_laserLineDefaultPos.y, range));
            }
        }
    }
}
