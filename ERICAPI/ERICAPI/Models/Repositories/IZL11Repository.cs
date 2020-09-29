using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ERICAPI.Models.Repositories
{
    public interface IZL11Repository
    {
        IEnumerable<VIEW_spare_All> Query(Dictionary<string, string> form);
    }
}
