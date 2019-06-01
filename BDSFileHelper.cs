using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace BDSPlayerMgmt
{
    class BDSFileHelper
    {
        public bool ReadFromFile(ref string content, string path)
        {
            try
            {
                FileStream fs = File.OpenRead(path);
                StreamReader sr = new StreamReader(fs, Encoding.UTF8);
                content = sr.ReadToEnd();
                return true;
            }
            catch (Exception e)
            {
                content = e.Message;
                return false;
            }
        }
    }
}
