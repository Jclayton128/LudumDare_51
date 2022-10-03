using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using FMODUnity;
using UnityEngine;

public class StatsHandler : MonoBehaviour {
    ExplosionController _explosionController;

    public Action<int> OnChangeShieldLayerCount;

    /// <summary>
    /// First argument is the subsystem (0-3), second argument is that system's 
    /// current damage level. Third argument is whether this event is called due to
    /// damage being repaire (to determine whether to emit RepairParticleFX)
    /// </summary>
    public Action<int, float, bool> OnReceiveDamage;


    public Action OnPlayerDying;
    public Action<float> OnShieldChargeLevelChange;

    //Scene References
    TimeController _timeController;

    //settings
    [SerializeField] float _cameraZoom_Normal = 6f;

    [SerializeField] float _moveSpeed_Normal = 10f;
    [SerializeField] float _rotationSpeed_Normal = 360f;
    [SerializeField] float _shieldRegenRate_Normal = 0.3f;
    [SerializeField] float _healRate_Normal = 0.3f;
    [SerializeField] StudioEventEmitter repairSound;
    [SerializeField] StudioEventEmitter repairCompleteSound;
    [SerializeField] StudioEventEmitter playerShieldHitSound;
    [SerializeField] StudioEventEmitter playerHurtSound;
    [SerializeField] StudioEventEmitter playerDeathSound;
    [SerializeField] CinemachineImpulseSource screenShakeOnDamage;
    [SerializeField] CinemachineImpulseSource screenShakeOnDeath;
    const float _requiredRegenLevelPerShield = 1f;
    const int _shieldLayers_Max = 3;

    const int _numberOfSubsystems = 4;
    const int _maxDamagePossible = 4;
    //Seconds between shots - lower is better;
    [SerializeField] float _fireRate_Normal = 0.25f;
    [SerializeField] float _fireRate_Fast = 0.1f;

    bool hasPerfectHealthThisRound = true;

    //state

    //0 is full, 4 is dead.
    [SerializeField] float[] _damageLevelsBySubsystem = new float[4] { 0f, 0f, 0f, 0f };

    //These modifiers are multiplied against appropriate system.
    //They are changed as the ship received/repairs damage
    [SerializeField] float[] _statModifiersBySubsystem = new float[4] { 1f, 1f, 1f, 1f };

    //float _moveSpeed_Current;
    //float _rotationSpeed_Current;
    //float _shieldRegenRate_Current;
    [SerializeField] float _shieldChargeLevel_Current;
    int _shieldLayers_Current;
    float _fireRate_Current;
    bool isAlive = true;

    public bool IsAlive => isAlive;

    public float CameraZoom { get => isAlive ? _cameraZoom_Normal * _statModifiersBySubsystem[0] : _cameraZoom_Normal; }

    public float ShieldRegenRate { get => _shieldRegenRate_Normal * _statModifiersBySubsystem[3]; }

    public float MoveSpeed { get => _moveSpeed_Normal * _statModifiersBySubsystem[2]; }

    public float FireRate { get => _fireRate_Current * _statModifiersBySubsystem[1]; }  

    public float RotationSpeed { get => _rotationSpeed_Normal; }

    private void OnEnable() {
        _timeController.OnNewPhase += HandleNewPhase;
    }

    private void OnDisable() {
        _timeController.OnNewPhase -= HandleNewPhase;
    }

    void HandleNewPhase(TimeController.Phase incomingPhase) {
        if (incomingPhase == TimeController.Phase.A_mobility) {
            hasPerfectHealthThisRound = GetHasPerfectHealth();
        }
    }

    bool GetHasPerfectHealth() {
        for (int i = 0; i < _damageLevelsBySubsystem.Length; i++) {
            if (_damageLevelsBySubsystem[i] > 0) return false;
        }
        return true;
    }

    private void Awake() {
        _timeController = FindObjectOfType<TimeController>();
        _explosionController = _timeController.GetComponent<ExplosionController>();
        AppIntegrity.Assert(repairSound != null);
        AppIntegrity.Assert(repairCompleteSound != null);
        AppIntegrity.Assert(playerShieldHitSound != null);
        AppIntegrity.Assert(playerHurtSound != null);
        AppIntegrity.Assert(playerDeathSound != null);
        AppIntegrity.Assert(screenShakeOnDamage != null);
        AppIntegrity.Assert(screenShakeOnDeath != null);
    }

