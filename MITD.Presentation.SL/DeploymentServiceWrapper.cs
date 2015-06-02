using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace MITD.Presentation
{
    public class DeploymentServiceWrapper : IDeploymentServiceWrapper
    {

        public void GetXapVersion(string fileName, Action<string> onAction, Action<Exception> onError)
        {
            var url = string.Format("/Deployment/GetXapVersion?fileName=" + fileName);
            var client = new WebClient();
            client.DownloadStringCompleted += (s, a) =>
                {
                    if (a.Error == null)
                    {

                        onAction(a.Result);
                    }
                    else
                        onError(a.Error);
                };
            client.DownloadStringAsync(new Uri(url, UriKind.Relative));
        }

        public void GetXapVersions(string[] fileNames, Action<string[]> onAction, Action<Exception> onError)
        {
            var url = string.Format("/Deployment/GetXapVersions");
            var client = new WebClient();
            client.UploadStringCompleted += (s, a) =>
            {
                if (a.Error == null)
                {
                    var jj = JArray.Parse(a.Result);
                    var res = JsonConvert.DeserializeObject<string[]>(jj.ToString());
                    onAction(res);
                }
                else
                    onError(a.Error); 
            };
            var j = JsonConvert.SerializeObject(fileNames);
            client.Headers[HttpRequestHeader.Accept] = "application/json";
            client.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
            client.UploadStringAsync(new Uri(url, UriKind.Relative), "fileNames=" + j);

        }
    }
}
