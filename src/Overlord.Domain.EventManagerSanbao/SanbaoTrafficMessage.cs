using System;
using System.Collections.Generic;
using Overlord.Domain.Event;

namespace Overlord.Domain.EventManagerSanbao
{
    public class SanbaoTrafficMessage
    {
        private static DateTime _startTime = new System.DateTime(1970, 1, 1, 0, 0, 0, 0);

        // 消息类型：取值为：1001
        public int MsgType { get; set; }

        // 相机编号
        public string DevNo { get; set; }

        // 设备类型
        public int DeviceType { get; set; }

        // 时间戳(13 位，精确到毫秒)
        public long Timestamp { get; set; }

        // 车道事件信息列表
        public List<SanbaoLaneEventInfo> ChannelEvtInfo { get; set; }

        public SanbaoTrafficMessage()
        {
            ChannelEvtInfo = new List<SanbaoLaneEventInfo>();
        }

        public static SanbaoTrafficMessage GenerateTrafficMessage(string deviceNo, int toiLaneIndex, SanbaoTrafficEvent trafficEvent)
        {
            SanbaoLaneEventInfo laneEventInfo = new SanbaoLaneEventInfo();
            laneEventInfo.ChannelId = toiLaneIndex;
            laneEventInfo.Evt_List.Add(trafficEvent);

            SanbaoTrafficMessage trafficMessage = new SanbaoTrafficMessage();
            trafficMessage.ChannelEvtInfo.Add(laneEventInfo);
            trafficMessage.MsgType = 1001;
            trafficMessage.DevNo = deviceNo;
            trafficMessage.DeviceType = 1001;

            trafficMessage.Timestamp = (DateTime.Now.Ticks - _startTime.Ticks) / 10000;

            return trafficMessage;
        }
    }
}