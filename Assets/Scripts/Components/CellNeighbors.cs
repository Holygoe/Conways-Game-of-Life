using Unity.Collections;
using Unity.Entities;

namespace GameOfLife
{
    [GenerateAuthoringComponent]
    public struct CellNeighbors : IComponentData
    {
        public FixedList128<Entity> Value;

        public int AliveCount;
    }
}