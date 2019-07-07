using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiffGenerator2.Interfaces
{
    public interface IFileSelector
    {
        string GetSelectedFile(string filter);
    }
}
