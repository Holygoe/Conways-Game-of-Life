using Unity.Entities;

namespace GameOfLife
{
    [GenerateAuthoringComponent]
    public struct CellState : IComponentData
    {
        public bool IsAlive;

        public CellState Inverse()
        {
            IsAlive = !IsAlive;

            return this;
        }
    }
}