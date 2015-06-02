using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MITD.Services
{
    public static class DeploymentServiceHelper
    {
        public static string GetXapFileVersion(string fileName)
        {
            var fileUri = AppDomain.CurrentDomain.BaseDirectory + @"ClientBin\" + fileName;
            var lastWriteTime = File.GetLastWriteTime(fileUri);
            return fileName + "?ignore=" + lastWriteTime;
        }

        public static string[] GetXapFileVersion(string[] fileNames)
        {
            var res = new string[fileNames.Count()];
            var i=0;
            foreach (var file in fileNames)
            {
                var fileUri = AppDomain.CurrentDomain.BaseDirectory + @"ClientBin\" + file;
                var lastWriteTime = File.GetLastWriteTime(fileUri);
                res[i] = file + "?ignore=" + lastWriteTime;
                ++i;
            }
            return res;
        }
    }
}
