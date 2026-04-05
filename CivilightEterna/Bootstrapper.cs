using System;
using Stylet;
using StyletIoC;
using CivilightEterna.Pages;
using System.Reflection;
using iNKORE.UI.WPF.Modern.Common.IconKeys;
using System.Threading.Tasks;
using System.Windows;
using log4net;
using log4net.Appender;
using log4net.Core;
using log4net.Layout;
using log4net.Repository.Hierarchy;

namespace CivilightEterna
{
    public class Bootstrapper : Bootstrapper<ShellViewModel>
    {
        protected override void ConfigureIoC(IStyletIoCBuilder builder)
        {
            // Configure the IoC container in here
            builder.Bind<ILog>().ToInstance(LogManager.GetLogger("BABEL"));
            builder.Autobind(Assembly.GetExecutingAssembly());
        }

        protected override void Configure()
        {
            // Perform any other configuration before the application starts
            // 1. 创建布局（日志格式）
            var patternLayout = new PatternLayout
            {
                ConversionPattern = "%date{yyyy-MM-dd HH:mm:ss.fff}|%logger|[%thread] %-5level  - %message%newline"
            };
            patternLayout.ActivateOptions();

            // 2. 控制台输出
            var consoleAppender = new ConsoleAppender
            {
                Layout = patternLayout
            };
            consoleAppender.ActivateOptions();

            // 3. 文件输出（滚动文件，按大小+日期）
            var fileAppender = new RollingFileAppender
            {
                // 🔥 日志保存路径（这里写你想要的地址）
                File = @"Logs\Log_",
                Encoding = System.Text.Encoding.UTF8,
                AppendToFile = true,
                RollingStyle = RollingFileAppender.RollingMode.Composite,
                DatePattern = "yyyy-MM-dd'.log'",
                MaxSizeRollBackups = 10,     // 最多保留10个文件
                MaximumFileSize = "10MB",    // 单个文件最大10MB
                StaticLogFileName = false,
                Layout = patternLayout,
            };
            fileAppender.ActivateOptions();

            // 4. 配置根 Logger
            var hierarchy = (Hierarchy)LogManager.GetRepository();
            hierarchy.Root.AddAppender(consoleAppender);
            hierarchy.Root.AddAppender(fileAppender);
            hierarchy.Root.Level = Level.Debug;  // 日志级别
            hierarchy.Configured = true;
        }

        protected override void OnLaunch()
        {
            // This is called just after the root ViewModel has been launched
            // Something like a version check that displays a dialog might be launched from here
            // BABEL LOADING
            Container.Get<ILog>().Info("BABEL LOADING");
            Container.Get<ILog>().Info("Civilight Eterna V1");
            Container.Get<ILog>().Info("HDiffPatch");
            Container.Get<ILog>().Warn("Launch");
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
        }
        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Container.Get<ILog>().Error("CurrentDomain_UnhandledException", (Exception)e.ExceptionObject);
            Execute.OnUIThread(() =>
            {
                //Util.ErrFileSave("CurrentDomain_UnhandledException", ((Exception)e.ExceptionObject).Message, ((Exception)e.ExceptionObject).ToString());
                if (e.IsTerminating)
                {
                    iNKORE.UI.WPF.Modern.Controls.MessageBox.Show(((Exception)e.ExceptionObject).Message, "错误", MessageBoxButton.OK, FluentSystemIcons.Warning_24_Filled);
                }
                else
                {
                    iNKORE.UI.WPF.Modern.Controls.MessageBox.Show(((Exception)e.ExceptionObject).Message, "错误", MessageBoxButton.OK, FluentSystemIcons.Warning_24_Filled);
                }
            });
        }

        void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            Container.Get<ILog>().Error("TaskScheduler_UnobservedTaskException", e.Exception);

            //Util.ErrFileSave("TaskScheduler_UnobservedTaskException", e.Exception.Message, e.Exception.ToString());
            //task线程内未处理捕获
            Execute.OnUIThread(() =>
            {
                iNKORE.UI.WPF.Modern.Controls.MessageBox.Show(e.Exception.Message, "错误", MessageBoxButton.OK, FluentSystemIcons.Warning_24_Filled);
                e.SetObserved();//设置该异常已察觉（这样处理后就不会引起程序崩溃）
            });
        }
    }
}
