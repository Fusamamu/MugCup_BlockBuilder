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

        [Space(10)]
        public HorizontalFaceDetails Forward;
        public HorizontalFaceDetails Back;
        public HorizontalFaceDetails Left;
        public HorizontalFaceDetails Right;
        public VerticalFaceDetails Up;
        public VerticalFaceDetails Down;

        public Dictionary<ModuleFace, IFaceDetails> Faces 
        {
            get
            {
                if (FaceMap == null || FaceMap.Count == 0)
                {
                    FaceMap = new Dictionary<ModuleFace, IFaceDetails>
                    {
                        { ModuleFace.FORWARD, Forward },
                        { ModuleFace.BACK   , Back    },
                        { ModuleFace.LEFT   , Left    },
                        { ModuleFace.RIGHT  , Right   },
                        { ModuleFace.UP     , Up      },
                        { ModuleFace.DOWN   , Down    },
                    };
                }
                
                return FaceMap;
            }
        }

        public Dictionary<ModuleFace, IFaceDetails> FaceMap = new Dictionary<ModuleFace, IFaceDetails>();


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

        // public PrototypeData CreatePrototype(bool _focusProjectWindow = false)
        // {
        //     var _prototypeData = ScriptableObject.CreateInstance<PrototypeData>();
        //     _prototypeData.CopyData(this);
        //
        //     var _targetFolder = "Packages/com.mugcupp.mugcup-blockbuilder/Editor Resources/Setting/Prototypes";
        //     var _fileName     = $"{Name}.asset";
        //
        //     AssetDatabase.CreateAsset(_prototypeData, _targetFolder + "/" + _fileName);
        //     AssetDatabase.SaveAssets();
        //
        //     if (_focusProjectWindow)
        //     {
        //         EditorUtility.FocusProjectWindow();
        //         Selection.activeObject = _prototypeData;
        //     }
        //
        //     return _prototypeData;
        // }
        
        public Module CreateModule()
        {
            var _prototypeData = ScriptableObject.CreateInstance<Module>();
            _prototypeData.CopyData(this);

            return _prototypeData;
        }
        
        public void Reset() 
        {
            Forward = new HorizontalFaceDetails();
            Back    = new HorizontalFaceDetails();
            Left    = new HorizontalFaceDetails();
            Right   = new HorizontalFaceDetails();
            Up      = new VerticalFaceDetails  ();
            Down    = new VerticalFaceDetails  ();
            
            // foreach (var face in this.Faces) {
            //     face.ExcludedNeighbours = new ModulePrototype[] { };
            // }
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
	        
            // Handles.Label(_center + Vector3.right/2,   PosXSocket);
            // Handles.Label(_center + Vector3.left/2,    NegXSocket);
            // Handles.Label(_center + Vector3.forward/2, PosZSocket);
            // Handles.Label(_center + Vector3.back/2,    NegZSocket);
            // Handles.Label(_center + Vector3.up/2,      PosYSocket);
            // Handles.Label(_center + Vector3.down/2,    NegYSocket);

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

            foreach (var _kvp in _modulePrototype.Faces)
            {
                var _moduleFace = _kvp.Key;
                var _face       = _kvp.Value;
                
                Handles.Label(_center + Orientations.Rotations[_moduleFace] * Vector3.forward / 2f, _face.ToString(), style);
            }
        }
#endif
    }
}