    private void Start() {
        this.OnReceiveDamage += ConvertDamageLevelsIntoStatChanges;

        _shieldChargeLevel_Current = 0;
        _shieldLayers_Current = _shieldLayers_Max;

        OnChangeShieldLayerCount?.Invoke(_shieldLayers_Current);
        AppIntegrity.Assert(_numberOfSubsystems == _damageLevelsBySubsystem.Length, "_damageLevelsBySubsystem.Length does not match _numberOfSubsystems!!!");
    }

    #region Flow over time
    private void Update() {
        RegenerateShield();
        _fireRate_Current = _timeController.IsPhaseB ? _fireRate_Fast : _fireRate_Normal;
    }

    private void RegenerateShield() {
        if (_shieldChargeLevel_Current >= _requiredRegenLevelPerShield) {
            _shieldChargeLevel_Current = 0;
            OnShieldChargeLevelChange?.Invoke(_shieldChargeLevel_Current);
            _shieldLayers_Current++;
            OnChangeShieldLayerCount?.Invoke(_shieldLayers_Current);
        }

        if (_shieldLayers_Current >= _shieldLayers_Max)
        {
            OnShieldChargeLevelChange?.Invoke(1);
            return;
        }
        else {
            _shieldChargeLevel_Current +=
                ShieldRegenRate * Time.deltaTime * _timeController.PlayerTimeScale;
            OnShieldChargeLevelChange?.Invoke(_shieldChargeLevel_Current);
        }
    }

    #endregion

    #region Receive Incoming Damage

    private void OnTriggerEnter2D(Collider2D collision) {
        if (!isAlive) return;
        ReceiveBulletImpact();
    }

    public void ReceiveExplosiveImpact(float damage) {
        if (!isAlive) return;
        float appliedAmout = damage - _shieldLayers_Current;
        _shieldLayers_Current = Mathf.Max(0, _shieldLayers_Current - Mathf.RoundToInt(damage));
        OnChangeShieldLayerCount?.Invoke(_shieldLayers_Current);
        while (appliedAmout > 0) {
            ReceiveDamage();
            appliedAmout--;
        }
    }

    public void ReceiveBulletImpact() {
        if (!isAlive) return;
        if (_shieldLayers_Current > 0) {
            _shieldLayers_Current--;
            OnChangeShieldLayerCount?.Invoke(_shieldLayers_Current);
            _explosionController.RequestExplosion(2, transform.position, Color.green);
            playerShieldHitSound.Play();
        } else {
            ReceiveDamage();
            playerHurtSound.Play();
        }
    }

    private void ReceiveDamage() {
        if (!isAlive) return;
        hasPerfectHealthThisRound = false;
        AppIntegrity.Assert(_damageLevelsBySubsystem.Length != 0, "_damageLevelsBySubsystem is empty!");
        _explosionController.RequestExplosion(2, transform.position, Color.green);
        int damagedSubsystem = UnityEngine.Random.Range(0, _numberOfSubsystems);
        _damageLevelsBySubsystem[damagedSubsystem] += 1f;
        OnReceiveDamage?.Invoke(
            damagedSubsystem, _damageLevelsBySubsystem[damagedSubsystem], false);
        StartCoroutine(ScreenShakeOnDamage(UnityEngine.Random.Range(.3f, 1f)));
        CheckForDeath();
    }

    public void ReceivedTargetedBulletImpact(int targetedSystem) {
        if (!isAlive) return;
        hasPerfectHealthThisRound = false;
        if (_shieldLayers_Current > 0) {
            _shieldLayers_Current--;
            OnChangeShieldLayerCount?.Invoke(_shieldLayers_Current);
            _explosionController.RequestExplosion(2, transform.position, Color.green);
            playerShieldHitSound.Play();
        } else {
            playerHurtSound.Play();
            ReceiveTargetedDamage(targetedSystem);
        }
    }

    /// <summary>
    /// This is a Debug tool. Gameplay damage should be random.
    /// </summary>
    /// <param name="targetedSystem"></param>
    private void ReceiveTargetedDamage(int targetedSystem) {
        AppIntegrity.Assert(_damageLevelsBySubsystem.Length != 0, "_damageLevelsBySubsystem is empty!");
        _explosionController.RequestExplosion(2, transform.position, Color.green);

        _damageLevelsBySubsystem[targetedSystem] += 1f;
        OnReceiveDamage?.Invoke(
            targetedSystem, _damageLevelsBySubsystem[targetedSystem], false);
        StartCoroutine(ScreenShakeOnDamage(UnityEngine.Random.Range(.3f, 1f)));
        CheckForDeath();
    }


