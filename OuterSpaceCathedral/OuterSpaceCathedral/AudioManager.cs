using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace OuterSpaceCathedral
{
    internal static class AudioManager
    {
        private static SoundEffect  playerDeathSFX;
        private static SoundEffect  enemyDeathSFX;
        private static SoundEffect  playerJoinSFX;
        private static SoundEffect  playerFireSFX;
        private static SoundEffect  cursorMoveSFX;
        private static SoundEffect  cursorSelectSFX;
        private static SoundEffect  levelUnlockedSFX;
        private static SoundEffect  bebopColaSFX;
        private static SoundEffect  countdownSFX;
        private static SoundEffect  fireworkSFX;

        private static SoundEffectInstance enemyDeathSFXInstance;
        private static SoundEffectInstance playerFireSFXInstance;

        private static Song max300, outerSpace, ocean, city, newYear, bebopBoss;

        public static void Initialize(ContentManager content)
        {
            playerDeathSFX      = content.Load<SoundEffect>("sfx\\playerDeath");
            enemyDeathSFX       = content.Load<SoundEffect>("sfx\\enemyDeath");
            playerJoinSFX       = content.Load<SoundEffect>("sfx\\playerJoin");
            playerFireSFX       = content.Load<SoundEffect>("sfx\\playerFireLoop");
            cursorMoveSFX       = content.Load<SoundEffect>("sfx\\cursorMove");
            cursorSelectSFX     = content.Load<SoundEffect>("sfx\\cursorSelect");
            levelUnlockedSFX    = content.Load<SoundEffect>("sfx\\levelUnlock");
            bebopColaSFX        = content.Load<SoundEffect>("sfx\\bebop");
            countdownSFX        = content.Load<SoundEffect>("sfx\\countdown");
            fireworkSFX         = content.Load<SoundEffect>("sfx\\fireworkPop");

            max300              = content.Load<Song>("songs\\Max300");
            outerSpace          = content.Load<Song>("songs\\OuterSpace");
            ocean               = content.Load<Song>("songs\\RipTide");
            city                = content.Load<Song>("songs\\City");
            newYear             = content.Load<Song>("songs\\NewYear");
            bebopBoss           = content.Load<Song>("songs\\BebopBoss");

            enemyDeathSFXInstance = enemyDeathSFX.CreateInstance();
            playerFireSFXInstance = playerFireSFX.CreateInstance();
            playerFireSFXInstance.IsLooped = true;
        }

        public static void PlayCursorMoveSFX()
        {
            cursorMoveSFX.Play();
        }

        public static void PlayCursorSelectSFX()
        {
            cursorSelectSFX.Play();
        }

        public static void PlayPlayerDeathSFX()
        {
            playerDeathSFX.Play();
        }

        public static void PlayPlayerJoinSFX()
        {
            playerJoinSFX.Play();
        }

        public static void PlayEnemyDeathSFX()
        {
            enemyDeathSFXInstance.Stop();
            enemyDeathSFXInstance.Play();
        }

        public static void PlayLevelUnlockedSFX()
        {   
            levelUnlockedSFX.Play();
        }

        public static void PlayBebopSFX()
        {   
            bebopColaSFX.Play();
        }

        public static void PlayCountdownSFX()
        {   
            countdownSFX.Play();
        }

        public static void PlayFireworkPopSFX()
        {   
            fireworkSFX.Play();
        }

        public static void Update(float deltaTime)
        {
            if (GameState.Level != null)
            {
                if (GameState.Level.IsPlayerFiring())
                {
                    if (playerFireSFXInstance.State != SoundState.Playing)
                    {
                        StartPlayerFireSFX();
                    }
                }
                else
                {
                    if (playerFireSFXInstance.State == SoundState.Playing)
                    {
                        StopPlayerFireSFX();
                    }
                }
            }
        }

        public static void StartPlayerFireSFX()
        {
            playerFireSFXInstance.Play();
        }

        public static void StopPlayerFireSFX()
        {
            playerFireSFXInstance.Stop(false);
        }

        private static void PlaySongAndMaxVolume(Song song)
        {
            MediaPlayer.Play(song);
            MediaPlayer.Volume = 1.0f;
        }

        public static void SetSongFade(float fadePercent)
        {
            MediaPlayer.Volume = fadePercent;
        }

        public static void PlayMaxSong()
        {   
            PlaySongAndMaxVolume(max300);
        }

        public static void PlaySpaceSong()
        {
            PlaySongAndMaxVolume(outerSpace);
        }

        public static void PlayOceanSong()
        {
            PlaySongAndMaxVolume(ocean);
        }

        public static void PlayCitySong()
        {
            PlaySongAndMaxVolume(city);
        }

        public static void PlayNewYearSong()
        {
            PlaySongAndMaxVolume(newYear);
        }

        public static void PlayBebopBossSong()
        {   
            PlaySongAndMaxVolume(bebopBoss);
        }

        public static void StopAllMusic()
        {
            MediaPlayer.Stop();
        }
    }
}
