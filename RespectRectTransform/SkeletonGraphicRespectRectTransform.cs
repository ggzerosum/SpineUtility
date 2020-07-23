using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ProvisGames.Core.SpineSupport
{
    /// <summary>
    /// Make SkeletonGraphic fit into RectTransform
    /// </summary>
    [ExecuteAlways]
    [AddComponentMenu("Spine/RespectRectTranform")]
    public class SkeletonGraphicRespectRectTransform : MonoBehaviour
    {
        [SerializeField] private SkeletonGraphic skeletonGraphic;
        [SerializeField] private bool m_preserveAspectRatio = true;
        [Header("Call skeletonGraphic.SetToSetupPose when scale changed")]
        [SerializeField] private bool m_SetToSetupPoseWhenScaling = false;
        [HideInInspector] public bool m_isEditorUpdateStopped = false;

        // Cache initial bound & skeleton scale for standard of size change
        [HideInInspector] [SerializeField] private Rect m_initialRectWithBounds;
        [HideInInspector] [SerializeField] private Vector2 m_initialSpineSkeletonScale;

        private bool m_isInitialized = false;

        void Reset()
        {
            skeletonGraphic = this.GetComponent<SkeletonGraphic>();
            SetScale(1.0f, 1.0f);
            CacheRectTransformWithBounds();
        }
        void Start()
        {
            this.Initialize(false);
        }
        void Update()
        {
            if (Application.isEditor && m_isEditorUpdateStopped)
                return;

            RespectRectTranform();
        }

        void Initialize(bool overwrite)
        {
            if (m_isInitialized && !overwrite)
                return;

            skeletonGraphic.Initialize(false);

            m_isInitialized = true;
        }

        public bool RespectRectTranform()
        {
            Rect currentRect = this.skeletonGraphic.GetPixelAdjustedRect();
            if (currentRect.size.sqrMagnitude > 0.0f && m_initialRectWithBounds.size.sqrMagnitude > 0.0f && m_initialSpineSkeletonScale.sqrMagnitude > 0.0f)
            {
                if (m_preserveAspectRatio)
                {
                    Vector2 size = PreserveAspectRatio(currentRect, m_initialRectWithBounds);
                    currentRect.width = size.x;
                    currentRect.height = size.y;
                }

                float xDiff = currentRect.width / m_initialRectWithBounds.width;
                float yDiff = currentRect.height / m_initialRectWithBounds.height;

                SetScale(m_initialSpineSkeletonScale.x * xDiff, m_initialSpineSkeletonScale.y * yDiff);

                return true;
            }

            return false;
        }
        private Vector2 PreserveAspectRatio(Rect rect, Rect spine)
        {
            float spineAspectRatio = spine.width / spine.height;
            float rectAspectRatio = rect.width / rect.height;

            if (spineAspectRatio > rectAspectRatio)
            {
                float adjustedHeight = rect.width * (1.0f / spineAspectRatio);
                return new Vector2(rect.width, adjustedHeight);
            }
            else
            {
                float adjustedWidth = rect.height * spineAspectRatio;
                return new Vector2(adjustedWidth, rect.height);
            }
        }

        private void SetScale(float x, float y)
        {
            this.skeletonGraphic.Skeleton.ScaleX = x;
            this.skeletonGraphic.Skeleton.ScaleY = y;

            if (m_SetToSetupPoseWhenScaling)
                this.skeletonGraphic.Skeleton.SetToSetupPose();

            this.skeletonGraphic.Skeleton.UpdateWorldTransform();
        }

        public bool CacheRectTransformWithBounds()
        {
            bool result = MatchRectTransformWithBounds_lowVersionCompatablity(this.skeletonGraphic);
            if (result) CacheSize();

            return result;
        }
        private void CacheSize()
        {
            m_initialRectWithBounds = this.skeletonGraphic.GetPixelAdjustedRect();
            m_initialSpineSkeletonScale = new Vector2(this.skeletonGraphic.Skeleton.ScaleX, this.skeletonGraphic.Skeleton.ScaleY);
        }

        private bool MatchRectTransformWithBounds_lowVersionCompatablity(SkeletonGraphic skeletonGraphic)
        {
            skeletonGraphic.UpdateMesh();

            Mesh mesh = skeletonGraphic.GetLastMesh();
            if (mesh == null)
            {
                return false;
            }

            if (mesh.vertexCount == 0)
            {
                skeletonGraphic.rectTransform.sizeDelta = new Vector2(50f, 50f);
                skeletonGraphic.rectTransform.pivot = new Vector2(0.5f, 0.5f);
                return false;
            }

            mesh.RecalculateBounds();
            var bounds = mesh.bounds;
            var size = bounds.size;
            var center = bounds.center;
            var p = new Vector2(
                0.5f - (center.x / size.x),
                0.5f - (center.y / size.y)
            );

            skeletonGraphic.rectTransform.sizeDelta = size;
            skeletonGraphic.rectTransform.pivot = p;
            return true;
        }
    }

    #if UNITY_EDITOR
    [CustomEditor(typeof(SkeletonGraphicRespectRectTransform))]
    [CanEditMultipleObjects]
    internal class SkeletonGraphicRespectRectTransformDrawer : UnityEditor.Editor
    {
        private SerializedProperty m_isEditorUpdateStopped;

        protected virtual void OnDisable()
        {

        }
        protected virtual void OnEnable()
        {
            m_isEditorUpdateStopped = this.serializedObject.FindProperty("m_isEditorUpdateStopped");
        }
        protected virtual void OnValidate()
        {

        }

        public sealed override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            this.serializedObject.UpdateIfRequiredOrScript();

            DrawSerializedProperty(this.serializedObject);
            DrawInspector();

            if (EditorGUI.EndChangeCheck())
            {
                this.serializedObject.ApplyModifiedProperties();
            }
        }

        protected void DrawInspector()
        {
            EditorGUILayout.Space();

            GUIContent matchButtonLabel = new GUIContent("Make current mesh bound as standard of spine size change");
            Vector2 matchButtonSize = GUI.skin.label.CalcSize(matchButtonLabel);

            GUIContent resizeButtonLabel = new GUIContent("Can Resize Spine with RectTranform Manually");
            Vector2 resizeButtonSize = GUI.skin.label.CalcSize(resizeButtonLabel);

            float maxButtonSize = Mathf.Max(matchButtonSize.x, resizeButtonSize.x);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(matchButtonLabel, GUILayout.Width(maxButtonSize));
            if (GUILayout.Button("Fit & Cache", EditorStyles.miniButton, GUILayout.Width(120f)))
            {
                foreach (Object target in targets)
                {
                    if (target is SkeletonGraphicRespectRectTransform instance)
                    {
                        instance.CacheRectTransformWithBounds();
                    }
                }
            }
            EditorGUILayout.EndHorizontal();


            bool guiPrevState = GUI.enabled;
            GUI.enabled = Application.isPlaying; // Allow Manual Resize when playmode
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(resizeButtonLabel, GUILayout.Width(maxButtonSize));
            if (GUILayout.Button("Resize", EditorStyles.miniButton, GUILayout.Width(120f)))
            {
                foreach (Object target in targets)
                {
                    if (target is SkeletonGraphicRespectRectTransform instance)
                    {
                        instance.RespectRectTranform();
                    }
                }
            }
            GUI.enabled = guiPrevState;
            EditorGUILayout.EndHorizontal();


            EditorGUILayout.Space();
            bool prev = GUI.enabled;
            GUI.enabled = false;
            EditorGUILayout.PropertyField(m_isEditorUpdateStopped);
            GUI.enabled = prev;
            EditorGUILayout.BeginHorizontal();

            string buttonName = string.Empty;
            Color prevColor = GUI.color;
            if (m_isEditorUpdateStopped.boolValue)
            {
                buttonName = "Play Editor Update";
                GUI.color = new Color(0.45f, 0.63f, 0.76f);
            }
            else
            {
                buttonName = "Stop Editor Update";
                GUI.color = new Color(0.83f, 0.13f, 0.18f);
            }
            if (GUILayout.Button(buttonName))
            {
                m_isEditorUpdateStopped.boolValue = !m_isEditorUpdateStopped.boolValue;
            }
            GUI.color = prevColor;
            EditorGUILayout.EndHorizontal();
        }

        protected void DrawSerializedProperty(SerializedObject obj, bool isEnterChildren = true)
        {
            SerializedProperty iterator = obj.GetIterator();

            while (iterator.NextVisible(isEnterChildren))
            {
                using (new EditorGUI.DisabledScope("m_Script" == iterator.propertyPath))
                    EditorGUILayout.PropertyField(iterator, true);
            }
        }
    }
    #endif
}
