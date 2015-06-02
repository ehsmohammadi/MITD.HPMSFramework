using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using MITD.Core;
using System.Windows;


namespace MITD.Presentation
{
    public interface IApplicationController
    {
        #region Public

        void Publish<TArgs>(TArgs arg);
        void Subscribe<TArg>(IEventHandler<TArg> eventHandler);
        void Close(WorkspaceViewModel vm);
        void BeginInvokeOnDispatcher(Action action);
        void ShowMessage(string msg);
        MessageBoxResult ShowMessage(string message, string caption, MessageBoxButton msButtons);
        Boolean ShowConfirmationBox(string message, string title);
        void ShowBusyIndicator(string message);
        void ShowBusyIndicator();
        void HideBusyIndicator();

        #endregion
    }
}
