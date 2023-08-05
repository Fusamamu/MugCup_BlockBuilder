using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MugCup_BlockBuilder
{
    //Might need custom editor to display some extra info like bit into binary
    [CreateAssetMenu(fileName = "PrototypeData", menuName = "ScriptableObjects/PrototypeDataObject", order = 8)]
    public class Module : ScriptableObject, IModule
    {
        [field: SerializeField] public string Name { get; private set; }
        
        [field: SerializeField] public int Index    { get; set; }
        [field: SerializeField] public int BitMask  { get; private set; }
        [field: SerializeField] public string MetaData { get; private set; }
        
        [field: SerializeField] public Mesh MeshPrototype { get; private set; }

        [field: SerializeField] public float Probability { get; private set; }
        [field: SerializeField] public float PLogP       { get; private set; }

        [field: SerializeField] public int RotationIndex { get; private set; }

        [field: SerializeField] public FaceDetails FaceDetails { get; private set; } = new FaceDetails();
        
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
            MetaData      = _modulePrototype.MetaData;
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
