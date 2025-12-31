using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QMS.Application.Interfaces
{
    public interface ISystemService
    {
        string GetValueOfParameter(string code, string defaultValue = "");
    }
}
