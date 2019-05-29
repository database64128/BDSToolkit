using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace WpfApp1
{
    class BDSDataHelper
    {
        public BDSDataHelper()
        {
            InitBDSData();
        }

        /// <summary>
        /// Update DataSet From Server Log Text
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="log"></param>
        public void UpdateFromLog(BDSData ds, string log)
        {
            ds.T_LogRecords.Clear();
            foreach (string r in log.Split('\n'))
            {
                if (
                    r.Length > 0
                    && r[0] == '['
                    && r.IndexOf("Player") >= 0
                    && r.IndexOf("xuid: ") >= 0
                    )
                {
                    try
                    {
                        DateTime time = Convert.ToDateTime(r.Substring(1, 19));
                        ulong xuid = Convert.ToUInt64(r.Substring(r.IndexOf("xuid: ") + 6));
                        string gtag = r.Substring(r.IndexOf("ed: ") + 4, r.IndexOf(',') - r.IndexOf("ed: ") - 4);
                        string type;
                        if (r.IndexOf("Player disconnected: ") >= 0)
                        {
                            type = "Disconnected";
                        }
                        else if(r.IndexOf("Player Connected: ") >= 0)
                        {
                            type = "Connected";
                        }
                        else
                        {
                            type = "Unknown";
                        }
                        ds.T_LogRecords.Rows.Add(type, time, xuid, gtag);
                    }
                    catch
                    {
                        continue;
                    }
                }
            }

        }

        private void InitBDSData()
        {

        }

        enum LogRecordTypes
        {
            Connected = 0,
            Disconnected = 1,
            Unknown = -1,
        }
    }
}
