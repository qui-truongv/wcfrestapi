using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QMS.Core.Enums
{
    public enum CacheType
    {
        /// <summary>
        /// Tất cả các loại Cache
        /// </summary>
        All = 0,

        /// <summary>
        /// Danh sách bệnh nhân đang đợi (QueueItems)
        /// </summary>
        QueueItem = 1,

        /// <summary>
        /// Danh mục hàng đợi/phòng khám (Queues)
        /// </summary>
        Queue = 2,

        /// <summary>
        /// Danh mục màn hình hiển thị (Screens)
        /// </summary>
        Screen = 3,

        /// <summary>
        /// Danh mục tham số hệ thống (Parameters)
        /// </summary>
        Parameter = 4,

        /// <summary>
        /// Danh mục quầy/máy trạm (Clients)
        /// </summary>
        Client = 5
    }
}
