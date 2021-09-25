using System.Collections.Generic;

namespace Overlord.Domain.EventManagerSanbao
{
    public class SanbaoLaneEventInfo
    {
        // 车道号
        public int ChannelId { get; set; }

        // 摄像机朝向
        public int Direction { get; set; }

        // 行驶方向
        public int DriveWay { get; set; }

        // 事件类信息列表
        public List<SanbaoTrafficEvent> Evt_List { get; set; }

        public SanbaoLaneEventInfo()
        {
            Evt_List = new List<SanbaoTrafficEvent>();
        }
    }
}