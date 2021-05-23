using DiffGenerator2.DTOs;
using System.Collections.Generic;

namespace DiffGenerator2.Interfaces
{
    public interface IOrderDiffGenerator
    {
        OrderReport InclusionReport(ICollection<int> orderNums, int lowerBound, int upperBound);
    }
}
