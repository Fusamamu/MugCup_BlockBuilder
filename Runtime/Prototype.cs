using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MugCup_BlockBuilder
{
    public class Prototype : MonoBehaviour
    {
        public string Name;
        
        public Mesh MeshPrototype;

	    public int RotationIndex;
        
        [Header("Socket")]
        public string PosXSocket = "PosX";
        public string NegXSocket = "NegX";
        public string PosZSocket = "PosZ";
        public string NegZSocket = "NegZ";
        public string PosYSocket = "PosY";
        public string NegYSocket = "NegY";

        [Header("Neighbors")]
        public List<string> PosXNeighbors = new List<string>();
        public List<string> NegXNeighbors = new List<string>();
        public List<string> PosZNeighbors = new List<string>();
        public List<string> NegZNeighbors = new List<string>();
        public List<string> PosYNeighbors = new List<string>();
        public List<string> NegYNeighbors = new List<string>();
        
        [Header("Debug Setting")]
        [SerializeField] private bool ShowGizmos;
        [SerializeField] private bool ShowDebugText;
        [SerializeField] private bool ShowGizmosOnSelected;

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

        public void CreatePrototype(bool _focusProjectWindow = false)
        {
            var _prototypeData = ScriptableObject.CreateInstance<PrototypeData>();
            _prototypeData.CopyData(this);

            var _targetFolder = "Packages/com.mugcupp.mugcup-blockbuilder/Editor Resources/Setting/Prototypes";
            var _fileName     = $"{Name}.asset";

            AssetDatabase.CreateAsset(_prototypeData, _targetFolder + "/" + _fileName);
            AssetDatabase.SaveAssets();

            if (_focusProjectWindow)
            {
                EditorUtility.FocusProjectWindow();
                Selection.activeObject = _prototypeData;
            }
        }
#endif
        
        public void SetShowGizmos(bool _value)
        {
            ShowGizmos = _value;
        }

        public void SetShowGizmosOnSelected(bool _value)
        {
            ShowGizmosOnSelected = _value;
        }

        public void SetShowDebugText(bool _value)
        {
            ShowDebugText = _value;
        }
        
        private void OnDrawGizmos()
        {
            if(ShowGizmosOnSelected || !ShowGizmos) return;
            
            var _center = transform.position;
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(_center, 0.05f);
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(_center, Vector3.one);
            
            if(!ShowDebugText) return;
	        
            Handles.Label(_center + Vector3.right/2,   PosXSocket);
            Handles.Label(_center + Vector3.left/2,    NegXSocket);
            Handles.Label(_center + Vector3.forward/2, PosZSocket);
            Handles.Label(_center + Vector3.back/2,    NegZSocket);
            Handles.Label(_center + Vector3.up/2,      PosYSocket);
            Handles.Label(_center + Vector3.down/2,    NegYSocket);

            var _guiStyle = new GUIStyle
            {
                normal = { textColor = Color.yellow },
                alignment = TextAnchor.MiddleCenter
            };

            Handles.Label(_center + Vector3.up,  gameObject.name, _guiStyle);
        }
    }
}
