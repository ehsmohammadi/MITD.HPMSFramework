using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MITD.Presentation
{
    public interface IViewManager
    {
        void ShowInTabControl(IView view);
        void ShowInTabControl<T>(IDeploymentManagement deploymentManagement, Action<T> acition, Action<Exception> onError) where T : IView;
        T ShowInTabControl<T>(bool showInNewWindow = false) where T : IView;
        T ShowInTabControl<T>(Func<WorkspaceViewModel, bool> predicateShowInNewWindow) where T : IView;
        void ShowInDialog(IView view, bool hasCloseButton=true);
        void Close(WorkspaceViewModel workspaceViewModel);
        void ShowMessage(string msg);
        void Activate(WorkspaceViewModel workspaceViewModel);

        void BeginInvokeOnDispatcher(Action action);

        void ShowInNewBrowser(string pageUrl);
        void CloseAllTabs();
        object TabControl { get; set; }
        object TabHeaderTemplate { get; set; }
        object BusyIndicatorObject { get; set; }
        void ShowBusyIndicator();
        void ShowBusyIndicator(string message);
        void HideBusyIndicator();
    }
}

