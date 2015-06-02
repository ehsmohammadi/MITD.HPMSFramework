using System;
using System.Linq;
using System.Linq.Expressions;
using System.Windows.Data;
using System.Windows.Media.Media3D;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.ComponentModel;
using System.Windows.Media;
using System.Diagnostics;
using System.Windows.Browser;
using MITD.Core;
using MITD.Presentation.UI.Message;

namespace MITD.Presentation.UI
{
    public static class Extensions
    {
        public static T FindParentOfType<T>(this FrameworkElement element)
        {
            var parent = VisualTreeHelper.GetParent(element) as FrameworkElement;

            while (parent != null)
            {
                if (parent is T)
                    return (T)(object)parent;

                parent = VisualTreeHelper.GetParent(parent) as FrameworkElement;
            }
            return default(T);
        }
    }

    public class ViewManager :  IViewManager
    {
        private static Dispatcher dispatcher;
        private static bool? _designer;
        private TabControl tc;
        private DataTemplate tabHeaderTemplate;
        private BusyIndicator busyIndicatorObject;
        private BusyIndicatorVM busyIndicatorVM = new BusyIndicatorVM();

        private static void RequireInstance()
        {
            if (_designer == null)
            {
                _designer = DesignerProperties.IsInDesignTool;
            }
            // Design-time is more of a no-op, won't be able to resolve the             
            // dispatcher if it isn't already set in these situations.             
            if (_designer == true)
            {
                return;
            }
            // Attempt to use the RootVisual of the plugin to retrieve a             
            // dispatcher instance. This call will only succeed if the current             
            // thread is the UI thread.             
            try
            {
                dispatcher = Application.Current.RootVisual.Dispatcher;
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("The first time SmartDispatcher is used must be from a user interface thread. Consider having the application call Initialize, with or without an instance.", e);
            }
            if (dispatcher == null)
            {
                throw new InvalidOperationException("Unable to find a suitable Dispatcher instance.");
            }
        }

        public ViewManager()
        {
        }

        public static void Initialize()
        {
            if (dispatcher == null)
            {
                RequireInstance();
            }
        }

        private static void Initialize(Dispatcher dispatcherParam)
        {
            if (dispatcherParam == null)
            {
                throw new ArgumentNullException("dispatcher");
            }
            dispatcher = dispatcherParam;
            if (_designer == null)
            {
                _designer = DesignerProperties.IsInDesignTool;
            }
        }

        private static bool CheckAccess()
        {
            if (dispatcher == null)
            {
                RequireInstance();
            }
            return dispatcher.CheckAccess();
        }

        public void BeginInvokeOnDispatcher(Action action)
        {
            if (dispatcher == null)
            {
                RequireInstance();
            }
            // If the current thread is the user interface thread, skip the             
            // dispatcher and directly invoke the Action.             
            if (dispatcher.CheckAccess() || _designer == true)
            {
                action();
            }
            else { dispatcher.BeginInvoke(action); }
        }

        public void ShowInDialog(IView v, bool hasCloseButton = true)
        {
            var dv = ServiceLocator.Current.GetInstance<IDialogView>();
            Debug.Assert(v != null);
            dv.Title = v.ViewModel.DisplayName;
            var bi = new BusyIndicator();
            bi.Content = v as UserControl;
            var vm = v.ViewModel as WorkspaceViewModel;
            bi.SetBinding(BusyIndicator.IsBusyProperty, new Binding { Source = vm, Path = new PropertyPath("IsBusy"), Mode = BindingMode.TwoWay });
            bi.SetBinding(BusyIndicator.BusyContentProperty, new Binding { Source = vm, Path = new PropertyPath("BusyMessage"), Mode = BindingMode.TwoWay });
            dv.DialogContent = bi;
            var cv = (dv as ChildWindow);
            cv.HasCloseButton = hasCloseButton;
            cv.Show();
        }

        public void ShowInDialog<T>(bool hasCloseButton=true) where T : IView
        {
            var v = ServiceLocator.Current.GetInstance<IView>();
            ShowInDialog(v, hasCloseButton);
        }

