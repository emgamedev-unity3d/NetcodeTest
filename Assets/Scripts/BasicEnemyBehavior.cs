using UnityEngine;
using Unity.Netcode;

public class BasicEnemyBehavior : NetworkBehaviour
{
    public float m_EnemySpeed = 7f;

    private NetworkVariable<float> m_EnemyLifetime =
        new NetworkVariable<float>(NetworkVariableReadPermission.Everyone, 10f);

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
    }

    // Update is called once per frame
    void Update()
    {
        if (IsOwner)
        {
            transform.Translate(Vector2.left * m_EnemySpeed * Time.deltaTime);
        }

        if (IsServer)
        {
            m_EnemyLifetime.Value -= Time.deltaTime;
            if (m_EnemyLifetime.Value <= 0f)
            {
                DespawnEnemy();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Only react to trigger on the server
        if (!IsServer)
            return;

        // check if it's collided with a player spaceship
        var spacheshipController = other.gameObject.GetComponent<SpaceshipController>();
        if (spacheshipController != null)
        {
            DespawnEnemy();

            // tell the spaceship that it's taken damage
            spacheshipController.TakeDamage();
        }

        // check if it's collided with a player's bullet
        var shipBulletBehavior = other.gameObject.GetComponent<ShipBulletBehavior>();
        if (shipBulletBehavior != null)
        {
            DespawnEnemy();
        }
    }

    private void DespawnEnemy()
    {
        gameObject.SetActive(false);

        // Server tells clients that this object is no longer in play
        NetworkObject.Despawn();

        Destroy(gameObject);
    }
}
