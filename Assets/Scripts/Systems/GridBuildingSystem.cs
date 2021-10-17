using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace GameOfLife
{
    public class GridBuildingSystem : SystemBase
    {
        private const string GRID_WIDTH_PLAYER_PREF = "Grid Width";
        private const string GRID_HEIGHT_PLAYER_PREF = "Grid Height";
        
        private EntityCommandBufferSystem _commandBufferSystem;
        private NativeArray<Entity> _cells;

        public int2 GridSize
        {
            get
            {
                var width = PlayerPrefs.GetInt(GRID_WIDTH_PLAYER_PREF, Grid.MIN_SIZE);
                var height = PlayerPrefs.GetInt(GRID_HEIGHT_PLAYER_PREF, Grid.MIN_SIZE);

                return new int2(width, height);
            }
            
            set
            {
                PlayerPrefs.SetInt(GRID_WIDTH_PLAYER_PREF, value.x);
                PlayerPrefs.SetInt(GRID_HEIGHT_PLAYER_PREF, value.y);
                PlayerPrefs.Save();

                var grid = GetSingleton<Grid>();
                grid = new Grid(value, grid.CellSize);
                SetSingleton(grid);
            }
        }

        public float Fullness { get; set; }

        public event Action<int2, float> GridBuilt;
        
        public Grid InitiateGrid(float cellSize)
        {
            return new Grid(GridSize, cellSize);
        }

        protected override void OnCreate()
        {
            RequireSingletonForUpdate<EditModeTag>();
        }

        protected override void OnStartRunning()
        {
            if (!_cells.IsCreated)
            {
                BuildGrid();
            }
        }

        public void BuildGrid()
        {
            if (_cells.IsCreated)
            {
                EntityManager.DestroyEntity(_cells);
                _cells.Dispose();
            }
            
            var cellConfig = GetSingleton<CellConfig>();
            var grid = GetSingleton<Grid>();

            _commandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
            
            var cells = new NativeArray<Entity>(grid.CellCount, Allocator.TempJob);
            EntityManager.Instantiate(cellConfig.CellPrefab, cells);
            
            _cells = new NativeArray<Entity>(cells.Length, Allocator.Persistent);
            _cells.CopyFrom(cells);

            var commandBuffer = _commandBufferSystem.CreateCommandBuffer().AsParallelWriter();
            var random = new Random(15);
            var fullness = Fullness;

            var directions = new FixedList128<int2>
            {
                new int2(-1, 1), new int2(0, 1), new int2(1, 1), new int2(1, 0),
                new int2(1, -1), new int2(0, -1), new int2(-1, -1),
                new int2(-1, 0)
            };

            Job.WithCode(() =>
            {
                for (var i = 0; i < grid.Size.x; i++)
                {
                    for (var j = 0; j < grid.Size.y; j++)
                    {
                        var cellIndex = new int2(i, j);
                        var entityIndex = grid.GetCellEntityIndex(cellIndex);
                        var isAlive = random.NextFloat() <= fullness;
                        
                        commandBuffer.SetComponent(
                            0, 
                            cells[entityIndex], 
                            new Translation { Value = grid.GetCellPosition(cellIndex) });
                        
                        commandBuffer.SetComponent(
                            0, 
                            cells[entityIndex], 
                            new URPMaterialPropertyBaseColor { Value = isAlive ? cellConfig.AliveColor : cellConfig.DeadColor});
                        
                        commandBuffer.SetComponent(
                            0,
                            cells[entityIndex],
                            new CellState { IsAlive = isAlive }
                            );
                        
                        var neighbours = new FixedList128<Entity>();
                        
                        foreach (var direction in directions)
                        {
                            var neighbourIndex = GetNeighbourIndex(cellIndex, direction, grid.Size);
                            var entityNeighbourIndex = grid.GetCellEntityIndex(neighbourIndex);
                            neighbours.Add(cells[entityNeighbourIndex]);
                        }
                        
                        commandBuffer.SetComponent(
                            0, 
                            cells[grid.GetCellEntityIndex(i, j)], 
                            new CellNeighbors { Value = neighbours } );
                    }
                }
            }).Schedule();

            _commandBufferSystem.AddJobHandleForProducer(Dependency);
            cells.Dispose(Dependency);
            
            GridBuilt?.Invoke(grid.Size, grid.CellSize);
        }
        
        protected override void OnUpdate()
        {
            var clickedCell = GetSingleton<ClickedCell>();
            
            if (clickedCell.Equals(ClickedCell.None)) return;
            
            var grid = GetSingleton<Grid>();
            var cellConfig = GetSingleton<CellConfig>();
            
            var cell = _cells[grid.GetCellEntityIndex(clickedCell.CellIndex)];
            var cellState = EntityManager.GetComponentData<CellState>(cell).Inverse();
            var baseColor = new URPMaterialPropertyBaseColor { Value = cellState.IsAlive ? cellConfig.AliveColor : cellConfig.DeadColor };
            
            EntityManager.SetComponentData(cell, cellState);
            EntityManager.SetComponentData(cell, baseColor);
        }

        protected override void OnDestroy()
        {
            _cells.Dispose();
        }

        private static int2 GetNeighbourIndex(int2 current, int2 direction, int2 gridSize)
        {
            var index = current + direction;
            
            Repeat(0);
            Repeat(1);

            return index;
            
            void Repeat(int axis)
            {
                if (index[axis] < 0)
                {
                    index[axis] = gridSize[axis] - 1;
                }
                else if (index[axis] >= gridSize[axis])
                {
                    index[axis] = 0;
                }
            }
        }
    }
}