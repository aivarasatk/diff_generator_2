using DiffGenerator2.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiffGenerator2.Interfaces
{
    public interface INamingSchemeReader
    {
        IList<ColumnNamingScheme> GetColumnNamingScheme(string fileName); 
    }
}
