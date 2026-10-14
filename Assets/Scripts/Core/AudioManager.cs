using System.Collections.Generic;
using UnityEngine;

namespace StickmanParty.Core
{
    public enum SoundType
    {
        ButtonClick,
        CountdownBeep,
        CountdownGo,
        Punch,
        Dash,
        TankShoot,
        Explosion,
        Whistle,
        GoalCheer,
        StarWin,
        BombTick,
        TilePaint
    }

    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        private AudioSource sfxSource;
        private AudioSource musicSource;
        private Dictionary<SoundType, AudioClip> proceduralClips = new Dictionary<SoundType, AudioClip>();

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            sfxSource = gameObject.AddComponent<AudioSource>();
            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.loop = true;

            GenerateProceduralSounds();
        }

        private void GenerateProceduralSounds()
        {
            proceduralClips[SoundType.ButtonClick] = CreateToneClip(800f, 0.05f, 0.3f);
            proceduralClips[SoundType.CountdownBeep] = CreateToneClip(440f, 0.15f, 0.5f);
            proceduralClips[SoundType.CountdownGo] = CreateToneClip(880f, 0.4f, 0.6f);
            proceduralClips[SoundType.Punch] = CreateThudClip(120f, 0.12f);
            proceduralClips[SoundType.Dash] = CreateNoiseSweepClip(false, 0.18f);
            proceduralClips[SoundType.TankShoot] = CreateTankShotClip();
            proceduralClips[SoundType.Explosion] = CreateExplosionClip();
            proceduralClips[SoundType.Whistle] = CreateWhistleClip();
            proceduralClips[SoundType.GoalCheer] = CreateCheerClip();
            proceduralClips[SoundType.StarWin] = CreateFanfareClip();
            proceduralClips[SoundType.BombTick] = CreateToneClip(1200f, 0.04f, 0.4f);
            proceduralClips[SoundType.TilePaint] = CreateToneClip(523.25f, 0.06f, 0.25f);
        }

        public void PlaySound(SoundType type, float volume = 1f)
        {
            if (proceduralClips.TryGetValue(type, out var clip))
            {
                sfxSource.PlayOneShot(clip, volume);
            }
        }

        #region Procedural Audio Generators
        private AudioClip CreateToneClip(float frequency, float duration, float volume = 0.5f)
        {
            int sampleRate = 44100;
            int samplesCount = (int)(sampleRate * duration);
            float[] samples = new float[samplesCount];

            for (int i = 0; i < samplesCount; i++)
            {
                float t = (float)i / sampleRate;
                float envelope = 1f - ((float)i / samplesCount); // linear fade out
                samples[i] = Mathf.Sin(2 * Mathf.PI * frequency * t) * envelope * volume;
            }

            AudioClip clip = AudioClip.Create("Tone_" + frequency, samplesCount, 1, sampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }

        private AudioClip CreateThudClip(float baseFreq, float duration)
        {
            int sampleRate = 44100;
            int samplesCount = (int)(sampleRate * duration);
            float[] samples = new float[samplesCount];

            for (int i = 0; i < samplesCount; i++)
            {
                float t = (float)i / sampleRate;
                float env = Mathf.Exp(-t * 25f);
                float freq = Mathf.Lerp(baseFreq, 30f, (float)i / samplesCount);
                float sine = Mathf.Sin(2 * Mathf.PI * freq * t);
                float noise = (Random.value * 2f - 1f) * 0.3f;
                samples[i] = (sine + noise) * env * 0.7f;
            }

            AudioClip clip = AudioClip.Create("Thud", samplesCount, 1, sampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }

        private AudioClip CreateNoiseSweepClip(bool pitchUp, float duration)
        {
            int sampleRate = 44100;
            int samplesCount = (int)(sampleRate * duration);
            float[] samples = new float[samplesCount];

            for (int i = 0; i < samplesCount; i++)
            {
                float progress = (float)i / samplesCount;
                float env = Mathf.Sin(progress * Mathf.PI);
                float noise = (Random.value * 2f - 1f);
                samples[i] = noise * env * 0.4f;
            }

            AudioClip clip = AudioClip.Create("NoiseSweep", samplesCount, 1, sampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }

        private AudioClip CreateTankShotClip()
        {
            int sampleRate = 44100;
            float duration = 0.25f;
            int samplesCount = (int)(sampleRate * duration);
            float[] samples = new float[samplesCount];

            for (int i = 0; i < samplesCount; i++)
            {
                float t = (float)i / sampleRate;
                float env = Mathf.Exp(-t * 18f);
                float freq = Mathf.Lerp(300f, 40f, (float)i / samplesCount);
                float sine = Mathf.Sin(2 * Mathf.PI * freq * t);
                float noise = (Random.value * 2f - 1f) * 0.6f;
                samples[i] = (sine + noise) * env * 0.8f;
            }

            AudioClip clip = AudioClip.Create("TankShot", samplesCount, 1, sampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }

        private AudioClip CreateExplosionClip()
        {
            int sampleRate = 44100;
            float duration = 0.6f;
            int samplesCount = (int)(sampleRate * duration);
            float[] samples = new float[samplesCount];

            for (int i = 0; i < samplesCount; i++)
            {
                float t = (float)i / sampleRate;
                float env = Mathf.Exp(-t * 7f);
                float noise = (Random.value * 2f - 1f);
                float rumble = Mathf.Sin(2 * Mathf.PI * 55f * t) * 0.5f;
                samples[i] = (noise + rumble) * env * 0.9f;
            }

            AudioClip clip = AudioClip.Create("Explosion", samplesCount, 1, sampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }

        private AudioClip CreateWhistleClip()
        {
            int sampleRate = 44100;
            float duration = 0.45f;
            int samplesCount = (int)(sampleRate * duration);
            float[] samples = new float[samplesCount];

            for (int i = 0; i < samplesCount; i++)
            {
                float t = (float)i / sampleRate;
                float env = Mathf.Sin(((float)i / samplesCount) * Mathf.PI);
                float f1 = Mathf.Sin(2 * Mathf.PI * 2400f * t);
                float f2 = Mathf.Sin(2 * Mathf.PI * 2425f * t);
                samples[i] = (f1 + f2) * 0.5f * env * 0.6f;
            }

            AudioClip clip = AudioClip.Create("Whistle", samplesCount, 1, sampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }

        private AudioClip CreateCheerClip()
        {
            int sampleRate = 44100;
            float duration = 0.8f;
            int samplesCount = (int)(sampleRate * duration);
            float[] samples = new float[samplesCount];

            for (int i = 0; i < samplesCount; i++)
            {
                float t = (float)i / sampleRate;
                float env = Mathf.Sin(((float)i / samplesCount) * Mathf.PI);
                float c1 = Mathf.Sin(2 * Mathf.PI * 523.25f * t); // C5
                float e1 = Mathf.Sin(2 * Mathf.PI * 659.25f * t); // E5
                float g1 = Mathf.Sin(2 * Mathf.PI * 783.99f * t); // G5
                samples[i] = ((c1 + e1 + g1) / 3f) * env * 0.6f;
            }

            AudioClip clip = AudioClip.Create("Cheer", samplesCount, 1, sampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }

        private AudioClip CreateFanfareClip()
        {
            int sampleRate = 44100;
            float duration = 1.0f;
            int samplesCount = (int)(sampleRate * duration);
            float[] samples = new float[samplesCount];

            for (int i = 0; i < samplesCount; i++)
            {
                float t = (float)i / sampleRate;
                float env = Mathf.Clamp01((duration - t) / duration);
                float note = 440f;
                if (t > 0.25f) note = 554.37f;
                if (t > 0.5f) note = 659.25f;
                if (t > 0.75f) note = 880f;

                samples[i] = Mathf.Sin(2 * Mathf.PI * note * t) * env * 0.5f;
            }

            AudioClip clip = AudioClip.Create("Fanfare", samplesCount, 1, sampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }
        #endregion
    }
}
