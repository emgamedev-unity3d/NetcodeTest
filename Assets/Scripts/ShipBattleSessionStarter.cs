using UnityEngine;
using Unity.Netcode;

public class ShipBattleSessionStarter : MonoBehaviour
{
    private NetworkManager m_networkManager;

    private void Start()
    {
        m_networkManager = gameObject.GetComponent<NetworkManager>();
        m_networkManager.ConnectionApprovalCallback += M_networkManager_ConnectionApprovalCallback;
        m_networkManager.OnClientConnectedCallback += M_networkManager_OnClientConnectedCallback;
        m_networkManager.OnServerStarted += M_networkManager_OnServerStarted;
    }

    private void M_networkManager_OnServerStarted()
    {
        NetworkLog.LogInfoServer("Connected!!... as server!");
    }

    private void M_networkManager_OnClientConnectedCallback(ulong obj)
    {
        NetworkLog.LogInfoServer("Connected!!... as client!");
    }

    private void M_networkManager_ConnectionApprovalCallback(byte[] arg1, ulong arg2, NetworkManager.ConnectionApprovedDelegate arg3)
    {
        NetworkLog.LogInfoServer("Connected approved!!");
    }

    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 300, 300));
        if (!m_networkManager.IsClient && !m_networkManager.IsServer)
        {
            StartButtons();
        }
        else
        {
            StatusLabels();
        }

        GUILayout.EndArea();
    }

    void StartButtons()
    {
        if (GUILayout.Button("Host"))
            m_networkManager.StartHost();

        if (GUILayout.Button("Client"))
            m_networkManager.StartClient();

        if (GUILayout.Button("Server"))
            m_networkManager.StartServer();
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
