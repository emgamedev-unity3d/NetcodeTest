using UnityEngine;
using Unity.Netcode;

public class EnemySpawner : NetworkBehaviour
{
    [SerializeField]
    private GameObject enemyToSpawn;

    private NetworkVariable<float> enemySpawnTime =
        new NetworkVariable<float>(NetworkVariableReadPermission.Everyone, 5f);

    // Update is called once per frame
    void Update()
    {
        if (IsServer)
        {
            enemySpawnTime.Value -= Time.deltaTime;
            if (enemySpawnTime.Value <= 0f)
            {
                Vector3 position = new Vector3(transform.position.x, Random.Range(-5f, 5f), 0f);

                var newEnemy = Instantiate(enemyToSpawn, Vector3.zero, Quaternion.identity);
                newEnemy.GetComponent<NetworkObject>().Spawn();

                newEnemy.transform.position = position;

                enemySpawnTime.Value = 5f;
            }
        }
    }
}