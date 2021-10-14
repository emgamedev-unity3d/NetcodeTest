using UnityEngine;
using Unity.Netcode;

public class ShipBattleSessionInfo : MonoBehaviour
{
    private NetworkManager m_networkManager;

    private void Start()
    {
        m_networkManager = NetworkManager.Singleton;
    }

    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 300, 300));
        if (m_networkManager.IsClient || m_networkManager.IsServer)
        {
            StatusLabels();
        }

        GUILayout.EndArea();
    }

    void StatusLabels()
    {
        var mode = m_networkManager.IsHost ?
            "Host" : m_networkManager.IsServer ? "Server" : "Client";

        GUILayout.Label("Transport: " +
            m_networkManager.NetworkConfig.NetworkTransport.GetType().Name);

        GUILayout.Label("Mode: " + mode);
    }
}
