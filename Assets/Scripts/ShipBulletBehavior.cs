using UnityEngine;
using Unity.Netcode;

public class ShipBulletBehavior : NetworkBehaviour
{
    public float m_BulletSpeed = 10f;

    public GameObject m_BulletOwner = null;

    [SerializeField]
    private SpriteRenderer m_SpriteRenderer;

    [SerializeField]
    private ParticleSystem m_TrailGameObjectParticleSystem = null;

    private NetworkVariable<float> m_BulletLifetime = 
        new NetworkVariable<float>(NetworkVariableReadPermission.Everyone, 10f);

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
    }

    // Update is called once per frame
    void Update()
    {
        if (IsServer)
        {
            transform.Translate(Vector2.right * m_BulletSpeed * Time.deltaTime);

            m_BulletLifetime.Value -= Time.deltaTime;
            if (m_BulletLifetime.Value <= 0f)
            {
                DespawnBullet();
            }
        }
    }

    [ClientRpc]
    public void SetTrailColorClientRpc(Color color)
    {
        // need to make sure that we're updating the right player
        if (m_TrailGameObjectParticleSystem != null)
        {
            var main = m_TrailGameObjectParticleSystem.main;
            main.startColor = new ParticleSystem.MinMaxGradient(color);
        }
        else
        {
            Debug.Log("something is null!");
        }
    }
    
    [ClientRpc]
    public void PlayParticlesClientRpc()
    {
        if (m_TrailGameObjectParticleSystem != null)
        {
            m_TrailGameObjectParticleSystem.Play();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Only react to trigger on the server
        if (!IsServer)
            return;

        // check if the bullet has collided with an enemy
        var basicEnemy = other.gameObject.GetComponent<BasicEnemyBehavior>();
        if(basicEnemy != null)
        {
            var spaceshipController = m_BulletOwner.GetComponent<SpaceshipController>();
            if(spaceshipController != null)
            {
            }

            DespawnBullet();
        }
    }

    private void DespawnBullet()
    {
        if (NetworkObject.IsSpawned)
        {
            gameObject.SetActive(false);

            // Server tells clients that this object is no longer in play
            NetworkObject.Despawn();

            Destroy(gameObject);
        }
    }
}
