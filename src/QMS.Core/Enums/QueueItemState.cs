using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QMS.Core.Enums;

public enum QueueItemState
{
    Waiting = 0,
    Called = 1,
    Serving = 2,
    Completed = 3,
    Cancelled = 4,
    NoShow = 5,
    Transferred = 6
}
