using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MugCup_BlockBuilder
{
    [CreateAssetMenu(fileName = "PrototypeData", menuName = "ScriptableObjects/PrototypeDataObject", order = 8)]
    public class Module : ScriptableObject
    {
        public int Index;
        
        public string Name;

        public int BitMask;
        
        public Mesh MeshPrototype;

        public float Probability;
        public float PLogP;

        public int RotationIndex;

        public FaceDetails FaceDetails = new FaceDetails();
        
        public ModuleSet[] PossibleNeighbors;
        public Module[][] PossibleNeighborsArray;

        public void SetProbability(float _value)
        {
            Probability = _value;
        }
            
        public void CalculatePLogP()
        {
            PLogP = Probability * Mathf.Log(Probability);
        }

        public void CopyData(ModulePrototype _modulePrototype)
        {
            Index         = _modulePrototype.Index;
            Name          = _modulePrototype.Name;
            BitMask       = _modulePrototype.BitMask;
            MeshPrototype = _modulePrototype.MeshPrototype;
            RotationIndex = _modulePrototype.RotationIndex;
            FaceDetails   = _modulePrototype.FaceDetails;
        }

        public void StorePossibleNeighbors(List<Module> _allModuleInScene)
        {
            var _allModuleCount = _allModuleInScene.Count;
            
            PossibleNeighbors = new ModuleSet[6];

            foreach (ModuleFace _face in Enum.GetValues(typeof(ModuleFace)))
            {
                PossibleNeighbors[(int)_face] = new Func<ModuleSet>(() =>
                {
                    var _newModuleSet = new ModuleSet(_allModuleCount);

                    foreach (var _otherModule in _allModuleInScene)
                    {
                        if (CheckFaceIsFit(_face, _otherModule))
                            _newModuleSet.Add(_otherModule);
                    }

                    return _newModuleSet;
                })();
            }

            PossibleNeighborsArray = PossibleNeighbors.Select(_ms => _ms.ToArray()).ToArray();
        }

        private bool CheckFaceIsFit(ModuleFace _face, Module _otherModule)
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
    }
}
