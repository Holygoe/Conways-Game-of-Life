using Unity.Entities;
using Unity.Rendering;

namespace GameOfLife
{
    public class SimulatingSystem : SystemBase
    {
        private const double STEP_TIMESPAN = 0.02f;

        private double _updateTimestamp;
        
        protected override void OnCreate()
        {
            RequireSingletonForUpdate<SimulationModeTag>();
        }

        protected override void OnUpdate()
        {
            if (Time.ElapsedTime < _updateTimestamp) return;
            
            _updateTimestamp = Time.ElapsedTime + STEP_TIMESPAN;
            
            Entities.ForEach((ref CellNeighbors cellNeighbors) =>
            {
                var entityCellState = GetComponentDataFromEntity<CellState>(true);
                
                var aliveCount = 0;
                
                foreach (var neighbour in cellNeighbors.Value)
                {
                    var cellState = entityCellState[neighbour];
                    
                    if (cellState.IsAlive)
                    {
                        aliveCount++;
                    }
                }

                cellNeighbors.AliveCount = aliveCount;
            }).ScheduleParallel();
            
            var cellConfig = GetSingleton<CellConfig>();
            
            Entities.ForEach((
                ref CellState cellState, 
                ref URPMaterialPropertyBaseColor baseColor,
                in CellNeighbors cellNeighbors) =>
            {
                if (cellState.IsAlive)
                {
                    if (cellNeighbors.AliveCount == 2 || cellNeighbors.AliveCount == 3) return;
                    
                    cellState.IsAlive = false;
                    baseColor.Value = cellConfig.DeadColor;
                }
                else if (cellNeighbors.AliveCount == 3)
                {
                    cellState.IsAlive = true;
                    baseColor.Value = cellConfig.AliveColor;
                }
            }).ScheduleParallel();
        }
    }
}