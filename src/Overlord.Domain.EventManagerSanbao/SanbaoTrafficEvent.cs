using Overlord.Domain.Event;

namespace Overlord.Domain.EventManagerSanbao
{
    public class SanbaoTrafficEvent : TrafficEvent
    {
        // 唯一编号(41 位)
        // 格式：E+17 位YYYYMMDDhhmmsszzz 毫秒级时间字符串
        // +20位唯一设备编号（可用数字和字母，不足 20 位则左方补够 0，如果大于 20 位则截取后 20 位）
        // +3 位（000~999 轮转编号）
        public string ID { get; set; }

        // 目标编号
        public long ObjId { get; set; }

        // 事件开始结束标志 0：开始，1：结束
        public int EvtBeginEnd { get; set; }

        // 事件类型
        // 2	        车辆慢速
        // 4    	    车辆超速
        // 8        	车辆压线
        // 16	    异常变道
        // 64	    禁行闯入
        // 128	    车辆停驶
        // 256	    车辆逆行
        // 512	    抛洒物
        // 1024	    道路拥堵
        // 2048	    行人闯入
        // 4096	    大货车禁行
        // 8192	    应急车道占道
        // 16384    	非机动车
        // 32768    	非法上下客
        // 65536	    施工区域
        // 131072	交通事故
        // 524288	排队超限
        // 10000    	车辆倒车
        // 10001	    道路缓行
        // 10002	    道路救援
        // 10003	    危险品车辆
        // 10004    	货车占用客车道
        // 10005	    环境异常
        // 10006	    相机移动 
        public int EvtType { get; set; }

        // 事件发生位置，单位米
        public float PosX { get; set; }

        // 事件发生位置，单位米
        public float PosY { get; set; }

        // 经度(有效值为小数点后 6 位)
        public float Lon { get; set; }

        // 纬度(有效值为小数点后 6 位)
        public float Lat { get; set; }

        // 车牌号码“未识别”代表未识别出结果，“无牌车”代表无牌车
        public string PlateNo { get; set; }

        // 号牌类型分为农用车辆号牌、使馆车辆号牌、临时号牌等。无法区分，默认为空(暂时不支持)
        public int PlateType { get; set; }

        // 车牌颜色
        // 1	 蓝色
        // 2	 黑色
        // 3 黄色
        // 4	 白色
        // 5 绿色
        // 6	 其他
        public int PlateColor { get; set; }

        // 机动车类型
        // 0	    SUV
        // 1	    MPV
        // 2	    中型汽车
        // 3	    大型汽车
        // 4	    其他车型
        // 5	    轿车
        // 6	    摩托车
        // 7	    小客车
        // 8	    客车
        // 9	    小货车
        // 10	货车
        // 11	大货车 
        public int VehicleType { get; set; }

        // 车辆颜色
        // 1    	蓝色
        // 2	    绿色
        // 3	    黑色
        // 4	    紫色
        // 5	    红色
        // 6    	黄色
        // 7    	白色
        // 8    	灰色
        // 9    	棕色
        // 10	粉色
        // 11	其他 
        public int VehicleColor { get; set; }

        // 车牌样式，1 单行；2：双行；3 其他
        public int PlateStyle { get; set; }

        // 排队时间单位 s(EvtType 为 0x400 或0x80000 时均有效)
        public float QueueLength { get; set; }

        // 排队时间单位 s(EvtType 为 0x400 或0x80000 时均有效)
        public float QueueTime { get; set; }

        // 图片类型 0：图片路径 1：base64
        public int EventImageType { get; set; }

        // 事件图片大小，当 EventImageType 为 1 时有效
        public int EventImageSize { get; set; }

        // 事件图片内容，图片上用空三角形标识事件发生点位
        public string EventImage { get; set; }

        // 违法抓拍图片路径
        public string EventImagePath { get; set; }

        // 违法抓拍图片路径 2(部分违法事件提供)
        public string EventImagePath2 { get; set; }

        // 违法抓拍图片路径 3(部分违法事件提供)
        public string EventImagePath3 { get; set; }

        // 违法抓拍图片路径 4(部分违法事件提供)
        public string EventImagePath4 { get; set; }

        // 违法录像路径
        public string EventVideoPath { get; set; }

        public SanbaoTrafficEvent()
        {
            PlateNo = "未识别";
            PlateColor = 6;
            VehicleColor = 11;
            PlateStyle = 1;
            EventImageType = 0;
        }
    }
}
