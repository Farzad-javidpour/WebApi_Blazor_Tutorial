using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Application.Settings
{
    public class PagingSetting
    {
        public int PageSize { get; set; }
        public int PageIndex { get; set; }

        public PagingSetting()
        {
            PageSize = 2;
            PageIndex = 1;
        }
    }
}
