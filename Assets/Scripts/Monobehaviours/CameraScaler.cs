using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace GameOfLife
{
    [RequireComponent(typeof(Camera))]
    public class CameraScaler : MonoBehaviour
    {
        private const float PADDING = 0.05f;
        
        private Camera _camera;
        private GridBuildingSystem _buildingSystem;
        
        private void Awake()
        {
            _camera = GetComponent<Camera>();
            _buildingSystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<GridBuildingSystem>();
        }

        private void OnEnable()
        {
            if (_buildingSystem.TryGetSingleton<Grid>(out var grid))
            {
                UpdateCameraSize(grid.Size, grid.CellSize);
            }
            
            _buildingSystem.GridBuilt += UpdateCameraSize;
        }

        private void OnDisable()
        {
            _buildingSystem.GridBuilt -= UpdateCameraSize;
        }

        private void UpdateCameraSize(int2 gridSize, float cellSize)
        {
            var size = (float2)gridSize * cellSize;
            var aspect = _camera.aspect;
            var orthographicSize = size.x / size.y > aspect ? size.x / (2 * aspect) : size.y / 2;
            _camera.orthographicSize = orthographicSize * (1 + PADDING);
        }
    }
}