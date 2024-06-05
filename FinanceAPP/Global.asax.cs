using Autofac.Integration.Mvc;
using Autofac;
using FinanceService.Contracts.Partner;
using FinanceService.Services.Partner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using FinanceService.Contracts.FinancialItam;
using FinanceService.Contracts.TransferExcel2List;
using FinanceService.Contracts.UploadFile;
using FinanceService.Services.FinanceItem;
using FinanceService.Services.TransferExcel2List;
using FinanceService.Services.UploadFile;
using FinanceService.Contracts.PartnerFinancialItem;
using FinanceService.Services.PartnerFinancialItems;
using Serilog;

using Serilog.Sinks.File;

namespace FinanceAPP
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            RegisterAutofac();

            string fileName = "log" + DateTime.Now.ToString("yy-MM-dd") +".txt";
            Log.Logger = new LoggerConfiguration()
           .MinimumLevel.Debug()
           .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
           .Enrich.FromLogContext()
           .WriteTo.File(Server.MapPath("~/Logs/") + fileName)
           .CreateLogger();

        }

        private void RegisterAutofac()
        {
            var builder = new ContainerBuilder();

            // Register controllers
            builder.RegisterControllers(typeof(MvcApplication).Assembly);

            // Register services

            builder.RegisterType<PartnerSrv>().As<IPartner>();
            builder.RegisterType<FinancialItemSrv>().As<IFinancialItem>();
            builder.RegisterType<UploadFileSrv>().As<IUploadFile>();


            builder.RegisterGeneric(typeof(TransferExcel2ListSrv<>)).As(typeof(ITransferExcel2List<>))
                    .InstancePerLifetimeScope();
                   

            //builder.RegisterType<PartnerFinancialItemsSrv>().AsSelf();

            builder.RegisterType<PartnerFinancialItemsSrv>()
               .As<IPartnerFinancialItems>()
               .WithParameter(
                   (pi, ctx) => pi.ParameterType == typeof(IPartner),
                   (pi, ctx) => ctx.Resolve<IPartner>()
               )
               .WithParameter(
                   (pi, ctx) => pi.ParameterType == typeof(IFinancialItem),
                   (pi, ctx) => ctx.Resolve<IFinancialItem>()
               )
               .InstancePerLifetimeScope();

            var container = builder.Build();

            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }
    }
}
