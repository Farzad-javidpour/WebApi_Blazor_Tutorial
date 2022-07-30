using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Application.Settings;

namespace Infrastructure.Common.Wrappers
{
    public class FilterDto
    {
        public string? FilterString { get; set; }
        public PagingSetting? PagingSetting { get; set; }

        public FilterDto()
        {
            PagingSetting = new PagingSetting
            {
                PageSize = 2,
                PageIndex = 1
            };
        }
    }
}
