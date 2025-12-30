using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QMS.Application.DTOs.Screen
{
    public class QueueDisplayItemDto
    {
        public int QueueId { get; set; }
        public string QueueName { get; set; } = string.Empty;
        public List<QueueItemDisplayDto> Items { get; set; } = new();
    }

}
