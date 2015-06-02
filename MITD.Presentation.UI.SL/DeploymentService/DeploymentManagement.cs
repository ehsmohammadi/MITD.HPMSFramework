using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows.Resources;
using System.Windows;
using System.Xml.Linq;
using System.Windows.Controls;
using System.Reflection;
using System.IO;
using System.Threading;
using System.ComponentModel;

namespace MITD.Presentation.UI
{
    public class DeploymentManagement : IDeploymentManagement
    {
        public IList<DeploymentModule> Modules{get;set;}
        private readonly IDeploymentServiceWrapper deploymentService;
        private SynchronizationContext synchContext;
        public DeploymentManagement(IDeploymentServiceWrapper baMITDInfoService)
        {
            Modules = new List<DeploymentModule>();
            this.deploymentService = baMITDInfoService;
            synchContext = SynchronizationContext.Current;

        }
        
        private void addModule(string fileName, Action<Object> action,Action<Exception> onError,bool checkVersion=false)
        {
            var module = Modules.FirstOrDefault(m => m.Name.ToLower() == fileName.ToLower());
            if(!checkVersion && !string.IsNullOrWhiteSpace(module.FileUrl))
            {
                action(null);
                return;
            }
            var request = new InstanceRequest()
            {
                AssemblyName = Path.ChangeExtension(fileName,"dll"),
                ClassName = null,
                ReturnAction = action,
                OnError=onError
            };

            WebClient client = new WebClient();
            client.OpenReadCompleted += (s, a) =>
            {
                if (a.Error == null)
                {
                    module.FileUrl = a.UserState as string;
                    addParts(a.Result, request);
                }
                request.OnError(a.Error);
            };
            deploymentService.GetXapVersion(fileName,
                                           res =>
                                               {
                                                   request.NewUrl = res;
                                                   request.OldUrl = module.FileUrl;
                                                   if (String.IsNullOrEmpty(module.FileUrl) || module.FileUrl != res)
                                                   {

                                                       client.OpenReadAsync(new Uri(res, UriKind.Relative), res);

                                                   }
                                                   else
                                                   {
                                                     
                                                       request.ReturnAction(null);
                                                   }
                                               },
                                               onError);

        }

        public void AddModule(Type type, Action<object> action, Action<Exception> onError)
        {
            
            var module=Modules.FirstOrDefault(m => m.Types.Any(v =>v == type));
            addModule(module.Name, action,onError);

        }

        private void addParts(Stream stream, InstanceRequest request)
        {
            var sri = new StreamResourceInfo(stream, null);
            StreamResourceInfo sri4dll = Application.GetResourceStream(sri,
                                             new Uri("AppManifest.xaml", UriKind.Relative));
            var manifest = XElement.Load(sri4dll.Stream);

            var q = from assemblyPart in manifest.Descendants()
                    where assemblyPart.Name.LocalName == "ExtensionPart"
                    select assemblyPart.Attribute("Source").Value;

            request.PartCount = q.Count();
            if (request.PartCount == 0)
            {
                synchContext.Post(delegate
                {
                    addAssemblyParts(sri, manifest, request);
                }, null);
            }
            else
            {
                var worker = new BackgroundWorker();
                worker.DoWork += (s, a) =>
                {
                    var partNames=q.ToArray();
                    deploymentService.GetXapVersions(partNames,
                        res =>
                        {
                            var i = 0;
                            foreach (var f in res)
                            {
                                addExternalParts(sri, manifest,
                                                 partNames[i],
                                                 f, request);
                                ++i;
                            }
                        }, request.OnError);
                };
                worker.RunWorkerAsync();
            }
        }
        
        private void addExternalParts(StreamResourceInfo sri, XElement manifest, string asmPartName, string fileUrl, InstanceRequest request)
        {
            var client = new WebClient();
            var asmPartDllName = System.IO.Path.ChangeExtension(asmPartName, "dll");
            client.OpenReadCompleted += (s, a) =>
            {
                var sri2 = new StreamResourceInfo(a.Result, null);
                var res = Application.GetResourceStream(sri2,
                                                 new Uri(asmPartDllName, UriKind.Relative));
                synchContext.Post(delegate
                                      {
                                          var asmPart = new AssemblyPart();
                                          asmPart.Load(res.Stream);
                                          --request.PartCount;
                                          if (request.PartCount == 0)
                                          {
                                              addAssemblyParts(sri, manifest, request);
                                          }
                                      }, null);
            };
            client.OpenReadAsync(new Uri(fileUrl, UriKind.Relative));
        }

        private void addAssemblyParts(StreamResourceInfo sri, XElement manifest, InstanceRequest request)
        {
            Assembly asm = null;

            var q = from assemblyPart in manifest.Descendants()
                    where assemblyPart.Name.LocalName == "AssemblyPart"
                    select assemblyPart.Attribute("Source").Value;


            q.ToList().ForEach(asmPartName =>
            {
                var res = Application.GetResourceStream(sri,
                                                 new Uri(asmPartName, UriKind.Relative));
                var asmPart = new AssemblyPart();
                if (asmPartName.ToLower() == request.AssemblyName.ToLower())
                    asm = asmPart.Load(res.Stream);
                else
                    asmPart.Load(res.Stream);

            });


            var type = asm.GetTypes().FirstOrDefault(t => typeof (IBootStrapper).IsAssignableFrom(t));
            if (type != null)
            {
                var obj = Activator.CreateInstance(type);
                if (obj != null)
                {
                    ((IBootStrapper) obj).Execute();
                    var dic = new Dictionary<string, string>();
                    dic.Add(request.NewUrl, request.OldUrl);
                    request.ReturnAction(dic);
                }
            }
        }
    }

}