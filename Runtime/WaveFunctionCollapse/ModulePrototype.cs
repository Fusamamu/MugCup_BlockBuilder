using System;
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

        public List<ModulePrototype> ForwardNeighbors = new List<ModulePrototype>();
        public List<ModulePrototype> RightNeighbors   = new List<ModulePrototype>();
        public List<ModulePrototype> BackNeighbors    = new List<ModulePrototype>();
        public List<ModulePrototype> LeftNeighbors    = new List<ModulePrototype>();
        public List<ModulePrototype> UpNeighbors      = new List<ModulePrototype>();
        public List<ModulePrototype> DownNeighbors    = new List<ModulePrototype>();


        [Header("Debug Setting")]
        [SerializeField] private bool ShowGizmos;
        [SerializeField] private bool ShowPivot;
        [SerializeField] private bool ShowBoundBox;
        [SerializeField] private bool ShowDebugText;
        [SerializeField] private bool ShowGizmosOnSelected;
        [SerializeField] private bool ShowFaceBits;
        [SerializeField] private bool ShowBitInBinary;

        public ModulePrototype SetBitMask(int _bit)
        {
            BitMask = _bit;
            return this;
        }

        public ModulePrototype UpdateFaceBits()
        {
            FaceDetails.UpdateFaceBits(BitMask);
            return this;
        }
#if UNITY_EDITOR
        public void TryUpdateData()
        {
            Name = gameObject.name;
            
            if (gameObject.TryGetComponent<MeshFilter>(out var _meshFilter))
            {
                MeshPrototype = _meshFilter.sharedMesh;
            }

            UpdateFaceBits();
            
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
        
        public void StorePossibleNeighbors(List<ModulePrototype> _allModuleInScene)
        {
            foreach (ModuleFace _face in Enum.GetValues(typeof(ModuleFace)))
            {
                switch (_face)
                {
                    case ModuleFace.FORWARD:
                        ForwardNeighbors = GetValidNeighbors(_face, _allModuleInScene);
                        break;
                    case ModuleFace.RIGHT:
                        RightNeighbors   = GetValidNeighbors(_face, _allModuleInScene);
                        break;
                    case ModuleFace.BACK:
                        BackNeighbors    = GetValidNeighbors(_face, _allModuleInScene);
                        break;
                    case ModuleFace.LEFT:
                        LeftNeighbors    = GetValidNeighbors(_face, _allModuleInScene);
                        break;
                    case ModuleFace.UP:
                        UpNeighbors      = GetValidNeighbors(_face, _allModuleInScene);
                        break;
                    case ModuleFace.DOWN:
                        DownNeighbors    = GetValidNeighbors(_face, _allModuleInScene);
                        break;
                }
            }
        }

        private List<ModulePrototype> GetValidNeighbors(ModuleFace _face, List<ModulePrototype> _allModuleInScene)
        {
            var _newModuleSet = new List<ModulePrototype>();
                    
            foreach (var _otherModule in _allModuleInScene)
            {
                if (CheckFaceIsFit(_face, _otherModule))
                {
                    _newModuleSet.Add(_otherModule);
                }
            }

            return _newModuleSet;
        }

        private bool CheckFaceIsFit(ModuleFace _face, ModulePrototype _otherModule)
        {
            var _oppositeFace = Orientations.GetOppositeFace(_face);
            
            if (Orientations.IsHorizontal(_face))
            {
                var _thisFace  =              FaceDetails.Faces[_face]         as HorizontalFaceDetails;
                var _otherFace = _otherModule.FaceDetails.Faces[_oppositeFace] as HorizontalFaceDetails;

                return _thisFace?.Equals(_otherFace) ?? false;
            }
            else
            {
                var _thisFace  =              FaceDetails.Faces[_face]         as VerticalFaceDetails;
                var _otherFace = _otherModule.FaceDetails.Faces[_oppositeFace] as VerticalFaceDetails;
                
                return _thisFace?.Equals(_otherFace) ?? false;
            }
        }
        
#region Set Gizmos Setting
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

        public void SetShowFaceBits(bool _value)
        {
            ShowFaceBits = _value;
        }

        public void SetShowBitInBinary(bool _value)
        {
            ShowBitInBinary = _value;
        }
#endregion
        
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
            
            Handles.Label(_center + Vector3.up * 1.3f, $"{Convert.ToString(BitMask, 2).PadLeft(8, '0').Insert(4, "_")}", _guiStyle);

            if (ShowFaceBits)
            {
                foreach (var _kvp in FaceDetails.Faces)
                {
                    var _moduleFace = _kvp.Key;
                    var _face       = _kvp.Value;

                    var _bitText = ShowBitInBinary ?
                        BitUtil.Get4BitTextFormat(_face.FaceBit) : $"[{_face.FaceBit}]";
                    
                    Handles.Label(_center + Orientations.Rotations[_moduleFace] * Vector3.forward / 2f, _bitText, style);
                }
            }
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
                
                //Handles.Label(_center + Orientations.Rotations[_moduleFace] * Vector3.forward / 2f, _face.ToString(), style);
            }
        }
#endif
    }
}
