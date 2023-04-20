using System;
using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T m_instance;

    public static T GetInstance
    {
        get
        {
            if (!m_instance)
            {
                m_instance = FindObjectOfType<T>();
                if (!m_instance)
                {
                    GameObject ins = new GameObject();
                    m_instance = ins.AddComponent<T>();
                }
            }
            return m_instance;
        }
    }
}