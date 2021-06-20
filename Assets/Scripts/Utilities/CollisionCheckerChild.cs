using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionCheckerChild : MonoBehaviour
{
    BoxCollider m_boxTriggerer;

    public HashSet<Collider> m_colliders = new HashSet<Collider>();
    public HashSet<Collider> GetColliders() { return m_colliders; }

    public HashSet<T> GetCollidersWithFilter<T>()
    {
        HashSet<T> hashSet = new HashSet<T>();
        foreach(Collider collider in m_colliders)
        {
            T t = collider.GetComponent<T>();
            if(t != null)
            {
                hashSet.Add(t);
            }
        }
        return hashSet;
    }

    public bool CheckColliderHaveEntityTag<T>()
    {
        foreach(Collider collider in m_colliders)
        {
            if (!collider.isTrigger && collider.gameObject.isStatic)
                return true;
            if (collider.GetComponent<T>() != null)
                return true;
        }

        return false;
    }

    private void Awake()
    {
        m_boxTriggerer = GetComponent<BoxCollider>();
    }

    private void LateUpdate()
    {
        //if(m_colliders.Count > 0)
        //{
        //    Collider[] colliders = new Collider[m_colliders.Count];
        //    m_colliders.CopyTo(colliders);
        //    for(int i=colliders.Length - 1; i>= 0; i--)
        //    {
        //        if (m_boxTriggerer.ClosestPoint(colliders[i].transform.position) != colliders[i].transform.position)
        //            m_colliders.Remove(colliders[i]);
        //    }

        //    m_colliders = new HashSet<Collider>(colliders);
        //}
    }

    private void OnTriggerEnter(Collider other)
    {
        m_colliders.Add(other);
    }

    // this shit is buggy
    private void OnTriggerExit(Collider other)
    {
        m_colliders.Remove(other);
    }
}
