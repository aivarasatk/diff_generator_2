using DiffGenerator2.DTOs;

namespace DiffGenerator2.Interfaces
{
    public interface IReportGenerator
    {
        void GenerateDiffReport(DiffReport diffReport);
        void GenerateOrderReport(OrderReport orderReport);
    }
}