        public void ShowInTabControl(IView view)
        {
            var bi = new BusyIndicator();
            bi.Content = view as UserControl;
            var vm = view.ViewModel as WorkspaceViewModel;
            bi.SetBinding(BusyIndicator.IsBusyProperty, new Binding { Source = vm, Path = new PropertyPath("IsBusy"), Mode = BindingMode.TwoWay });
            bi.SetBinding(BusyIndicator.BusyContentProperty, new Binding { Source = vm, Path = new PropertyPath("BusyMessage"), Mode = BindingMode.TwoWay });
            var ti = new TabItem()
            {
                HeaderTemplate = tabHeaderTemplate,
                Header = view.ViewModel,
                Content = bi
            };
            tc.Items.Add(ti);
            ti.Focus();
            ti.IsSelected = true;
        }

        public void ShowInTabControl<T>(IDeploymentManagement deploymentManagement,
            Action<T> acition, Action<Exception> onError) where T : IView
        {
            BusyIndicator bi;
            TabItem ti;
            InitializeTabItem(out bi, out ti);
            deploymentManagement.AddModule(typeof(T),
                                        obj =>
                                        {
                                            var view = InitailizeViewInTabItem<T>(bi, ti);
                                            acition(view);
                                        },
                                        onError
                );

        }

        private TabItem findViewinTabControl<T>() where T:IView
        {
            var q = from TabItem t in tc.Items
                    where (t.Content as BusyIndicator).Content is T 
                    select t;
            return q.FirstOrDefault();
        }
        private TabItem findViewinTabControl<T>(Func<WorkspaceViewModel, bool> predicateShowInNewWindow) where T : IView
        {
            var q = from TabItem t in tc.Items
                    where (t.Content as BusyIndicator).Content is T
                    select t;
            return q.FirstOrDefault(t => predicateShowInNewWindow(((t.Content as BusyIndicator).Content as IView).ViewModel));
        }

        private void InitializeTabItem(out BusyIndicator bi, out TabItem ti)
        {
            bi = new BusyIndicator
            {
                BusyContent = "درحال بارگذاری ...",
                Background = new SolidColorBrush { Color = SystemColors.GrayTextColor },
                Projection = new Matrix3DProjection { ProjectionMatrix = new Matrix3D() },
                FontSize = 10,
                FontFamily = new FontFamily("tahoma"),
                IsBusy = true
            };
            ti = new TabItem()
            {
                HeaderTemplate = tabHeaderTemplate,
                Content = bi
            };
            tc.Items.Add(ti);
            ti.Focus();
            ti.IsSelected = true;
        }

        public T ShowInTabControl<T>(bool showInNewWindow = false) where T : IView
        {
            if (!showInNewWindow)
            {
                var tii = findViewinTabControl<T>();
                if (tii != null)
                {
                    tii.Focus();
                    tii.IsSelected = true;
                    return (T)((tii.Content as BusyIndicator).Content);
                }

            }
            BusyIndicator bi;
            TabItem ti;
            InitializeTabItem(out bi, out ti);
            return InitailizeViewInTabItem<T>(bi, ti);
        }

        public T ShowInTabControl<T>(Func<WorkspaceViewModel, bool> predicateShowInNewWindow) where T : IView
        {
            var tii = findViewinTabControl<T>(predicateShowInNewWindow);
            if (tii != null)
            {
                tii.Focus();
                tii.IsSelected = true;
                return (T)((tii.Content as BusyIndicator).Content);
            }
            BusyIndicator bi;
            TabItem ti;
            InitializeTabItem(out bi, out ti);
            return InitailizeViewInTabItem<T>(bi, ti);

        }
        private static T InitailizeViewInTabItem<T>(BusyIndicator bi, TabItem ti) where T : IView
        {
            var view = ServiceLocator.Current.GetInstance<T>();
            bi.Content = view as UserControl;
            var vm = view.ViewModel as WorkspaceViewModel;
            bi.SetBinding(BusyIndicator.IsBusyProperty, new Binding { Source = vm, Path = new PropertyPath("IsBusy"), Mode = BindingMode.TwoWay });
            bi.SetBinding(BusyIndicator.BusyContentProperty, new Binding { Source = vm, Path = new PropertyPath("BusyMessage"), Mode = BindingMode.TwoWay });
            ti.Header = view.ViewModel;
            return view;
        }

