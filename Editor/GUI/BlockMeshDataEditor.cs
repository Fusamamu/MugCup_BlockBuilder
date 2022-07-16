// using System;
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using BlockBuilder.Core.Scriptable;
// using BlockBuilder.Core;
// using BlockBuilder.Runtime.Core;
// using MugCup_BlockBuilder.Runtime;
// using MugCup_BlockBuilder.Runtime.Core;
// using Object = System.Object;
// #if UNITY_EDITOR
// using UnityEditor;
// using UnityEditorInternal;
// #endif
//
// namespace BlockBuilder.Editor.GUI
// {
// #if UNITY_EDITOR
//     [CustomEditor(typeof(BlockMeshData))]
//     public class BlockMeshDataEditor : UnityEditor.Editor
//     {
// 	    private Texture isolatedBlockTexture;
// 	    private Texture topSurfaceBlockTexture;
//
// 	    private SerializedProperty isolatedBlock;
// 	    private SerializedProperty topSurfaceBlock;
//
// 	    private SerializedProperty topSizeW;
// 	    private SerializedProperty topSizeN;
// 	    private SerializedProperty topSizeE;
// 	    private SerializedProperty topSizeS;
//
// 	    private SerializedProperty topCornerNW;
// 	    private SerializedProperty topCornerNE;
// 	    private SerializedProperty topCornerSW;
// 	    private SerializedProperty topCornerSE;
//
// 	    private bool displayNotRotatableBlocks = false;
//
// 	    private PreviewRenderUtility previewRenderUtility;
// 	    private RenderTexture renderTexture;
//
// 	    private void OnEnable()
// 	    {
// 		    if(previewRenderUtility != null)
// 			    previewRenderUtility.Cleanup();
// 		    else
// 			    previewRenderUtility = new PreviewRenderUtility();
// 		    
// 		    var _camera = previewRenderUtility.camera;
// 		    _camera.fieldOfView   = 30f;
// 		    _camera.nearClipPlane = 0.01f;
// 		    _camera.farClipPlane  = 1000;
// 		    _camera.transform.position = new Vector3(0.025f, 0.025f, -0.025f);
// 		    _camera.transform.LookAt(Vector3.zero + new Vector3(0, 0.005f, 0));
// 		    
// 		    isolatedBlock   = serializedObject.FindProperty("IsolatedBlock");
// 		    topSurfaceBlock = serializedObject.FindProperty("TopSurfaceBlock");
//
// 		    topSizeW = serializedObject.FindProperty("SizeW");
// 		    topSizeN = serializedObject.FindProperty("SizeN");
// 		    topSizeE = serializedObject.FindProperty("SizeE");
// 		    topSizeS = serializedObject.FindProperty("SizeS");
// 		    topCornerNW = serializedObject.FindProperty("CornerNW");
// 		    topCornerNE = serializedObject.FindProperty("CornerNE");
// 		    topCornerSW = serializedObject.FindProperty("CornerSW");
// 		    topCornerSE = serializedObject.FindProperty("CornerSE");
// 	    }
//
// 	    public override void OnInspectorGUI()
// 	    {
// 		    isolatedBlockTexture   = AssetDatabase.LoadAssetAtPath<Texture>("Packages/com.mugcupp.mugcup-blockbuilder/Editor Resources/Images/Icons/Isolated Block.png");
// 		    topSurfaceBlockTexture = AssetDatabase.LoadAssetAtPath<Texture>("Packages/com.mugcupp.mugcup-blockbuilder/Editor Resources/Images/Icons/Top Surface Block.png");
// 		    
// 		    EditorGUI.DrawRect(new Rect(0, 0, 300, 50), Color.black);
// 		    EditorGUILayout.Space(50);
// 		    
// 		    displayNotRotatableBlocks = EditorGUILayout.Foldout(displayNotRotatableBlocks, "Non-Rotatable Blocks");
//
// 		    if (displayNotRotatableBlocks)
// 		    {
// 			    EditorGUI.DrawPreviewTexture(new Rect(30, 80, 30, 30), isolatedBlockTexture);
// 			    EditorGUI.LabelField(new Rect(30, 110, 100, 30), "[Isolated]");
// 			    
// 			    EditorGUI.DrawPreviewTexture(new Rect(120, 80, 30, 30), topSurfaceBlockTexture);
// 			    EditorGUI.LabelField(new Rect(120, 110, 100, 30), "[Top Surface]");
// 			   
// 			    EditorGUILayout.BeginVertical();
// 			    
// 			    
// 			    
// 				EditorGUILayout.Space(70);
// 			    EditorGUILayout.PropertyField(isolatedBlock, new GUIContent("Isolated Block"));
// 			    EditorGUILayout.PropertyField(topSurfaceBlock, new GUIContent("Top Surface Block"));
// 			    EditorGUILayout.EndVertical();
// 		
//
// 			    if (previewRenderUtility == null)
// 				    previewRenderUtility = new PreviewRenderUtility();
//
//
// 			    var _prefab = isolatedBlock.objectReferenceValue as Block;
//
// 			    if (_prefab != null)
// 			    {
// 				    var _texture = CreatePreviewTexture(new Rect(150, 80, 128, 128), _prefab);
// 				    EditorGUI.DrawPreviewTexture(new Rect(150, 80, 128, 128), _texture);
// 			    }
// 			    else
// 			    {
// 				    Debug.Log("Prefab null");
// 			    }
//
// 			    var _prefabB = topSurfaceBlock.objectReferenceValue as Block;
//
// 			    if (_prefabB != null)
// 			    {
// 				    var _tex = CreatePreviewTexture(new Rect(300, 80, 128, 128), _prefabB);
// 				    EditorGUI.DrawPreviewTexture(new Rect(300, 80, 128, 128), _tex);
// 			    }
//
// 			    EditorGUILayout.Space(40);
// 		    }
// 		    
// 		    DrawDefaultInspector();
// 	    }
//
// 	
//
// 	    private Texture CreatePreviewTexture(Rect _rect, Block _block)
// 	    {
// 		    MeshFilter _meshFilter     = _block.GetComponent<MeshFilter>();
// 		    MeshRenderer _meshRenderer = _block.GetComponent<MeshRenderer>();
// 				    
// 		    previewRenderUtility.BeginPreview(_rect, GUIStyle.none);
// 		    previewRenderUtility.lights[0].transform.localEulerAngles = new Vector3(30, 30, 0);
// 		    previewRenderUtility.lights[0].intensity = 2;
// 		    previewRenderUtility.DrawMesh(_meshFilter.sharedMesh, Matrix4x4.identity, _meshRenderer.sharedMaterial, 0);
// 		    previewRenderUtility.camera.Render();
// 		    
// 		    return previewRenderUtility.EndPreview();
// 	    }
// 	    
// 	    private RenderTexture CreatePreviewTexture(GameObject obj) {
// 		    previewRenderUtility.BeginPreview(new Rect(0, 0, 128, 128), GUIStyle.none);
//
// 		    previewRenderUtility.lights[0].transform.localEulerAngles = new Vector3(30, 30, 0);
// 		    previewRenderUtility.lights[0].intensity = 2;
// 		    previewRenderUtility.AddSingleGO(obj);
// 		    previewRenderUtility.camera.Render();
//
// 		    return (RenderTexture)previewRenderUtility.EndPreview();
// 	    }
//
// 	    private void OnDisable()
// 	    {
// 		    previewRenderUtility.Cleanup();
// 	    }
//     }
// #endif
// }
