using UnityEngine;
using Unity.Netcode;

public class BasicEnemyBehavior : NetworkBehaviour
{
    public float enemySpeed = 7f;

    public float rotateSpeed = 19f;

    private NetworkVariable<float> enemyLifetime =
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
            transform.Translate(Vector2.left * enemySpeed * Time.deltaTime);
        }

        if (IsServer)
        {
            enemyLifetime.Value -= Time.deltaTime;
            if (enemyLifetime.Value <= 0f)
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

        var spacheshipController = other.gameObject.GetComponent<SpaceshipController>();
        if (spacheshipController != null)
        {
            DespawnEnemy();

            spacheshipController.TakeDamage();
        }

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
