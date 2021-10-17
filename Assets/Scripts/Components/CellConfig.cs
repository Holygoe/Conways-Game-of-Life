using Unity.Entities;
using Unity.Mathematics;

namespace GameOfLife
{
    public readonly struct CellConfig : IComponentData
    {
        public readonly Entity CellPrefab;
        public readonly float4 AliveColor;
        public readonly float4 DeadColor;

        public CellConfig(Entity cellPrefab, float4 aliveColor, float4 deadColor)
        {
            CellPrefab = cellPrefab;
            AliveColor = aliveColor;
            DeadColor = deadColor;
        }
    }
}
