#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MugCup_BlockBuilder.Editor;
using UnityEditor;
using UnityEngine;

namespace MugCup_BlockBuilder
{
	public static class PaintPanelGUI
	{
		private static Material blockMaterial;
		private static Material waterMaterial;

		private static bool displayMaterialSetting;
        
		public static void Display()
		{
			displayMaterialSetting = BBEditorStyling.DrawHeader(Color.clear, "Material Setting", displayMaterialSetting);

			if (displayMaterialSetting)
			{
				GUILayout.BeginVertical("GroupBox");
				blockMaterial = (Material)EditorGUILayout.ObjectField("Block Material", blockMaterial, typeof(Material));
				waterMaterial = (Material)EditorGUILayout.ObjectField("Block Material", waterMaterial, typeof(Material));

				if (GUILayout.Button("Apply material to all blocks"))
				{
					if (blockMaterial != null)
					{
						var _gridElementDataManager = BBEditorManager.GridElementDataManager;

						var _grassBlocks = _gridElementDataManager.GridElementData.GridNodes.Where(_node => _node.MetaType == '1' || _node.MetaType == '\0');
						var _waterBlocks = _gridElementDataManager.GridElementData.GridNodes.Where(_node => _node.MetaType == '2');

						foreach (var _gridElem in _grassBlocks)
						{
							foreach (var _corner in _gridElem.VolumePoints)
							{
								if(_corner == null)
									continue;
								
								if(!_corner.TryGetComponent<Renderer>(out var _renderer))
									continue;
								
								_renderer.materials = new[]
								{
									blockMaterial
								};
								
								EditorUtility.SetDirty(_corner);
							}
						}
						
						foreach (var _gridElem in _waterBlocks)
						{
							for (var _i = 0; _i < 4; _i++)
							{
								var _corner = _gridElem.VolumePoints[_i];
								if(_corner == null)
									continue;
								
								if(!_corner.TryGetComponent<Renderer>(out var _renderer))
									continue;
								
								_renderer.materials = new[]
								{
									blockMaterial
								};
								
								EditorUtility.SetDirty(_corner);
							}
							
							for(var _i = 4; _i < _gridElem.VolumePoints.Length; _i++)
							{
								var _corner = _gridElem.VolumePoints[_i];
								
								if(_corner == null)
									continue;
								
								if(!_corner.TryGetComponent<Renderer>(out var _renderer))
									continue;
								
								if (_corner.TryGetComponent<MeshFilter>(out var _meshFilter))
								{
									if (_meshFilter.sharedMesh != null)
									{
										if (_meshFilter.sharedMesh.subMeshCount > 1)
										{
											_renderer.materials = new[]
											{
												blockMaterial, waterMaterial
											};
										}
										else
										{
											_renderer.material = waterMaterial;
										}
									}
								}
								
								EditorUtility.SetDirty(_corner);
							}
						}
					}
				}
				GUILayout.EndHorizontal();
			}
		}
	}
}
#endif
