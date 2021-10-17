using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace GameOfLife
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(ConvertToEntity))]
    public class GameInitializer : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
    {
        [SerializeField] private GameObject cellPrefab;
        
        [SerializeField] private Color aliveColor;
        
        [SerializeField] private Color deadColor;

        [SerializeField] private float cellSize;
        
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new CellConfig(
                conversionSystem.GetPrimaryEntity(cellPrefab),
                new float4(aliveColor.r, aliveColor.g, aliveColor.b, aliveColor.a),
                new float4(deadColor.r, deadColor.g, deadColor.b, deadColor.a)));

            var gridBuildingSystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<GridBuildingSystem>();
            dstManager.AddComponentData(entity, gridBuildingSystem.InitiateGrid(cellSize));
        }

        public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
        {
            referencedPrefabs.Add(cellPrefab);
        }
    }
}
