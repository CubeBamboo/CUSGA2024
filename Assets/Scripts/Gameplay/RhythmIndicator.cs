using Shuile.Rhythm.Runtime;

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using UnityEngine;

namespace Shuile.Gameplay
{
    public class RhythmIndicator : MonoBehaviour
    {
        private struct Note
        {
            public RectTransform transform;
            public CanvasGroup alphaGroup;
            public float hitTime;

            public Note(RectTransform transform, CanvasGroup alphaGroup, float hitTime)
            {
                this.transform = transform;
                this.alphaGroup = alphaGroup;
                this.hitTime = hitTime;
            }
        }

        [SerializeField] private float distanceUnit;
        [SerializeField] private float preDisplayTime;

        private GameObject notePrefab;
        private ObjectPool<GameObject> notePool;
        private List<Note> notes;
        private MusicRhythmManager rhythmManager;
        private float lastRealTime = 0f;
        private float lerpTime;
        private ChartPlayer chartPlayer;

        private float MissTolerance => rhythmManager.MissToleranceInSeconds;

        public RhythmIndicator()
        {
            notePool = new ObjectPool<GameObject>(() => Instantiate(notePrefab, transform),
                ObjectPoolFuncStore.GameObjectGet,
                ObjectPoolFuncStore.GameObjectRelease,
                ObjectPoolFuncStore.GameObjectDestroy,
                8);
            notes = new(8);
        }

        private void Awake()
        {
            notePrefab = LevelResources.Instance.globalPrefabs.noteIndicator;
            rhythmManager = MusicRhythmManager.Instance;
            chartPlayer = new ChartPlayer(ChartDataCreator.CreatePlayerDefault(), GetPlayTime);
            chartPlayer.OnNotePlay += OnNote;
            // TODO: listen player attack event
        }

        private void Update()
        {
            // TODO: if (gameState is not playing) return;
            if (rhythmManager.CurrentTime == 0f)
                return;

            if (lastRealTime != rhythmManager.CurrentTime)
                lerpTime = lastRealTime = rhythmManager.CurrentTime;
            else
                lerpTime += Time.deltaTime;

            chartPlayer.PlayUpdate(lerpTime);
            for (int i = 0; i < notes.Count; )
            {
                if (lerpTime - MissTolerance >= notes[i].hitTime)
                {
                    notePool.Release(notes[i].transform.gameObject);
                    notes.UnorderedRemoveAt(i);
                    continue;
                }

                UpdateNoteGraphic(notes[i++]);
            }
        }

        private void OnDestroy()
        {
            notes.Clear();
            notePool.DestroyAll();
        }

        private void OnNote(BaseNoteData noteData)
        {
            var obj = notePool.Get();
            var canvasGroup = obj.GetComponent<CanvasGroup>();
            canvasGroup.alpha = 1f;
            notes.Add(new ((RectTransform)obj.transform, canvasGroup, noteData.ToPlayTime()));
        }

        private float GetPlayTime(BaseNoteData note) => note.ToPlayTime() - preDisplayTime;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void UpdateNoteGraphic(Note note)
        {
            var delta = note.hitTime - lerpTime;
            note.transform.localPosition = note.transform.localPosition.With(x: distanceUnit * delta);
            note.alphaGroup.alpha = 1f - Mathf.Clamp01((delta < 0 ? -delta : delta - preDisplayTime + MissTolerance) / MissTolerance);
        }
    }
}
