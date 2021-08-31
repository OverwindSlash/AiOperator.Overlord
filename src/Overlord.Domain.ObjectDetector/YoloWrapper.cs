using OpenCvSharp;
using Overlord.Core.Entities.Frame;
using Overlord.Domain.Interfaces;
using Overlord.Domain.ObjectDetector.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Overlord.Domain.ObjectDetector
{
    public class YoloWrapper : IObjectDetector, IDisposable
    {
        private const string YoloLibraryWin = @"Library/win/darknet.dll";
        private const string YoloLibraryLinux = @"Library/linux/darknet.so";
        private const string YoloLibrary = YoloLibraryWin;

        private const string CudaPath = @"CUDA_PATH";

        private const string CudnnFileWin = @"cudnn64_8.dll";
        private const string CudnnFileLinux = @"cudnn64_8.so";
        private const string CudnnFile = CudnnFileWin;

        private readonly Dictionary<int, string> _objectType = new Dictionary<int, string>();

        public CudaEnvironment CudaEnv { get; private set; }

        #region DllImport Gpu
        [DllImport(YoloLibrary, EntryPoint = "init")]
        internal static extern int InitializeYolo(string configurationFilename, string weightsFilename, int gpu);

        [DllImport(YoloLibrary, EntryPoint = "detect_image")]
        internal static extern int DetectImage(string filename, ref BboxContainer container);

        [DllImport(YoloLibrary, EntryPoint = "detect_mat_data")]
        internal static extern int DetectMatData(IntPtr pArray, int nSize, ref BboxContainer container, float thresh);

        [DllImport(YoloLibrary, EntryPoint = "detect_mat_ptr")]
        internal static extern int DetectMatPtr(IntPtr pArray, int nSize, ref BboxContainer container, float thresh);

        [DllImport(YoloLibrary, EntryPoint = "set_tracking_thresh")]
        internal static extern int SetTrackingThresh(bool change_history, int frames_story, int max_dist);

        [DllImport(YoloLibrary, EntryPoint = "get_device_count")]
        internal static extern int GetDeviceCount();

        [DllImport(YoloLibrary, EntryPoint = "get_device_name", CharSet = CharSet.Ansi)]
        internal static extern int GetDeviceName(int gpu, StringBuilder deviceName);

        [DllImport(YoloLibrary, EntryPoint = "dispose")]
        internal static extern int DisposeYolo();
        #endregion

        public YoloWrapper(YoloConfiguration config)
        {
            Initialize(config.ConfigFile, config.WeightsFile, config.NamesFile);
        }

        private void Initialize(string configurationFilename, string weightsFilename, string namesFilename, int gpuIndex = 0)
        {
            if (IntPtr.Size != 8)
            {
                throw new NotSupportedException("Only 64-bit processes are supported");
            }

            CheckCudaEnvironment(gpuIndex);

            GetObjectNames(namesFilename);

            InitializeYolo(configurationFilename, weightsFilename, gpuIndex);
        }

        private void CheckCudaEnvironment(int gpuIndex)
        {
            CudaEnv = new CudaEnvironment();

            var envirormentVariables = Environment.GetEnvironmentVariables(EnvironmentVariableTarget.Machine);
            if (envirormentVariables.Contains(CudaPath))
            {
                CudaEnv.CudaExists = true;

                string cudaPath = Environment.GetEnvironmentVariable(CudaPath);
                string cudnnPath = Path.Combine(cudaPath, "bin");
                string cudnnFile = Path.Combine(cudnnPath, CudnnFile);

                if (File.Exists(cudnnFile))
                {
                    CudaEnv.CudnnExists = true;
                }
            }

            GetVideoCardInfo(gpuIndex);
        }

        private void GetVideoCardInfo(int gpuIndex)
        {
            var deviceCount = GetDeviceCount();
            if (deviceCount == 0)
            {
                throw new NotSupportedException("No graphic device is available");
            }

            if (gpuIndex > (deviceCount - 1))
            {
                throw new IndexOutOfRangeException("Graphic device index is out of range");
            }

            var deviceName = new StringBuilder(256);
            GetDeviceName(gpuIndex, deviceName);
            CudaEnv.GraphicDeviceName = deviceName.ToString();
        }

        private void GetObjectNames(string namesFilename)
        {
            var lines = File.ReadAllLines(namesFilename);
            for (var i = 0; i < lines.Length; i++)
            {
                _objectType.Add(i, lines[i]);
            }
        }

        public List<TrafficObjectInfo> Detect(Mat image, float thresh)
        {
            var container = new BboxContainer();

            try
            {
                int count = DetectMatPtr(image.CvPtr, image.Width * image.Height, ref container, thresh);

                if (count == -1)
                {
                    throw new NotImplementedException("darknet dll compiled incorrectly");
                }
            }
            catch (Exception)
            {
                return new List<TrafficObjectInfo>();
            }

            IEnumerable<YoloItem> result = Convert(container);

            return ConvertToTrafficObjectInfo(result);
        }

        public List<TrafficObjectInfo> Detect(byte[] imageData, float thresh = 0.7F)
        {
            var container = new BboxContainer();

            var gcHandle = GCHandle.Alloc(imageData, GCHandleType.Pinned);
            IntPtr pnt = gcHandle.AddrOfPinnedObject();

            try
            {
                int count = DetectMatData(pnt, imageData.Length, ref container, thresh);

                if (count == -1)
                {
                    throw new NotImplementedException("darknet library compiled incorrectly");
                }
            }
            catch (Exception)
            {
                return new List<TrafficObjectInfo>();
            }
            finally
            {
                // Free the unmanaged memory.
                gcHandle.Free();
            }

            IEnumerable<YoloItem> result = Convert(container);

            return ConvertToTrafficObjectInfo(result);
        }

        private IEnumerable<YoloItem> Convert(BboxContainer container)
        {
            var yoloItems = new List<YoloItem>();
            foreach (var item in container.candidates.Where(o => o.h > 0 || o.w > 0))
            {
                if (!_objectType.TryGetValue((int)item.obj_id, out var objectType))
                {
                    objectType = "Unknown";
                }

                var yoloItem = new YoloItem
                {
                    TypeId = (int)item.obj_id,
                    X = (int)item.x,
                    Y = (int)item.y,
                    Height = (int)item.h,
                    Width = (int)item.w,
                    Confidence = item.prob,
                    Type = objectType,
                    TrackingId = item.track_id
                };

                yoloItems.Add(yoloItem);
            }

            return yoloItems;
        }

        private List<TrafficObjectInfo> ConvertToTrafficObjectInfo(IEnumerable<YoloItem> result)
        {
            List<TrafficObjectInfo> tois = new List<TrafficObjectInfo>();

            foreach (YoloItem item in result)
            {
                TrafficObjectInfo toi = new TrafficObjectInfo();
                toi.TypeId = item.TypeId;
                toi.Type = item.Type;
                toi.TrackingId = item.TrackingId;
                toi.Confidence = item.Confidence;
                toi.X = item.X;
                toi.Y = item.Y;
                toi.Width = item.Width;
                toi.Height = item.Height;

                tois.Add(toi);
            }

            return tois;
        }

        public int SetTrackingParam(bool change_history, int frames_story, int max_dist)
        {
            return SetTrackingThresh(change_history, frames_story, max_dist);
        }

        public void Dispose()
        {
            ReleaseUnmanagedResources();
            GC.SuppressFinalize(this);
        }

        private void ReleaseUnmanagedResources()
        {
            DisposeYolo();
        }

        ~YoloWrapper()
        {
            ReleaseUnmanagedResources();
        }
    }
}
