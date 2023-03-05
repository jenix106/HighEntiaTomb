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
            foreach (Transform transform in level.customReferences.Find(match => match.name == "NoFallDamage").transforms)
            {
                transform.gameObject.AddComponent<WaterComponent>();
            }
            level.customReferences.Find(match => match.name == "Button").transforms[0].gameObject.AddComponent<ButtonComponent>();
            return base.OnLoadCoroutine();
        }
        public override void Update()
        {
            base.Update();
            if (!WaveSpawner.TryGetRunningInstance(out WaveSpawner spawner))
            {
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
    public class ButtonComponent : MonoBehaviour
    {
        Animator floor;
        Animator shadow;
        bool pressed = false;
        public void Start()
        {
            floor = Level.current.customReferences.Find(match => match.name == "Floor").transforms[0].GetComponent<Animator>();
            shadow = Level.current.customReferences.Find(match => match.name == "Floor").transforms[1].GetComponent<Animator>();
        }
        public void OnCollisionEnter(Collision c)
        {
            if (c.collider.GetComponentInParent<RagdollHand>() != null && !pressed)
            {
                floor.Play("FloorRemove");
                shadow.Play("FloorRemove");
                pressed = true;
            }
        }
    }
    public class WaterComponent : MonoBehaviour
    {
        bool fallDamage;
        bool isStored;
        public void OnTriggerStay(Collider c)
        {
            if (c.GetComponentInParent<Player>() != null)
            {
                if (!isStored)
                {
                    fallDamage = Player.fallDamage;
                    isStored = true;
                }
                Player.fallDamage = false;
            }
        }
        public void OnTriggerExit(Collider c)
        {
            if (c.GetComponentInParent<Player>() != null)
            {
                Player.fallDamage = fallDamage;
                isStored = false;
            }
        }
    }
    public class SetTexture : StateMachineBehaviour
    {
        public List<Renderer> renderers;
        public Texture2D texture;
        public void Start()
        {
            if (Level.current?.customReferences.Find(match => match.name == "Water")?.transforms[0] != null)
                foreach (Transform reference in Level.current.customReferences.Find(match => match.name == "Water").transforms)
                {
                    if (!renderers.Contains(reference.GetComponent<Renderer>())) renderers.Add(reference.GetComponent<Renderer>());
                }
        }
        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (renderers.IsNullOrEmpty())
                foreach (Transform reference in Level.current.customReferences.Find(match => match.name == "Water").transforms)
                {
                    if (!renderers.Contains(reference.GetComponent<Renderer>())) renderers.Add(reference.GetComponent<Renderer>());
                }
            if (!renderers.IsNullOrEmpty())
                foreach (Renderer renderer in renderers)
                {
                    if (renderer != null)
                        renderer.material.SetTexture("_BaseMap", texture);
                }
        }
    }
}
