using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderIngestion.Application.Enums
{
    public enum InsertResult
    {
        Success=0,
        Duplicate=1,
        Error=2
    }
}
