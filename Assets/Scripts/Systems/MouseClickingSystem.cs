using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace GameOfLife
{
    public class MouseClickingSystem : SystemBase
    {
        private Camera _camera;

        protected override void OnCreate()
        {
            _camera = Camera.main;
            var clickedCellEntity = EntityManager.CreateEntity(typeof(ClickedCell));
            EntityManager.SetComponentData(clickedCellEntity, ClickedCell.None);
            
            RequireSingletonForUpdate<EditModeTag>();
        }

        protected override void OnUpdate()
        {
            var mouse = Mouse.current;

            if (!mouse.leftButton.wasPressedThisFrame || EventSystem.current.IsPointerOverGameObject())
            {
                SetSingleton(ClickedCell.None);
                
                return;
            }
            
            var worldPoint = _camera.ScreenToWorldPoint(mouse.position.ReadValue());
            var clickPoint = new float2(worldPoint.x, worldPoint.y);
            var grid = GetSingleton<Grid>();

            var clickedCell = grid.TryGetCellIndex(clickPoint, out var cellIndex)
                ? new ClickedCell { CellIndex = cellIndex }
                : ClickedCell.None;

            SetSingleton(clickedCell);
        }
    }
}
