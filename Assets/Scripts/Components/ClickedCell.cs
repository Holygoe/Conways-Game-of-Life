using System;
using Unity.Entities;
using Unity.Mathematics;

namespace GameOfLife
{
    public struct ClickedCell : IComponentData, IEquatable<ClickedCell>
    {
        public int2 CellIndex;
        
        public static ClickedCell None = new ClickedCell { CellIndex = new int2(-1, -1) };

        public bool Equals(ClickedCell other)
        {
            return CellIndex.Equals(other.CellIndex);
        }
    }
}