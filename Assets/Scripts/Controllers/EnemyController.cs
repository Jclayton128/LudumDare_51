using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EnemyController : MonoBehaviour
{
    GameController _gameController;
    TimeController _timeController;
    ExplosionController _explosionController;
    [SerializeField] GameObject[] _enemyPrefabs = null;
    [SerializeField] float _spawnPerimeterRadius = 5f;


    Dictionary<EnemyInfoHolder.EnemyType, GameObject> _enemyMenu 
        = new Dictionary<EnemyInfoHolder.EnemyType, GameObject>();

    Dictionary<EnemyInfoHolder.EnemyType, Queue<GameObject>> _enemyQueues =
        new Dictionary<EnemyInfoHolder.EnemyType, Queue<GameObject>>();

    [SerializeField] GameObject _guardPrefab;

    public Action<int> OnEnemyDead;

    int _enemyKillCount = 0;

    //settings
    //TODO make this steadily faster?
    [SerializeField] float _timeBetweenEnemySpawns = 2f;

    //state
    float _countdownUntilNextSpawn;
    [SerializeField] bool _isSpawning = false;

    private void Awake()
    {
        _gameController = GetComponent<GameController>();
        _gameController.OnPlayerStartsRun += HandleOnPlayerSpawn;

        _timeController = _gameController.GetComponent<TimeController>();
        _explosionController  =_gameController.GetComponent <ExplosionController>();
        
        CreateEnemyPools();
        CreateEnemyMenu();
    }

    private void CreateEnemyMenu()
    {
        foreach (var enemy in _enemyPrefabs)
        {
            EnemyInfoHolder.EnemyType eType = enemy.GetComponent<EnemyInfoHolder>().enemyType;

            if (_enemyMenu.ContainsKey(eType))
            {
                Debug.LogError($"Menu already contains a {eType} prefab");
                return;
            }

            _enemyMenu.Add(eType, enemy);
        }
    }

    private void CreateEnemyPools()
    {
        foreach (var enemy in _enemyPrefabs)
        {
            Queue<GameObject> queue = new Queue<GameObject>();
            EnemyInfoHolder.EnemyType eType = enemy.GetComponent<EnemyInfoHolder>().enemyType;
            
            if (_enemyQueues.ContainsKey(eType))
            {
                Debug.LogError($"Already have a pool for {eType}");
                return;
            }
            
            _enemyQueues.Add(eType, queue);
        }
    }

    public void HandleOnPlayerSpawn(GameObject newPlayer)
    {
        CreateGuardWall(34, 3f);
        CreateGuardWall(36, 3.25f);
    }

    private void CreateGuardWall(int count, float radiusMultiplier)
    {
        float degreesSpreadOfEntireBurst = 360f;
        int projectilesInBurst = count;

        float spreadSubdivided = degreesSpreadOfEntireBurst / projectilesInBurst;
        for (int i = 0; i < projectilesInBurst; i++)
        {
            Quaternion sector = Quaternion.Euler(0, 0, (i * spreadSubdivided) - (degreesSpreadOfEntireBurst / 2) + transform.eulerAngles.z);
            GameObject go = Instantiate(_guardPrefab);
            go.transform.rotation = sector;
            go.transform.position = _spawnPerimeterRadius * radiusMultiplier * go.transform.up;
        }
    }

    private void Update()
    {
        //SHIM DEBUG
        if (Input.GetKeyDown(KeyCode.Z)) SpawnEnemyOnSpawnPerimeter((EnemyInfoHolder.EnemyType)0, 1);
        if (Input.GetKeyDown(KeyCode.X)) SpawnEnemyOnSpawnPerimeter((EnemyInfoHolder.EnemyType)1, 1);
        if (Input.GetKeyDown(KeyCode.C)) SpawnEnemyOnSpawnPerimeter((EnemyInfoHolder.EnemyType)2, 1);
        if (Input.GetKeyDown(KeyCode.V)) SpawnEnemyOnSpawnPerimeter((EnemyInfoHolder.EnemyType)3, 1);
        if (Input.GetKeyDown(KeyCode.B)) SpawnEnemyOnSpawnPerimeter((EnemyInfoHolder.EnemyType)4, 1);
        if (Input.GetKeyDown(KeyCode.N)) SpawnEnemyOnSpawnPerimeter((EnemyInfoHolder.EnemyType)5, 1);

        if (Input.GetKeyDown(KeyCode.P)) _isSpawning = !_isSpawning;


        if (_isSpawning && _gameController.IsGameRunning )
        {
            _countdownUntilNextSpawn -= Time.deltaTime * _timeController.EnemyTimeScale;
            
            if (_countdownUntilNextSpawn <= 0)
            {
                
                SpawnEnemyOnSpawnPerimeter(GetRandomEnemyType(), 1);
                _countdownUntilNextSpawn = _timeBetweenEnemySpawns;
            }

        }
       
    }

    private EnemyInfoHolder.EnemyType GetRandomEnemyType()
    {
        if (_enemyPrefabs.Length == 0) return EnemyInfoHolder.EnemyType.Rat;
        int rand = UnityEngine.Random.Range(0, _enemyPrefabs.Length);
        return _enemyPrefabs[rand].GetComponent<EnemyInfoHolder>().enemyType;
    }

    private void SpawnEnemyOnSpawnPerimeter 
        (EnemyInfoHolder.EnemyType enemyType, int numberToSpawn)
    {
        for (int i = 0; i < numberToSpawn; i++)
        {
            GameObject enemy;
            if (_enemyQueues[enemyType].Count == 0)
            {
                GameObject menuExample = _enemyMenu[enemyType];
                if (menuExample == null)
                {
                    Debug.LogError($"{enemyType} isn't on the Menu!");
                    return;
                }
                enemy = Instantiate(menuExample);
                enemy.GetComponent<Entity>().Initialize(this, _explosionController);
            }
            else
            {
                enemy = _enemyQueues[enemyType].Dequeue ();
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
        _enemyKillCount++;
        OnEnemyDead?.Invoke(_enemyKillCount);
        EnemyInfoHolder.EnemyType eType = deadEnemy.GetComponent<EnemyInfoHolder>().enemyType;

        _enemyQueues[eType].Enqueue(deadEnemy);
        deadEnemy.SetActive(false);
    }

    private Vector2 FindPointOnUnitCircleCircumference()
    {
        float randomAngle = UnityEngine.Random.Range(0f, Mathf.PI * 2f);
        return new Vector2(Mathf.Sin(randomAngle), Mathf.Cos(randomAngle)).normalized;
    }

    public int GetKillCount()
    {
        return _enemyKillCount;
    }
}