    private void CheckForDeath() {
        for (int i = 0; i < _damageLevelsBySubsystem.Length; i++) {
            if (_damageLevelsBySubsystem[i] >= _maxDamagePossible) {

                ExecuteDeathSequence();
            }
        }
    }

    private void ExecuteDeathSequence() {
        isAlive = false;
        playerDeathSound.Play();
        _explosionController.RequestExplosion(20, transform.position, Color.green);
        OnPlayerDying?.Invoke();
        StartCoroutine(ScreenShakeOnDeath());
        HideSprite();
        Destroy(gameObject, 3f);

    }

    void HideSprite() {
        SpriteRenderer[] spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        if (spriteRenderers == null) return;
        
        foreach (SpriteRenderer sr in spriteRenderers)
        {
            sr.enabled = false;
        }

    }

    #endregion

    #region Repair damage

    public void RepairDamage(int subsystemIndex) {
        if (_timeController.CurrentPhase != TimeController.Phase.C_healing) return;
        AppIntegrity.Assert(subsystemIndex < _damageLevelsBySubsystem.Length, $"RepairDamage tried to repair out-of-bounds subsystem with index: {subsystemIndex}");
        float previousDamageLevel = _damageLevelsBySubsystem[subsystemIndex];
        _damageLevelsBySubsystem[subsystemIndex] -= _healRate_Normal * Time.unscaledDeltaTime;
        _damageLevelsBySubsystem[subsystemIndex] = Mathf.Clamp(_damageLevelsBySubsystem[subsystemIndex], hasPerfectHealthThisRound ? -1 : 0, _maxDamagePossible);
        float currentDamageLevel = _damageLevelsBySubsystem[subsystemIndex];

        if (currentDamageLevel < previousDamageLevel) {
            PlayRepairSound();
            OnReceiveDamage?.Invoke(subsystemIndex, currentDamageLevel, true);
            if (currentDamageLevel == 0 || currentDamageLevel == -1) OnRepairComplete();
        }
    }

    void PlayRepairSound() {
        if (!repairSound.IsPlaying()) repairSound.Play();
        if (cStopRepairSound != null) StopCoroutine(cStopRepairSound);
        // we want to stop playing the repair sound as soon as RepairDamage stops being called.
        cStopRepairSound = StartCoroutine(StopRepairSound());
    }

    void OnRepairComplete() {
        if (cStopRepairSound != null) StopCoroutine(cStopRepairSound);
        repairSound.Stop();
        repairCompleteSound.Play();
    }

    Coroutine cStopRepairSound;
    IEnumerator StopRepairSound() {
        yield return new WaitForSeconds(0.1f);
        repairSound.Stop();
    }

    #endregion

    private void ConvertDamageLevelsIntoStatChanges(
        int subsystem, float damageLevel, bool ignored) {

        float normalizedDamage = Mathf.InverseLerp(-1, _maxDamagePossible, damageLevel);

        switch (subsystem) {
            case 0:
                //Camera zooms in with more damage
                _statModifiersBySubsystem[0] = Mathf.Lerp(1.2f, 0.7f, normalizedDamage);
                break;

            case 1:
                // Time between shots increases with more damage
                _statModifiersBySubsystem[1] = Mathf.Lerp(.75f, 4f, normalizedDamage);
               
                break;

            case 2:
                // Translate speed drops with more damage
                _statModifiersBySubsystem[2] = Mathf.Lerp(1.33f, 0.5f, normalizedDamage);
                break;

            case 3:
                // Shield regen rate drops with more damage
                _statModifiersBySubsystem[3] = Mathf.Lerp(1.33f, 0.25f, normalizedDamage);
                break;

        }
    }

    #region SCREEN_SHAKE

    IEnumerator ScreenShakeOnDamage(float damage) {
        screenShakeOnDamage.GenerateImpulse(UnityEngine.Random.insideUnitCircle.normalized * damage * 0.1f);
        yield return new WaitForSecondsRealtime(0.1f);
        screenShakeOnDamage.GenerateImpulse(UnityEngine.Random.insideUnitCircle.normalized * damage * 0.1f);
    }

    IEnumerator ScreenShakeOnDeath() {
        float amount = UnityEngine.Random.Range(.4f, .6f);
        screenShakeOnDeath.GenerateImpulse(Vector3.right * amount);
        yield return new WaitForSecondsRealtime(0.1f);
        amount = UnityEngine.Random.Range(.4f, .6f);
        screenShakeOnDeath.GenerateImpulse(Vector3.up * amount);
    }

    #endregion

}
