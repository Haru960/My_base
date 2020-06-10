using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    #region Variables
    [SerializeField]
    private Transform target;
    [SerializeField]
    private GameObject enemyPrefab;
    [SerializeField]
    private float spawnTime = 0.3f;
    #endregion
    void Awake()
    {
        StartCoroutine(SpawnEnemy());
    }

    IEnumerator SpawnEnemy()
    {
        while(true)
        {
            GameObject clone = Instantiate(enemyPrefab);

            float fx = Random.Range(-24, 24);
            float fz = Random.Range(-24, 24);
            clone.transform.position = new Vector3(fx, 0, fz);

            clone.GetComponent<EnemyController>().Initialize(target);

            yield return new WaitForSeconds(spawnTime);
        }
    }
}
