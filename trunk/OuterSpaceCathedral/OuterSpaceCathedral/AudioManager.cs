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

        private static SoundEffectInstance enemyDeathSFXInstance;
        private static SoundEffectInstance playerFireSFXInstance;

        private static Song max300;

        private static bool playingPlayerFireSFX;

        public static void Initialize(ContentManager content)
        {
            playerDeathSFX      = content.Load<SoundEffect>("sfx\\playerDeath");
            enemyDeathSFX       = content.Load<SoundEffect>("sfx\\enemyDeath");
            playerJoinSFX       = content.Load<SoundEffect>("sfx\\playerJoin");
            playerFireSFX       = content.Load<SoundEffect>("sfx\\playerFireLoop");
            cursorMoveSFX       = content.Load<SoundEffect>("sfx\\cursorMove");
            cursorSelectSFX     = content.Load<SoundEffect>("sfx\\cursorSelect");
            levelUnlockedSFX    = content.Load<SoundEffect>("sfx\\levelUnlock");

            max300              = content.Load<Song>("songs\\Max300");

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

        public static void Update(float deltaTime)
        {
            if (GameState.Level != null)
            {
                if (GameState.Level.IsPlayerFiring())
                {
                    if (!playingPlayerFireSFX)
                    {
                        StartPlayerFireSFX();
                    }
                }
                else
                {
                    if (playingPlayerFireSFX)
                    {
                        StopPlayerFireSFX();
                    }
                }
            }
        }

        public static void StartPlayerFireSFX()
        {
            playerFireSFXInstance.Play();
            playingPlayerFireSFX = true;
        }

        public static void StopPlayerFireSFX()
        {
            playerFireSFXInstance.Stop(false);
            playingPlayerFireSFX = false;
        }

        public static void PlayMaxSong()
        {
            MediaPlayer.Play(max300);
        }
    }
}