        public void ShowMessage(string msg)
        {
            var view = ServiceLocator.Current.GetInstance<IMessageView>();
            var vm = view.ViewModel as MessageVM;
            vm.Message = msg;
            ShowInDialog(view);
        }

        public void Activate(WorkspaceViewModel workspaceViewModel)
        {
            var q = from TabItem t in tc.Items
                    where (t.Content as IView).ViewModel == workspaceViewModel
                    select t;
            var ti = q.FirstOrDefault();
            if (ti != null)
            {
                ti.Focus();
                ti.IsSelected = true;
            }
        }

        private IView getViewFromTabItem(TabItem tabItem)
        {
            return (tabItem.Content as BusyIndicator).Content as IView;
        }

        public void Close(WorkspaceViewModel workspaceViewModel)
        {
            Debug.Assert(workspaceViewModel.View != null);
            var viewWithContextMenu = workspaceViewModel.View as IViewWithContextMenu;
            if (viewWithContextMenu != null)
            {
                foreach (var item in viewWithContextMenu.ItemsWithContextMenu)
                {
                    ContextMenuService.SetContextMenu(item, null);
                }
            }
            var v = workspaceViewModel.View as UserControl;
            var d = v.FindParentOfType<ChildWindow>();
            if (d != null)
            {
                d.Close();
                workspaceViewModel.View = null;
                (v as IView).ViewModel = null;
            }
            else
            {
                var ti = tc.Items.FirstOrDefault(t => (((TabItem)t).Content as BusyIndicator).Content == v) as TabItem;
                //var q = from TabItem t in tc.Items
                //        where getViewFromTabItem(t).ViewModel == workspaceViewModel
                //        select t;
                //var ti = q.FirstOrDefault();
                if (ti != null)
                {
                    tc.Items.Remove(ti);
                    ti.Header = null;
                    workspaceViewModel.View = null;
                    (v as IView).ViewModel = null;
                }
            }

        }

        public void CloseAllTabs()
        {
            tc.Items.ToList().ForEach(
                t =>
                {
                    var ti = t as TabItem;
                    var workspaceViewModel = getViewFromTabItem(ti).ViewModel;
                    tc.Items.Remove(ti);
                    ti.Header = null;
                    workspaceViewModel.View = null;
                });
        }


        public void ShowInNewBrowser(string pageUrl)
        {
            HtmlPage.Window.Navigate(new Uri(pageUrl, UriKind.RelativeOrAbsolute), "__blank");
        }


        public object TabControl
        {
            get { return tc; }
            set { tc = value as TabControl; }
        }


        public object TabHeaderTemplate
        {
            get
            {
                return tabHeaderTemplate;
            }
            set
            {
                tabHeaderTemplate = value as DataTemplate;
            }
        }

        public object BusyIndicatorObject
        {
            get
            {
                return busyIndicatorObject;
            }
            set
            {
                busyIndicatorObject = value as BusyIndicator;
                busyIndicatorObject.SetBinding(BusyIndicator.IsBusyProperty, new Binding { Source = busyIndicatorVM, Path = new PropertyPath("IsBusy"), Mode = BindingMode.TwoWay });
                busyIndicatorObject.SetBinding(BusyIndicator.BusyContentProperty, new Binding { Source = busyIndicatorVM, Path = new PropertyPath("Message"), Mode = BindingMode.TwoWay });
            }
        }

        public void ShowBusyIndicator()
        {
            BeginInvokeOnDispatcher(()=> busyIndicatorVM.IsBusy = true);
        }

        public void ShowBusyIndicator(string message)
        {
            BeginInvokeOnDispatcher(() =>
            {
                busyIndicatorVM.Message = message;
                busyIndicatorVM.IsBusy = true;
            });
        }

        public void HideBusyIndicator()
        {
            BeginInvokeOnDispatcher(() => busyIndicatorVM.IsBusy = false);
        }
    }
}
