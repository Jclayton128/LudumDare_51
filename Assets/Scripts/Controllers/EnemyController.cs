using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    GameController _gameController;
    TimeController _timeController;
    ExplosionController _explosionController;
    [SerializeField] GameObject _enemyPrefab = null;
    [SerializeField] float _spawnPerimeterRadius = 5f;

    List<GameObject> _activeEnemies = new List<GameObject>();
    Queue<GameObject> _pooledEnemies = new Queue<GameObject> ();

    //settings
    float _timeBetweenEnemySpawns = 2f;

    //state
    float _countdownUntilNextSpawn;

    private void Awake()
    {
        _gameController = GetComponent<GameController>();
        _gameController.OnPlayerStartsRun += HandleOnPlayerSpawn;

        _timeController = _gameController.GetComponent<TimeController>();
        _explosionController  =_gameController.GetComponent <ExplosionController>();
    }

    public void HandleOnPlayerSpawn(GameObject newPlayer)
    {
        SpawnEnemyOnSpawnPerimeter(2);
    }

    private void Update()
    {
        //SHIM DEBUG
        if (Input.GetKeyDown(KeyCode.B)) SpawnEnemyOnSpawnPerimeter(1);

        if (_gameController.IsGameRunning )
        {
            _countdownUntilNextSpawn -= Time.deltaTime * _timeController.EnemyTimeScale;
            
            if (_countdownUntilNextSpawn <= 0)
            {
                SpawnEnemyOnSpawnPerimeter(1);
                _countdownUntilNextSpawn = _timeBetweenEnemySpawns;
            }

        }
       
    }

    private void SpawnEnemyOnSpawnPerimeter (int numberToSpawn)
    {
        for (int i = 0; i < numberToSpawn; i++)
        {
            GameObject enemy;
            if (_pooledEnemies.Count == 0)
            {
                enemy = Instantiate(_enemyPrefab);
                enemy.GetComponent<Entity>().Initialize(this, _explosionController);
            }
            else
            {
                enemy = _pooledEnemies.Dequeue ();
                enemy.SetActive(true);
            }
            enemy.GetComponent<Entity>().SetUpForUse();
            Vector2 pos = (Vector2)_gameController.CurrentPlayer.transform.position +
                (FindPointOnUnitCircleCircumference() * _spawnPerimeterRadius);
            enemy.transform.position = pos;
            
        }
    }

    public void ReturnDeadEnemy(GameObject deadEnemy)
    {
        _activeEnemies.Remove(deadEnemy);
        _pooledEnemies.Enqueue(deadEnemy);
        deadEnemy.SetActive(false);
    }

    private Vector2 FindPointOnUnitCircleCircumference()
    {
        float randomAngle = Random.Range(0f, Mathf.PI * 2f);
        return new Vector2(Mathf.Sin(randomAngle), Mathf.Cos(randomAngle)).normalized;
    }
}
