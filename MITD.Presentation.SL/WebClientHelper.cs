using MITD.Core.Exceptions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
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
    public static class WebClientHelper
    {
        
        public enum MessageFormat { Json, Xml };

        public static void Get<T>(Uri uri, Action<T, Exception> action, MessageFormat format = MessageFormat.Xml, Dictionary<string, string> headers = null)
        {
            var request = (HttpWebRequest)System.Net.Browser.WebRequestCreator.ClientHttp.Create(uri);
            request.Method = "GET";
            setAcceptHeader(format, request);
            if(headers != null)
                foreach (var header in headers)
                    request.Headers[header.Key] = header.Value;
            request.BeginGetResponse(iar2 =>
            {
                WebResponse response = null;
                try
                {
                    response = request.EndGetResponse(iar2);
                }
                catch (WebException exp)
                {
                    action(default(T), convertException(exp));
                    return;
                }
                catch (Exception exp)
                {
                    action(default(T), exp);
                    return;
                }
                action(deserializeObject<T>(format, response.GetResponseStream()), null);
            }, null);
        }

        public static void GetString(Uri uri, Action<string, Exception> action, Dictionary<string, string> headers = null)
        {
            var request = (HttpWebRequest)System.Net.Browser.WebRequestCreator.ClientHttp.Create(uri);
            request.Method = "GET";
            if (headers != null)
                foreach (var header in headers)
                    request.Headers[header.Key] = header.Value;
            request.BeginGetResponse(iar2 =>
            {
                WebResponse response = null;
                try
                {
                    response = request.EndGetResponse(iar2);
                }
                catch (WebException exp)
                {
                    action("", convertException(exp));
                    return;
                }
                catch (Exception exp)
                {
                    action(string.Empty, exp);
                    return;
                }
                action(readString(response.GetResponseStream()), null);
            }, null);
        }

        private static Exception convertException(WebException exp)
        {
            Exception exp2 = exp;
            var s = readString(exp.Response.GetResponseStream());
            exp2 = new Exception(s, exp);
            if (exp.Response is HttpWebResponse)
            {
                if (string.IsNullOrWhiteSpace(s))
                    s = "Unauthorized access.";
                var httpExp = exp.Response as HttpWebResponse;
                if (httpExp.StatusCode == HttpStatusCode.Unauthorized)
                    exp2 = new SecurityException(s, exp);
            }         
            return exp2;
        }

        public static void Post(Uri uri, Action<string, Exception> action, string sendData, Dictionary<string, string> headers = null)
        {
            webRequstCall(uri, action, sendData, "POST", headers);
        }

        public static void Post<T1, T2>(Uri uri, Action<T1, Exception> action, T2 sendData,
            MessageFormat format = MessageFormat.Xml, Dictionary<string, string> headers = null)
        {
            webRequstCall<T1, T2>(uri, action, sendData, format, "POST", headers);
        }

        public static void Put<T1, T2>(Uri uri, Action<T1, Exception> action, T2 sendData,
            MessageFormat format = MessageFormat.Xml, Dictionary<string, string> headers = null)
        {
            webRequstCall<T1, T2>(uri, action, sendData, format, "PUT", headers);
        }

        public static void Delete(Uri uri, Action<string, Exception> action, Dictionary<string, string> headers = null)
        {
            webRequstCall(uri, action, "", "DELETE", headers);
        }

        private static void setAcceptHeader(MessageFormat format, HttpWebRequest client)
        {
            switch (format)
            {
                case MessageFormat.Json:
                    client.Accept = "application/json";
                    break;
                default:
                    client.Accept = "application/xml";
                    break;
            }
        }

        private static void setContentType(MessageFormat format, WebRequest request)
        {
            switch (format)
            {
                case MessageFormat.Json:
                    request.ContentType = "application/json";
                    break;
                default:
                    request.ContentType = "application/xml";
                    break;
            }
        }

        private static T deserializeObject<T>(MessageFormat format, Stream stream) 
        {
            T obj;
            switch (format)
            {
                case MessageFormat.Json:
                    //var jsonSer = new DataContractJsonSerializer(typeof(T));
                    //var jsonSer = JsonConvert.()
                    //obj = (T)jsonSer.ReadObject(stream);
                    
                    var serializer = new JsonSerializer();
                    serializer.TypeNameHandling = TypeNameHandling.Auto;
                    serializer.TypeNameAssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple;
                    obj = (T)serializer.Deserialize(new StreamReader(stream), typeof(T));
                    break;
                default:
                    var ser = new DataContractSerializer(typeof(T));
                    obj = (T)ser.ReadObject(stream);
                    break;
            }
            return obj;
        }

        private static T serializeObject<T>(MessageFormat format, Stream stream, T obj)
        {
            switch (format)
            {
                case MessageFormat.Json:
                    //var jsonSer = new DataContractJsonSerializer(typeof(T));
                    //jsonSer.WriteObject(stream, obj);

                    var serializer = new JsonSerializer();
                    serializer.TypeNameHandling = TypeNameHandling.Auto;
                    serializer.TypeNameAssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple;
                    var writer = new StreamWriter(stream);
                    serializer.Serialize(writer, obj,typeof(T));
                    writer.Flush();
                    break;
                case MessageFormat.Xml:
                    var ser = new DataContractSerializer(typeof(T));
                    ser.WriteObject(stream, obj);
                    break;
            }
            return obj;
        }

        private static string readString(Stream stream)
        {
            var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }

        private static void writeString(Stream stream, string str)
        {
            var writer = new StreamWriter(stream);
            writer.Write(str);
            writer.Flush();
        }

        private static void webRequstCall<T1, T2>(Uri uri, Action<T1, Exception> action, T2 sendData,
            MessageFormat format, string method = "POST", Dictionary<string, string> headers=null)
        {
            var request = (HttpWebRequest)System.Net.Browser.WebRequestCreator.ClientHttp.Create(uri);
            request.Method = method;
            setContentType(format, request);
            setAcceptHeader(format, request);
            if (headers != null)
                foreach (var header in headers)
                    request.Headers[header.Key] = header.Value;
            request.BeginGetRequestStream(iar =>
            {
                var reqStr = request.EndGetRequestStream(iar);
                serializeObject(format, reqStr, sendData);
                reqStr.Close();
                request.BeginGetResponse(iar2 =>
                {
                    WebResponse response = null;
                    try
                    {
                        response = request.EndGetResponse(iar2);
                    }
                    catch (WebException exp)
                    {
                        action(default(T1), convertException(exp));
                        return;
                    }
                    catch (Exception exp)
                    {
                        action(default(T1), exp);
                        return;
                    }
                    action(deserializeObject<T1>(format, response.GetResponseStream()), null);
                }, null);
            }, null);
        }

        private static void webRequstCall(Uri uri, Action<string, Exception> action, string senData, string method = "POST", Dictionary<string, string> headers=null)
        {
            var request = (HttpWebRequest)System.Net.Browser.WebRequestCreator.ClientHttp.Create(uri);
            request.Method = method;
            request.ContentType = "application/x-www-form-urlencoded";
            if (headers != null)
                foreach (var header in headers)
                    request.Headers[header.Key] = header.Value;
            request.BeginGetRequestStream(iar =>
            {
                var reqStr = request.EndGetRequestStream(iar);
                writeString(reqStr, "="+senData);
                reqStr.Close();
                request.BeginGetResponse(iar2 =>
                {
                    WebResponse response = null;
                    try
                    {
                        response = request.EndGetResponse(iar2);
                    }
                    catch (WebException exp)
                    {
                        action("", convertException(exp));
                        return;
                    }
                    catch (Exception exp)
                    {
                        action(string.Empty, exp);
                        return;
                    }
                    action(readString(response.GetResponseStream()), null);
                }, null);
            }, null);
        }
    }
}
