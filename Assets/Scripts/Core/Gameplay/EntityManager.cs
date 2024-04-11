using CbUtils;

using System.Collections.Generic;
using System.Collections.ObjectModel;

using UnityEngine;

namespace Shuile.Gameplay
{
    public class EntityManager : MonoSingletons<EntityManager>
    {
        [SerializeField] private List<BehaviourEntity> preset = new();

        private readonly List<Enemy> enemyList = new();
        private readonly List<Gadget> gadgetList = new();
        private readonly List<BehaviourEntity> removeList = new();

        private PrefabConfigSO prefabs;
        private bool judging = false;
        private int frameCounter = 0;
        
        private Transform enemyParent;
        private Transform gadgetParent;
        private Transform propParent;

        public PrefabConfigSO Prefabs
        {
            get
            {
                if (prefabs == null)
                    prefabs = GameplayService.Interface.Get<PrefabConfigSO>();
                return prefabs;
            }
        }
        public ReadOnlyCollection<Enemy> Enemies => enemyList.AsReadOnly();
        public ReadOnlyCollection<Gadget> Gadgets => gadgetList.AsReadOnly();

        protected override void Awake()
        {
            base.Awake();
            enemyParent = new GameObject("Enemies").transform;
            gadgetParent = new GameObject("Gadgets").transform;
            propParent = new GameObject("Props").transform;

            foreach (var entity in preset)
            {
                if (entity.Type == EntityType.Enemy)
                    MarkEnemy((Enemy)entity);
                else if (entity.Type == EntityType.Gadget)
                    MarkGadget((Gadget)entity);
            }
        }

        private void OnEnable()
        {
            AutoPlayChartManager.Instance.OnRhythmHit += OnRhythmHit;
        }

        private void OnDisable()
        {
            AutoPlayChartManager.Instance.OnRhythmHit -= OnRhythmHit;
        }

        private void OnRhythmHit()
        {
            judging = true;
            var version = frameCounter++;

            // Judge enemy first
            foreach (var enemy in enemyList)
                enemy.Judge(version, false);
            foreach (Enemy enemy in removeList)
                RemoveImmediate(enemy);
            removeList.Clear();

            // Then is gadget
            foreach (var gadget in gadgetList)
                gadget.Judge(version, false);
            foreach (Gadget gadget in removeList)
                RemoveImmediate(gadget);
            removeList.Clear();

            judging = false;
        }

        public void Remove<T>(T behaviourEntity) where T : BehaviourEntity
        {
            if (judging)
            {
                removeList.Add(behaviourEntity);
                return;
            }
            RemoveImmediate(behaviourEntity);
        }

        public void RemoveImmediate<T>(T behaviourEntity) where T : BehaviourEntity
        {
            if (behaviourEntity is Gadget gadget)
                gadgetList.UnorderedRemove(gadget);
            else if (behaviourEntity is Enemy enemy)
                enemyList.UnorderedRemove(enemy);
        }

        public Enemy SpawnEnemy(GameObject enemyPrefab, Vector3 pos)
        {
            // if (LevelGrid.Instance.grid.IsOutOfBound(pos))
            //     return null;

            var enemyObject = Instantiate(enemyPrefab, pos, Quaternion.identity, enemyParent);
            var enemy = enemyObject.GetComponent<Enemy>();
            MarkEnemy(enemy);
            return enemy;
        }
        public Gadget SpawnGadget(GameObject gadgetPrefab, float destroyTime, Vector3 pos, Vector3 rotation)
        {
            var gadgetObject = Instantiate(gadgetPrefab, pos, Quaternion.Euler(rotation));
            var gadget = gadgetObject.GetComponent<Gadget>();

            gadget.destroyTime = destroyTime;
            MarkGadget(gadget);
            return gadget;
        }
    
        public void MarkEnemy(Enemy enemy)
        {
            enemyList.Add(enemy);
        }
        public void MarkGadget(Gadget gadget)
        {
            gadgetList.Add(gadget);
        }
    }
}
