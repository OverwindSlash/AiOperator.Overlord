using System;
using Overlord.Domain.Event;
using Overlord.Domain.Interfaces;

namespace Overlord.Domain.EventManagerSanbao
{
    public class SanbaoEventGenerator : ITrafficEventGenerator
    {
        private SanbaoTrafficEvent CreateBasicTrafficEvent(string deviceNo, int laneIndex, int typeId, long trackingId)
        {
            var trafficEvent = new SanbaoTrafficEvent();
            var id = GenerateId(deviceNo);

            trafficEvent.DeviceNo = deviceNo;
            trafficEvent.LaneIndex = laneIndex;
            trafficEvent.ID = id;
            trafficEvent.ObjId = trackingId;
            trafficEvent.VehicleType = GenerateVehicleType(typeId);
            return trafficEvent;
        }
        
        public TrafficEvent CreateForbiddenEvent(string deviceNo, int laneIndex, int typeId, long trackingId)
        {
            var trafficEvent = CreateBasicTrafficEvent(deviceNo, laneIndex, typeId, trackingId);
            trafficEvent.EvtType = 64;

            return trafficEvent;
        }

        public TrafficEvent CreateStoppedEvent(string deviceNo, int laneIndex, int typeId, long trackingId)
        {
            var trafficEvent = CreateBasicTrafficEvent(deviceNo, laneIndex, typeId, trackingId);
            trafficEvent.EvtType = 128;

            return trafficEvent;
        }

        public TrafficEvent CreateSlowEvent(string deviceNo, int laneIndex, int typeId, long trackingId)
        {
            var trafficEvent = CreateBasicTrafficEvent(deviceNo, laneIndex, typeId, trackingId);
            trafficEvent.EvtType = 2;

            return trafficEvent;
        }

        public TrafficEvent CreateAmbleEvent(string deviceNo, int laneIndex, int typeId, long trackingId)
        {
            var trafficEvent = CreateBasicTrafficEvent(deviceNo, laneIndex, typeId, trackingId);
            trafficEvent.EvtType = 10001;

            return trafficEvent;
        }

        public TrafficEvent CreateJamEvent(string deviceNo, int laneIndex, int typeId, long trackingId)
        {
            var trafficEvent = CreateBasicTrafficEvent(deviceNo, laneIndex, typeId, trackingId);
            trafficEvent.EvtType = 1024;

            return trafficEvent;
        }

        public TrafficEvent CreateReverseEvent(string deviceNo, int laneIndex, int typeId, long trackingId)
        {
            var trafficEvent = CreateBasicTrafficEvent(deviceNo, laneIndex, typeId, trackingId);
            trafficEvent.EvtType = 10000;

            return trafficEvent;
        }

        private string GenerateId(string deviceNo)
        {
            string timestamp = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            string prefix = "00000000000000000000";
            string cameraId = prefix + deviceNo;
            cameraId = cameraId.Substring(cameraId.Length - 20, 20);
            string id = $"E{timestamp}{cameraId}000";
            return id;
        }

        private int GenerateVehicleType(int typeId)
        {
            int vehicleType;
            switch (typeId)
            {
                case 1:
                    vehicleType = 6;
                    break;
                case 2:
                    vehicleType = 5;
                    break;
                case 3:
                    vehicleType = 6;
                    break;
                case 5:
                    vehicleType = 8;
                    break;
                case 6:
                    vehicleType = 11;
                    break;
                case 7:
                    vehicleType = 10;
                    break;
                default:
                    vehicleType = 4;
                    break;
            }

            return vehicleType;
        }

        
    }
}
