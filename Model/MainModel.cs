using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DiffGenerator2.Model
{
    public partial class MainModel
    {
        private string _excelFileName;
        private string _eipFileName;
        private bool _executeEnabled;

        public ICommand SelectExcelFileCommand { get; set; }
        public ICommand SelectEipFileCommand { get; set; }
        public ICommand ExecuteCommand { get; set; }

    }
}
