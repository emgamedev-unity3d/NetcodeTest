using UnityEngine;
using Unity.Netcode;
using System;

public class SpaceshipController : NetworkBehaviour
{
    [SerializeField]
    private Color[] PlayerColors;

    [SerializeField]
    private GameObject m_gameObjectHealthBar;

    private NetworkVariable<int> m_shipHealth = new NetworkVariable<int>(NetworkVariableReadPermission.Everyone, 5);

    private NetworkVariable<byte> m_shipColorId = new NetworkVariable<byte>(NetworkVariableReadPermission.OwnerOnly, 0);

    [SerializeField]
    private float m_MovementSpeed;

    [SerializeField]
    private GameObject m_BulletPrefab;

    [SerializeField]
    private SpriteRenderer m_SpriteRenderer;

    private Vector2 m_direction = new Vector2();

    private void Start()
    {
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Similar to Start(), but for the Networking session
    public override void OnNetworkSpawn()
    {
        m_SpriteRenderer = GetComponent<SpriteRenderer>();

        // your additional code here...
        UpdateColorServerRpc();

        base.OnNetworkSpawn();
    }

    private void OnEnable()
    {
        // Start listening for the team index being updated
        m_shipColorId.OnValueChanged += OnTeamChanged;
    }

    private void OnDisable()
    {
        // Stop listening for the team index being updated
        m_shipColorId.OnValueChanged -= OnTeamChanged;
    }

    private void OnTeamChanged(byte oldTeamIndex, byte newTeamIndex)
    {
        try
        {
            // Only clients need to update the renderer
            if (!IsClient) { return; }

            if(PlayerColors != null && m_SpriteRenderer != null)
            {
                GameLogFile.WriteToLog(OwnerClientId, $"Ship color ID: {newTeamIndex}, Ship color: { m_SpriteRenderer.color}\n");

                // Update the color of the player's mesh renderer
                m_SpriteRenderer.color = PlayerColors[newTeamIndex];
            }
            else
            {
                Debug.Log("something is null!");
                GameLogFile.WriteToLog(OwnerClientId, $"THIS IS NULL! PlayerColors:{PlayerColors == null}, m_SpriteRenderer:{m_SpriteRenderer == null} newTeamIndex:{newTeamIndex} oldTeamIndex:{oldTeamIndex}");
            }
        }
        catch(NullReferenceException nEx)
        {
            GameLogFile.WriteToLog(OwnerClientId, $"Source: {nEx.Source}\nStack Trace: {nEx.StackTrace}\n");
        }
        catch(Exception ex)
        {
            GameLogFile.WriteToLog(OwnerClientId, $"{ex.Message}\n");
        }
    }

    void Update()
    {
        if (IsServer)
        {
            UpdateServer();
        }

        if (IsClient)
        {
            UpdateClient();
        }
    }

    private void UpdateServer()
    {
        // server code here...
    }

    private void UpdateClient()
    {
        if (!IsLocalPlayer)
        {
            return;
        }

        m_direction = Vector2.zero;

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            m_direction.x -= 1f;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            m_direction.x += 1f;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            m_direction.y -= 1f;
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            m_direction.y += 1f;
        }
        m_direction.Normalize();

        // client lets the server know that it has moved it's player
        MoveSpaceShipServerRpc(m_direction);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Send a request to the server to spawn a bullet
            ShootBulletServerRPC();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void MoveSpaceShipServerRpc(Vector2 direction)
    {
        transform.Translate(direction * m_MovementSpeed * Time.deltaTime);
    }

    [ServerRpc(RequireOwnership = false)]
    private void UpdateColorServerRpc()
    {
        m_shipColorId.Value = (byte)(UnityEngine.Random.Range(0, PlayerColors.Length - 1));
    }

    [ServerRpc(RequireOwnership = false)]
    private void ShootBulletServerRPC()
    {
        if (m_BulletPrefab == null)
        {
            Debug.LogError("Cannot shoot, bullet prefab is null!");
            return;
        }

        var newBullet = Instantiate(m_BulletPrefab, transform.position, Quaternion.identity);
        var newBulletNetworkObject = newBullet.GetComponent<NetworkObject>();
        newBulletNetworkObject.Spawn();

        var shipBulletBehavior = newBullet.GetComponent<ShipBulletBehavior>();
        shipBulletBehavior.bulletOwner = gameObject;

        shipBulletBehavior.SetTrailColorClientRpc(PlayerColors[m_shipColorId.Value]);

        newBulletNetworkObject.gameObject.GetComponent<ShipBulletBehavior>().PlayParticlesClientRpc();
    }

    [ClientRpc]
    private void ShipIsDeadClientRPC()
    {
        if (IsOwner)
        {
            m_MovementSpeed = 0f;
        }
    }

    public void TakeDamage()
    {
        if (IsServer)
        {
            m_shipHealth.Value -= 1;

            var scale = m_gameObjectHealthBar.transform.localScale;
            scale.x = m_shipHealth.Value / 5f;
            m_gameObjectHealthBar.transform.localScale = scale;

            if (m_shipHealth.Value <= 0)
            {
                // update the client, tell it that it's spaceship is defeated
                ShipIsDeadClientRPC();
            }
        }
    }
}
