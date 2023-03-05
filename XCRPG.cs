using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThunderRoad;
using UnityEngine;
using XenobladeRPG;

namespace HighEntiaTomb
{
    class XCRPGModule : LevelModule
    {
        AudioSource battleMusic;
        AudioSource uniqueBattleMusic;
        bool isFightingUnique;
        public override IEnumerator OnLoadCoroutine()
        {
            battleMusic = level.customReferences.Find(match => match.name == "Music").transforms.Find(match => match.name == "BattleMusic").GetComponent<AudioSource>();
            uniqueBattleMusic = level.customReferences.Find(match => match.name == "Music").transforms.Find(match => match.name == "UniqueBattleMusic").GetComponent<AudioSource>();
            return base.OnLoadCoroutine();
        }
        public override void Update()
        {
            base.Update();
            if (!WaveSpawner.TryGetRunningInstance(out WaveSpawner spawner))
            {
                isFightingUnique = false;
                foreach (Creature creature in Creature.allActive)
                {
                    if (creature?.brain?.currentTarget != null && !creature.isPlayer && !creature.isKilled && (creature.brain.currentTarget == Player.local.creature || creature.brain.currentTarget.faction == Player.local.creature.faction))
                    {
                        if (creature.GetComponent<XenobladeStats>() != null && creature.GetComponent<XenobladeStats>().isUnique)
                        {
                            isFightingUnique = true;
                            if (uniqueBattleMusic.isPlaying) return;
                            uniqueBattleMusic.Play();
                            battleMusic.mute = true;
                        }
                    }
                }
                if (isFightingUnique || !uniqueBattleMusic.isPlaying) return;
                uniqueBattleMusic.Stop();
                battleMusic.mute = false;
            }
        }
    }
}
