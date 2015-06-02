using Castle.MicroKernel.ModelBuilder.Inspectors;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.Releasers;
using Castle.Windsor;
using MITD.Core;
using MITD.Core.Config;
using MITD.Presentation.UI;
using MITD.Presentation.UI.Message;
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Resources;

namespace MITD.Presentation.Config
{
    public static class ApplicationConfigHelper
    {
        private static Type viewModelType { get; set; }
        private static Type viewType { get; set; }

        public static IWindsorContainer Configure<T1, T2, T3>(Dictionary<string, List<Type>> modules)
            where T1 : WorkspaceViewModel
            where T2 : UserControl, IMainView
            where T3 : IApplicationController
        {
            viewModelType = typeof(T1);
            viewType = typeof(T2);

            var container = new WindsorContainer();
            container.Kernel.ComponentModelBuilder.RemoveContributor(
                container.Kernel.ComponentModelBuilder.Contributors.OfType<PropertiesDependenciesModelInspector>().Single());
            container.Kernel.ReleasePolicy = new NoTrackingReleasePolicy();
            container.AddFacility<EventAggregatorFacility>();
            container.Register(
                Component.For<IWindsorContainer>().Instance(container),
                Component.For<IViewManager>().ImplementedBy<ViewManager>().LifestyleSingleton(),
                Component.For<IMessageView>().ImplementedBy<MessageView>().LifestyleTransient(),
                Component.For<MessageVM>().LifestyleTransient(),
                Component.For<IDeploymentServiceWrapper>().ImplementedBy<DeploymentServiceWrapper>().LifestyleSingleton(),
                Classes.FromAssemblyContaining<T2>().BasedOn<IView>().WithService.FromInterface().LifestyleTransient(),
                 Classes.FromAssemblyContaining<T2>().BasedOn<ILocalizedResources>().WithService.FromInterface().LifestyleTransient(),
                Classes.FromAssemblyContaining<T2>().BasedOn<IDialogView>().WithService.FromInterface().LifestyleTransient(),
                Classes.FromAssemblyContaining<T1>().BasedOn<IApplicationController>().WithService.FromInterface().LifestyleSingleton()
                .ConfigureFor<T3>(c => c.Forward<IApplicationController>()),
                Classes.FromAssemblyContaining<T1>().BasedOn<IServiceWrapper>().WithService.FromInterface().LifestyleSingleton(),
                Classes.FromAssemblyContaining<T1>().BasedOn<WorkspaceViewModel>().LifestyleTransient(),
                Component.For<DeploymentManagement>().LifestyleSingleton(),
                Component.For<IDeploymentManagement>().UsingFactoryMethod<DeploymentManagement>(
                    c =>
                    {
                        var deploymentManagement = c.Resolve<DeploymentManagement>();
                        prepareModules(modules, deploymentManagement);
                        return deploymentManagement;
                    }).LifestyleSingleton()
                );
            var locator = new WindsorServiceLocator(container);
            ServiceLocator.SetLocatorProvider(() => locator);



            return container;
        }

        private static void prepareModules(Dictionary<string, List<Type>> modules, DeploymentManagement deploymentManagement)
        {
            foreach (var module in modules)
            {
                deploymentManagement.Modules.Add(new DeploymentModule
                {
                    FileUrl = "",
                    Name = module.Key,
                    Types = module.Value
                });
            }
        }

        public static void ConfigureModule<T1, T2>(StreamResourceInfo resourceDic)
            where T1 : class
            where T2 : class,T1
        {
            var container = ServiceLocator.Current.GetInstance<IWindsorContainer>();

            var sReader = new StreamReader(resourceDic.Stream);
            var sText = sReader.ReadToEnd();
            var rd = XamlReader.Load(sText);
            Application.Current.Resources.MergedDictionaries.Add(rd as ResourceDictionary);

            container.Register(
                Classes.FromAssemblyContaining<T2>().BasedOn<IView>().WithService.FromInterface().LifestyleTransient(),
                Classes.FromAssemblyContaining<T2>().BasedOn<ILocalizedResources>().WithService.FromInterface().LifestyleTransient(),
                Classes.FromAssemblyContaining<T2>().BasedOn<WorkspaceViewModel>().LifestyleTransient(),
                Component.For<T1>().ImplementedBy<T2>().LifestyleSingleton()
                );
        }

        public static void Start()
        {
            var vm = ServiceLocator.Current.GetInstance(viewModelType);
            var page = Activator.CreateInstance(viewType);

            (page as FrameworkElement).DataContext = vm;

            var viewManager = ServiceLocator.Current.GetInstance<IViewManager>();
            viewManager.TabControl = (page as IMainView).TabControl;
            viewManager.TabHeaderTemplate = (page as IMainView).TabHeaderTemplate;
            viewManager.BusyIndicatorObject = (page as IMainView).BusyIndicator;
            Application.Current.RootVisual = (page as FrameworkElement);

            ViewManager.Initialize();
        }
    }
}
