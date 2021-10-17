using Unity.Entities;
using Unity.Mathematics;

namespace GameOfLife
{
    public readonly struct Grid : IComponentData
    {
        public const int MIN_SIZE = 3;
        public const int MAX_SIZE = 1000;
        
        public readonly int2 Size;
        public readonly int CellCount;
        public readonly float CellSize;
        private readonly float2 _maxGridPoint;

        public Grid(int2 size, float cellSize)
        {
            Size = size;
            CellCount = size.x * size.y;
            CellSize = cellSize;
            _maxGridPoint = (float2)size * cellSize / 2;
        }

        public float3 GetCellPosition(int2 cellIndex)
        {
            var halfCellSize = CellSize / 2;
            var maxCellPosition = new float3(_maxGridPoint - new float2(halfCellSize), 0);

            return new float3(cellIndex.x * CellSize, cellIndex.y * CellSize, 0) - maxCellPosition;
        }

        public int GetCellEntityIndex(int2 cellIndex)
        {
            return cellIndex.x + cellIndex.y * Size.x;
        }
        
        public int GetCellEntityIndex(int i, int j)
        {
            return i + j * Size.x;
        }

        public bool TryGetCellIndex(float2 point, out int2 cellIndex)
        {
            if (!Contains(point))
            {
                cellIndex = default;

                return false;
            }

            cellIndex = (int2) math.floor((point + _maxGridPoint) / CellSize);

            return true;
        }

        private bool Contains(float2 point)
        {
            return point.x < _maxGridPoint.x 
                   && point.y < _maxGridPoint.y 
                   && point.x >= -_maxGridPoint.x 
                   && point.y >= -_maxGridPoint.y;
        }
        
        public override string ToString()
        {
            return $"Grid(size: {Size.x}, {Size.y}; cell count: {CellCount}; cell size: {CellSize}; maximum grid point: {_maxGridPoint.x}, {_maxGridPoint.y})";
        }
    }
}