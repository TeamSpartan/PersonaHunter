using System;
using System.Collections.Generic;

namespace SgLibUnite
{
    public class DisplaySettingDataContainer
    {
        private Dictionary<string, Tuple<int, int>> _resolutionList = new Dictionary<string, Tuple<int, int>>()
        {
            { "Full HD", new Tuple<int, int>(1920, 1080) },
        };

        public Dictionary<string, Tuple<int, int>> GetResolutionList
        {
            get { return _resolutionList; }
        }

        private List<int> _refreshRateList = new List<int>()
        {
            60,
            75,
            120,
            144,
            165
        };

        public List<int> GetRefreshRateList
        {
            get { return _refreshRateList; }
        }
    }
}