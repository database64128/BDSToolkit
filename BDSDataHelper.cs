using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.IO;

namespace WpfApp1
{
    class BDSDataHelper
    {
        public BDSDataHelper()
        {
            InitBDSData();
        }

        public BDSDataHelper(string log)
        {
            InitBDSData();
            UpdateFromLog(log);
        }

        /// <summary>
        /// Update DataSet From Server Log Text
        /// </summary>
        /// <param name="log"></param>
        public void UpdateFromLog(string log)
        {
            //Update Table_LogRecords
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
                        //Interpretation for Server Log Text
                        DateTime time = Convert.ToDateTime(r.Substring(1, 19));
                        ulong xuid = Convert.ToUInt64(r.Substring(r.IndexOf("xuid: ") + 6));
                        string gtag = r.Substring(r.IndexOf("ed: ") + 4, r.IndexOf(',') - r.IndexOf("ed: ") - 4);
                        string type;
                        if (r.IndexOf("Player disconnected: ") >= 0)
                        {
                            type = "Disconnected";
                        }
                        else if(r.IndexOf("Player connected: ") >= 0)
                        {
                            type = "Connected";
                        }
                        else
                        {
                            type = "Unknown";
                        }

                        //Add Rows to DataTable
                        DataRow dr = ds.T_LogRecords.NewRow();
                        dr["RecordType"] = type;
                        dr["Time"] = time;
                        dr["XUID"] = xuid;
                        dr["GamerTag"] = gtag;
                        ds.T_LogRecords.Rows.Add(dr);
                    }
                    catch
                    {
                        continue;
                    }
                }
            }

            //Update Table_Players Form Table_LogRecords
            List<ulong> xuidList = new List<ulong>();
            Dictionary<ulong, string> lastUsername = new Dictionary<ulong, string>();
            Dictionary<ulong, int> logonCount = new Dictionary<ulong, int>();
            Dictionary<ulong, DateTime> lastConnection = new Dictionary<ulong, DateTime>();
            Dictionary<ulong, TimeSpan> timePlayed = new Dictionary<ulong, TimeSpan>();
            foreach (DataRow dr in ds.T_LogRecords.Rows)
            {
                ulong xuid = (ulong)dr["XUID"];
                if (!xuidList.Contains(xuid))
                {
                    xuidList.Add(xuid);
                    timePlayed.Add(xuid, new TimeSpan(0));
                }
                if ((string)dr["RecordType"] == "Connected")
                {
                    if (logonCount.ContainsKey(xuid))
                    {
                        logonCount[xuid]++;
                    }
                    else
                    {
                        logonCount.Add(xuid, 1);
                    }
                    if (lastConnection.ContainsKey(xuid))
                    {
                        if (lastConnection[xuid] < (DateTime)dr["Time"])
                        {
                            lastConnection[xuid] = (DateTime)dr["Time"];
                            lastUsername[xuid] = (string)dr["GamerTag"];
                        }
                    }
                    else
                    {
                        lastConnection[xuid] = (DateTime)dr["Time"];
                        lastUsername[xuid] = (string)dr["GamerTag"];
                    }
                }
                else
                {
                    timePlayed[xuid] += (DateTime)dr["Time"] - lastConnection[xuid];
                }
            }

            //Add Rows to DataTable
            foreach (ulong xuid in xuidList)
            {
                DataRow dr = ds.T_Players.NewRow();
                dr["XUID"] = xuid;
                dr["GamerTag"] = lastUsername[xuid];
                dr["LastConnection"] = lastConnection[xuid];
                dr["LogonCount"] = logonCount[xuid];
                dr["TimePlayed"] = timePlayed[xuid];
                ds.T_Players.Rows.Add(dr);
            }
        }

        /// <summary>
        /// Returns all Rows in Table_LogRecords
        /// </summary>
        /// <returns></returns>
        public DataRowCollection GetRecords()
        {
            return ds.T_LogRecords.Rows;
        }

        /// <summary>
        /// Returns all Rows in Table_Players
        /// </summary>
        /// <returns></returns>
        public DataRowCollection GetPlayers()
        {
            return ds.T_Players.Rows;
        }


        private BDSData ds;

        private void InitBDSData()
        {
            ds = new BDSData();
        }

        /// <summary>
        /// Not currently in use
        /// </summary>
        enum LogRecordTypes
        {
            Connected = 0,
            Disconnected = 1,
            Unknown = -1,
        }
    }
}
