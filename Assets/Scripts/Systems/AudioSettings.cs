using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class AudioSettings : MonoBehaviour {
    // [Range(0f,10f)]public float volume = 7f;

    [SerializeField] StudioEventEmitter testVolumeSound;

    const string MASTER_BUS = "bus:/Master";
    const string MUSIC_BUS = "bus:/Master/Music";
    const string SFX_BUS = "bus:/Master/SFX";

    Bus masterBus;
    Bus musicBus;
    Bus sfxBus;

    public void SetMasterVolume(float volume) {
        SetChannelVolume(masterBus, volume);
    }

    public void SetMusicVolume(float volume) {
        SetChannelVolume(musicBus, volume);
    }

    public void SetSfxVolume(float volume) {
        SetChannelVolume(sfxBus, volume);
    }

    public void TestSfxVolume() {
        if (testVolumeSound != null) testVolumeSound.Play();
    }

    void Start() {
        masterBus = RuntimeManager.GetBus(MASTER_BUS);
        musicBus = RuntimeManager.GetBus(MUSIC_BUS);
        sfxBus = RuntimeManager.GetBus(SFX_BUS);
    }

    void SetChannelVolume(Bus bus, float volume) {
        // 1.0 = fader initial position, 0.0 = -Infinity
        bus.setVolume(volume);
    }
}