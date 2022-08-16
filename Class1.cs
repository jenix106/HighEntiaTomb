using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThunderRoad;
using UnityEngine;

namespace HighEntiaTomb
{
    public class HighEntiaTombModule : LevelModule
    {
        AudioSource battleMusic;
        AudioSource uniqueBattleMusic;
        AudioSource music;
        bool isInCombat;
        public override IEnumerator OnLoadCoroutine()
        {
            battleMusic = level.customReferences.Find(match => match.name == "Music").transforms.Find(match => match.name == "BattleMusic").GetComponent<AudioSource>();
            uniqueBattleMusic = level.customReferences.Find(match => match.name == "Music").transforms.Find(match => match.name == "UniqueBattleMusic").GetComponent<AudioSource>();
            music = level.customReferences.Find(match => match.name == "Music").transforms.Find(match => match.name == "Music").GetComponent<AudioSource>();
            foreach(Transform transform in level.customReferences.Find(match => match.name == "Climbable").transforms)
            {
                transform.gameObject.AddComponent<ClimbableComponent>();
            }
            return base.OnLoadCoroutine();
        }
        public override void Update()
        {
            base.Update();
            isInCombat = false;
            foreach (Creature creature in Creature.allActive)
            {
                if (creature?.brain?.currentTarget != null && !creature.isPlayer && !creature.isKilled && (creature.brain.currentTarget.isPlayer || creature.brain.currentTarget.faction == Player.local.creature.faction))
                {
                    isInCombat = true;
                    if (battleMusic.isPlaying) return;
                    battleMusic.Play();
                    music.Pause();
                }
            }
            if (isInCombat || !battleMusic.isPlaying) return;
            uniqueBattleMusic.Stop();
            battleMusic.Stop();
            battleMusic.mute = false;
            music.UnPause();
        }
    }
    public class ClimbableComponent : MonoBehaviour
    {
        public void OnCollisionStay(Collision c)
        {
            if (c.collider.GetComponentInParent<Player>() != null)
            {
                RagdollHandClimb.climbFree = true;
            }
        }
        public void OnCollisionExit(Collision c)
        {
            if (c.collider.GetComponentInParent<Player>() != null)
            {
                RagdollHandClimb.climbFree = false;
            }
        }
    }
}
