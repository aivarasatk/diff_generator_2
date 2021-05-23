using DiffGenerator2.DTOs;
using DiffGenerator2.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace DiffGenerator2.Services
{
    public class OrderDiffGenerator : IOrderDiffGenerator
    {
        public OrderReport InclusionReport(ICollection<int> orderNums, int lowerBound, int upperBound)
        {
            var ordersInterval = Enumerable.Range(lowerBound, upperBound - lowerBound + 1);
            var ordersInInterval = orderNums.Where(order => order >= lowerBound && order <= upperBound);

            return new OrderReport()
            {
                BelowLowerBound = orderNums.Where(order => order < lowerBound).OrderBy(o => o),
                MissingInInterval = ordersInterval.Except(ordersInInterval),
                AboveUpperBound = orderNums.Where(order => order > upperBound).OrderBy(o => o)
            };
        }
    }
}
