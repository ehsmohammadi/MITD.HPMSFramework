using System;
using System.Windows;
using System.Windows.Controls;
using System.Xml;
using MITD.Core;
using System.Collections.Generic;

namespace MITD.Presentation
{
    public class ApplicationController : IApplicationController
    {

        #region Fields

        protected readonly IViewManager viewManager;
        protected readonly IEventPublisher eventPublisher;
        protected Dictionary<ViewModelBase, UserControl> views = new Dictionary<ViewModelBase, UserControl>();
        protected readonly IDeploymentManagement deploymentManagement;

        #endregion

        #region Cunstructor

        public ApplicationController(IViewManager viewManager,
                                     IEventPublisher eventPublisher,
                                     IDeploymentManagement deploymentManagement)
        {
            this.viewManager = viewManager;
            this.eventPublisher = eventPublisher;
            this.deploymentManagement = deploymentManagement;
        }

        #endregion

        #region Public Contoller Methods

        public void Publish<TArgs>(TArgs arg)
        {
            eventPublisher.Publish<TArgs>(arg);
        }

        public void Subscribe<TArg>(IEventHandler<TArg> eventHandler)
        {
            eventPublisher.RegisterHandler<TArg>(eventHandler);
        }

        public void ShowDialog(IView view, bool hasCloseButton = true)
        {
            viewManager.ShowInDialog(view, hasCloseButton);
        }

        public void ShowMessage(string p)
        {
            MessageBox.Show(p);
        }

        public MessageBoxResult ShowMessage(string message, string caption, MessageBoxButton msButtons)
        {
            return MessageBox.Show(message, caption, msButtons);
        }

        public Boolean ShowConfirmationBox(string message, string title)
        {
            var result = MessageBox.Show(message, title, MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.OK || result == MessageBoxResult.Yes)
                return true;
            else
                return false;
        }

        private static string getAppSetting(string strKey)
        {
            var strValue = string.Empty;
            var settings = new XmlReaderSettings { XmlResolver = new XmlXapResolver() };
            var reader = XmlReader.Create("ServiceReferences.ClientConfig");
            reader.MoveToContent();
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element && reader.Name == "add")
                {
                    if (reader.HasAttributes)
                    {
                        strValue = reader.GetAttribute("key");
                        if (!string.IsNullOrEmpty(strValue) && strValue == strKey)
                        {
                            strValue = reader.GetAttribute("value");
                            return strValue;
                        }
                    }
                }
            }
            return strValue;
        }

        public void Close(WorkspaceViewModel vm)
        {

            viewManager.Close(vm);
            eventPublisher.UnregisterHandlers(vm);
        }

        public void BeginInvokeOnDispatcher(Action action)
        {
            viewManager.BeginInvokeOnDispatcher(action);
        }

        #endregion

        public void Login(string userName, string password)
        {

        }

        public void ShowBusyIndicator(string message)
        {
            viewManager.ShowBusyIndicator(message);
        }

        public void ShowBusyIndicator()
        {
            viewManager.ShowBusyIndicator();
        }

        public void HideBusyIndicator()
        {
            viewManager.HideBusyIndicator();
        }
    }

}
