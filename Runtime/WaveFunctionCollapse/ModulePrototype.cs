using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MugCup_BlockBuilder
{
    public class ModulePrototype : MonoBehaviour
    {
        public int Index;
        
        public string Name;

        public int BitMask;

        public Mesh MeshPrototype;

        public int RotationIndex;

        [Header("Socket")]
        public const int FaceCount = 6;

        public FaceDetails FaceDetails = new FaceDetails();

        [Header("Debug Setting")]
        [SerializeField] private bool ShowGizmos;
        [SerializeField] private bool ShowPivot;
        [SerializeField] private bool ShowBoundBox;
        [SerializeField] private bool ShowDebugText;
        [SerializeField] private bool ShowGizmosOnSelected;

        public void SetBitMask(int _bit)
        {
            BitMask = _bit;
        }

#if UNITY_EDITOR
        public void TryUpdateData()
        {
            Name = gameObject.name;
            
            if (gameObject.TryGetComponent<MeshFilter>(out var _meshFilter))
            {
                MeshPrototype = _meshFilter.sharedMesh;
            }
            
            EditorUtility.SetDirty(this);
        }
        
        public Module CreateModule()
        {
            var _prototypeData = ScriptableObject.CreateInstance<Module>();
            _prototypeData.CopyData(this);

            return _prototypeData;
        }
        
        public void Reset() 
        {
            FaceDetails.Reset();
        }
#endif
        
        public void SetShowGizmos(bool _value)
        {
            ShowGizmos = _value;
        }

        public void SetShowPivot(bool _value)
        {
            ShowPivot = _value;
        }

        public void SetShowBoundBox(bool _value)
        {
            ShowBoundBox = _value;
        }

        public void SetShowGizmosOnSelected(bool _value)
        {
            ShowGizmosOnSelected = _value;
        }

        public void SetShowDebugText(bool _value)
        {
            ShowDebugText = _value;
        }
        
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if(ShowGizmosOnSelected || !ShowGizmos) return;
                
            var _center = transform.position;

            if (ShowPivot)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(_center, 0.05f);
            }

            if (ShowBoundBox)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireCube(_center, Vector3.one);
            }
            
            if(!ShowDebugText) return;

            var _guiStyle = new GUIStyle
            {
                normal = { textColor = Color.yellow },
                alignment = TextAnchor.MiddleCenter
            };

            Handles.Label(_center + Vector3.up,  gameObject.name, _guiStyle);
        }

        private static GUIStyle style;
        
        [DrawGizmo(GizmoType.InSelectionHierarchy | GizmoType.NotInSelectionHierarchy)]
        private static void DrawGizmos(ModulePrototype _modulePrototype, GizmoType _gizmoType)
        {
            if (style == null) 
            {
                style = new GUIStyle
                {
                    alignment = TextAnchor.MiddleCenter
                };
            }
            
            style.normal.textColor = Color.white;

            var _center = _modulePrototype.transform.position;

            foreach (var _kvp in _modulePrototype.FaceDetails.Faces)
            {
                var _moduleFace = _kvp.Key;
                var _face       = _kvp.Value;
                
                Handles.Label(_center + Orientations.Rotations[_moduleFace] * Vector3.forward / 2f, _face.ToString(), style);
            }
        }
#endif
    }
}
