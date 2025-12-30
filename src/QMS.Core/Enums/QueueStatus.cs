using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace QMS.Core.Enums
{
    [DataContract]
    public enum QueueStatus
    {
        [EnumMember]
        Pause = 0, //Tạm ngừng gọi/thêm số thứ tự vào hàng đợi
        [EnumMember]
        Ongoing = 1//Đang thực hiện        
    }
}
