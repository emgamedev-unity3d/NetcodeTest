using UnityEngine;
using Unity.Netcode;

public class ShipBulletBehavior : NetworkBehaviour
{
    public float bulletSpeed = 10f;

    public float rotateSpeed = 90f;

    public GameObject bulletOwner = null;

    [SerializeField]
    private SpriteRenderer m_spriteRenderer;

    [SerializeField]
    private ParticleSystem trailGameObject = null;

    private NetworkVariable<float> bulletLifetime = 
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
            transform.Translate(Vector2.right * bulletSpeed * Time.deltaTime);

            //float rotation = transform.rotation.eulerAngles.z;
            //rotation += rotateSpeed * Time.deltaTime;

            //transform.localRotation = Quaternion.Euler(0f, 0f, rotation);
        }

        if (IsServer)
        {
            bulletLifetime.Value -= Time.deltaTime;
            if (bulletLifetime.Value <= 0f)
            {
                DespawnBullet();
            }
        }
    }

    [ClientRpc]
    public void SetTrailColorClientRpc(Color color)
    {
        // need to make sure that we're updating the right player
        if (trailGameObject != null)
        {
            var main = trailGameObject.main;
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
        if (trailGameObject != null)
        {
            trailGameObject.Play();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Only react to trigger on the server
        if (!IsServer)
            return;

        var basicEnemy = other.gameObject.GetComponent<BasicEnemyBehavior>();
        if(basicEnemy != null)
        {
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
