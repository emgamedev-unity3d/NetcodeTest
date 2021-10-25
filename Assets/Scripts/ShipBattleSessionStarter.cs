using UnityEngine;
using Unity.Netcode;

public class ShipBattleSessionInfo : MonoBehaviour
{
    private NetworkManager m_NetworkManager;

    private void Start()
    {
        m_NetworkManager = NetworkManager.Singleton;
    }

    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 300, 300));
        if (m_NetworkManager.IsClient || m_NetworkManager.IsServer)
        {
            StatusLabels();
        }

        GUILayout.EndArea();
    }

    void StatusLabels()
    {
        var connectionMode = m_NetworkManager.IsHost ?
            "Host" : m_NetworkManager.IsServer ? "Server" : "Client";

        GUILayout.Label("Transport: " +
            m_NetworkManager.NetworkConfig.NetworkTransport.GetType().Name);

        GUILayout.Label("Mode: " + connectionMode);
    }
}
