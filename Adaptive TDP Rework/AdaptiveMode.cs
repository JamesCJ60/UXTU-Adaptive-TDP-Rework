using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Adaptive_TDP_Rework
{
    internal class AdaptiveMode
    {
        private static int tdp = 0;
        private static int gfxClock = 0;
        private static int fpsLimit = 0;
        private static bool isFpsLimit = false;

        private static int temp = 0;
        private static int tempLimit = 0;
        private static int gfxUsage = 0;

        private static int cpuClock = 0;
        private static int cpuUsage = 0;

        private static double avGfxClock = 0;
        private static double avFps = 0;
        private static double avTdp = 0;

        private static List<int> tdpList = new List<int>();
        private static List<int> gfxClockList = new List<int>();
        private static List<int> fpsList = new List<int>();

        //Get average TDP limit
        public static double GetAvTdp()
        {
            return avTdp;
        }

        //Get average GFX clock
        public static double GetAvGfxClock()
        {
            return avGfxClock;
        }

        //Get average FPS
        public static double GetAvFps()
        {
            return avFps;
        }

        //Get FPS limit value
        public static int GetFpsLimit()
        {
            return fpsLimit;
        }

        //Get FPS limit state
        public static bool GetFpsLimitState()
        {
            return isFpsLimit;
        }

        //Update average TDP limit value
        private static void UpdateAvTdp(int _tdp)
        {
            tdpList.Add(_tdp);

            if (tdpList.Count > 10)
            {
                avTdp = CalculateAverage(tdpList);
                tdpList.Clear();
                tdpList.Add((int)avTdp);
            }

            avTdp = CalculateAverage(tdpList);
        }

        //Update average GFX clock value
        private static void UpdateAvGfxClock(int _gfxClock)
        {
            gfxClockList.Add(_gfxClock);

            if (gfxClockList.Count > 10)
            {
                avGfxClock = CalculateAverage(gfxClockList);
                gfxClockList.Clear();
                gfxClockList.Add((int)avGfxClock);
            }

            avGfxClock = CalculateAverage(gfxClockList);
        }

        //Update average fps value
        private static void UpdateAvFps(int fps = 0)
        {
            fpsList.Add(fps);

            if (fpsList.Count > 10)
            {
                avFps = CalculateAverage(fpsList);
                fpsList.Clear();
                fpsList.Add((int)avFps);
            }

            avFps = CalculateAverage(fpsList);
        }

        //Average calucation
        private static double CalculateAverage(List<int> dataList)
        {
            if (dataList.Count == 0)
                return 0;

            int total = 0;
            foreach (int data in dataList)
            {
                total += data;
            }
            return (double)total / dataList.Count;
        }

        //Update fps limit state and limit value
        public static void UpdateFpsLimit(bool _isFpsLimit = false, int _fpsLimit = 0)
        {
            isFpsLimit = isFpsLimit;
            fpsLimit = _fpsLimit;
        }

        //Update stats used by alogos to calulate new values 
        public static void UpdateStats(int _temp, int _tempLimit, int _gfxUsage, int _cpuClock, int _cpuUsage, int _fps)
        {
            temp = _temp;
            tempLimit = _tempLimit;
            gfxUsage = _gfxUsage;
            cpuClock = _cpuClock;
            cpuUsage = _cpuUsage;
            UpdateAvFps(_fps);
        }

        //Calulate new TDP limit
        public static int UpdateTDP(int minTdp, int maxTdp)
        {
            if (tdp == 0) tdp = maxTdp;

            if (temp >= tempLimit || isFpsLimit && avFps >= fpsLimit -1) tdp--;
            else if (temp < tempLimit - 5 || avFps < fpsLimit - 2) tdp++;

            if (tdp > maxTdp) tdp = maxTdp;
            if(tdp < minTdp) tdp = minTdp;

            UpdateAvTdp(tdp);

            return tdp;
        }

        //Calulate new GFX clock
        public static int UpdateGfxClock(int minGfx, int maxGfx, int minCpuClock)
        {
            if (gfxClock == 0) gfxClock = (int)((minGfx + maxGfx) / 2);

            if (temp >= tempLimit || gfxUsage < 93 || cpuClock < minCpuClock || isFpsLimit && avFps >= fpsLimit - 1) gfxClock -= 50;
            else if (temp < tempLimit - 5 || gfxUsage > 95 || cpuClock >= minCpuClock || avFps < fpsLimit - 2) gfxClock += 50;

            if (gfxClock > maxGfx) gfxClock = maxGfx;
            if (gfxClock < minGfx) gfxClock = minGfx;

            UpdateAvGfxClock(gfxClock);

            return gfxClock;
        }
    }
}
