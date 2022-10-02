using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatsHandler : MonoBehaviour
{
    public Action<int> OnChangeShieldLayerCount;

    /// <summary>
    /// First argument is the subsystem (0-3), second argument is that system's 
    /// current damage level. Third argument is whether this event is called due to
    /// damage being repaire (to determine whether to emit RepairParticleFX)
    /// </summary>
    public Action<int, float, bool> OnReceiveDamage;

    /// <summary>
    /// Argument is the subsystem (0-3)
    /// </summary>
    public Action<int> OnBeginRepairsToSystem;
    public Action<int> OnEndRepairsToSystem;
    public Action OnPlayerDying;

    //Scene References
    TimeController _timeController;

    //settings
    [SerializeField] float _moveSpeed_Normal = 10f;
    [SerializeField] float _rotationSpeed_Normal = 360f;

    [SerializeField] float _shieldRegenRate_Normal = 0.3f;
    [SerializeField] float _healRate_Normal = 0.3f;
    const float _requiredRegenLevelPerShield = 1f;
    const int _shieldLayers_Max = 3;

    const int _numberOfSubsystems = 4;
    const int _maxDamagePossible = 4;
    //Seconds between shots - lower is better;
    [SerializeField] float _fireRate_Normal = 0.25f;

    //state

    //0 is full, 4 is dead.
    [SerializeField] float[] _damageLevelsBySubsystem = new float[4] { 0f, 0f, 0f, 0f };

    float _moveSpeed_Current;
    float _rotationSpeed_Current;
    float _shieldRegenRate_Current;
    [SerializeField] float _shieldChargeLevel_Current;
    int _shieldLayers_Current;
    float _fireRate_Current;

    public float MoveSpeed { get => _moveSpeed_Current; }
    public float RotationSpeed { get => _rotationSpeed_Current; }
    public float FireRate { get => _fireRate_Current; }

    private void Awake()
    {
        _timeController = FindObjectOfType<TimeController>();
    }

    private void Start()
    {
        _moveSpeed_Current = _moveSpeed_Normal;
        _rotationSpeed_Current = _rotationSpeed_Normal;
        _shieldRegenRate_Current = _shieldRegenRate_Normal;
        _shieldChargeLevel_Current = 0;
        _shieldLayers_Current = _shieldLayers_Max;
        _fireRate_Current = _fireRate_Normal;

        OnChangeShieldLayerCount?.Invoke(_shieldLayers_Current);
        AppIntegrity.Assert(_numberOfSubsystems == _damageLevelsBySubsystem.Length, "_damageLevelsBySubsystem.Length does not match _numberOfSubsystems!!!");
    }

    #region Flow over time
    private void Update()
    {
        RegenerateShield();
    }

    private void RegenerateShield()
    {
        if (_shieldChargeLevel_Current >= _requiredRegenLevelPerShield)
        {
            _shieldChargeLevel_Current = 0;
            _shieldLayers_Current++;
            OnChangeShieldLayerCount?.Invoke(_shieldLayers_Current);
        }

        if (_shieldLayers_Current >= _shieldLayers_Max) return;
        else
        {
            _shieldChargeLevel_Current +=
                _shieldRegenRate_Current * Time.deltaTime * _timeController.PlayerTimeScale;
        }

    }

    #endregion

    #region Receive Incoming Damage

    public void ReceiveImpact()
    {
        Debug.Log("receiving impact");
        if (_shieldLayers_Current > 0)
        {
            _shieldLayers_Current--;
            OnChangeShieldLayerCount?.Invoke(_shieldLayers_Current);
        }
        else
        {
            ReceiveDamage();
        }
    }

    private void ReceiveDamage()
    {
        AppIntegrity.Assert(_damageLevelsBySubsystem.Length != 0, "_damageLevelsBySubsystem is empty!");
        int damagedSubsystem = UnityEngine.Random.Range(0, _numberOfSubsystems);
        _damageLevelsBySubsystem[damagedSubsystem]++;
        OnReceiveDamage?.Invoke(
            damagedSubsystem, _damageLevelsBySubsystem[damagedSubsystem], false);
        CheckForDeath();
    }

    private void CheckForDeath()
    {
        for (int i = 0; i < _damageLevelsBySubsystem.Length; i++)
        {
            if (_damageLevelsBySubsystem[i] >= _maxDamagePossible)
            {
                Debug.LogError("Player subsystem reached 4 hits - game over!");

                ExecuteDeathSequence();
            }
        }
    }

    private void ExecuteDeathSequence()
    {
        OnPlayerDying?.Invoke();

        Destroy(gameObject);
    }

    #endregion

    #region Repair damage

    public void RepairDamage(int subsystemIndex)
    {
        if (_timeController.CurrentPhase != TimeController.Phase.C_healing) return;
        AppIntegrity.Assert(subsystemIndex < _damageLevelsBySubsystem.Length, $"RepairDamage tried to repair out-of-bounds subsystem with index: {subsystemIndex}");
        _damageLevelsBySubsystem[subsystemIndex] -= _healRate_Normal * Time.deltaTime;
        _damageLevelsBySubsystem[subsystemIndex] = Mathf.Clamp(_damageLevelsBySubsystem[subsystemIndex], 0, _maxDamagePossible);
        OnReceiveDamage?.Invoke(subsystemIndex, _damageLevelsBySubsystem[subsystemIndex], true);
        
        //Might not need this if HealthUIDriver is emitting particles on that system's
        //particleFX each frame OnDamageReceived is called
        //StartCoroutine(RepairingFX());
    }

    bool isPlayingRepairFX = false;
    IEnumerator RepairingFX()
    {
        if (isPlayingRepairFX) yield break;
        isPlayingRepairFX = true;

        // TODO: play particle FX or something

        yield return null;
        isPlayingRepairFX = false;
    }

    #endregion
}
