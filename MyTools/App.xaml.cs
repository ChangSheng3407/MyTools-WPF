using MyTools.Helpers;
using MyTools.Model;
using Serilog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace MyTools
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        //通过创建一个全局的Mutex对象来确保同一时间内只有一个程序实例在运行。
        private Mutex mutex = new();
        public App()
        {
            //UI线程未捕获异常处理事件
            this.DispatcherUnhandledException += new DispatcherUnhandledExceptionEventHandler(App_DispatcherUnhandledException);
            //Task线程内未捕获异常处理事件
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
            //非UI线程未捕获异常处理事件
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
        }
        #region 全局异常处理

        /// <summary>
        /// UI线程未捕获异常处理事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            try
            {
                e.Handled = true; //把 Handled 属性设为true，表示此异常已处理，程序可以继续运行，不会强制退出
                WritrGlobalExceptionLog("UI线程异常", e.Exception);
                MessageBox.Show("UI线程异常:" + e.Exception.Message);
            }
            catch (Exception)
            {
                //此时程序出现严重异常，将强制结束退出
                MessageBox.Show("UI线程发生致命错误！");
            }
        }
        /// <summary>
        /// 非UI线程未捕获异常处理事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            StringBuilder sbEx = new StringBuilder();
            if (e.IsTerminating)
            {
                sbEx.Append("非UI线程发生致命错误");
            }
            sbEx.Append("非UI线程异常：");
            if (e.ExceptionObject is Exception ex)
            {
                sbEx.Append(ex.Message);
            }
            else
            {
                sbEx.Append(e.ExceptionObject);
            }
            WritrGlobalExceptionLog("非UI线程异常", (Exception)e.ExceptionObject);
            MessageBox.Show(sbEx.ToString());
        }
        /// <summary>
        /// Task线程内未捕获异常处理事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void TaskScheduler_UnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
        {
            //task线程内未处理捕获
            //MessageBox.Show("Task线程异常：" + e.Exception.Message);
            WritrGlobalExceptionLog("Task线程异常", e.Exception);
            e.SetObserved();//设置该异常已察觉（这样处理后就不会引起程序崩溃）
        }
        void WritrGlobalExceptionLog(string errorType, Exception ex)
        {
            Log.Error($"{errorType}：{ex.InnerException?.Message ?? ex.Message}\r\n{ex.InnerException?.StackTrace ?? ex.StackTrace}");
        }
        #endregion
        protected override void OnStartup(StartupEventArgs e)
        {
            bool createdNew;
            mutex = new Mutex(true, "MyUniqueAppName", out createdNew);

            if (!createdNew)
            {
                // 另一个实例已经在运行，激活它并退出当前实例
                Process current = Process.GetCurrentProcess();
                var process = Process.GetProcessesByName(current.ProcessName).FirstOrDefault(p => p.Id != current.Id);
                if (process != null)
                {
                    SetForegroundWindow(process.MainWindowHandle);
                    Environment.Exit(0);
                    return;
                }
            }
            StartupToDo();
            base.OnStartup(e);
        }

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        /// <summary>
        /// 启动时执行的操作
        /// </summary>
        void StartupToDo()
        {
            // 注册GB2312编码
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            if (Directory.Exists(CommonHelper.TempDir))
            {
                Directory.Delete(CommonHelper.TempDir, true);
            }
            Directory.CreateDirectory(CommonHelper.TempDir);
            LiteDBHelper.Init<SystemConfig>(o => o.Key == "BackgroundPath", new SystemConfig { Key = "BackgroundPath", Value = "Resources/Background.png" });
            Log.Logger = new LoggerConfiguration()
               .MinimumLevel.Debug().WriteTo
               .File("Logs/log.txt", rollingInterval: RollingInterval.Hour, rollOnFileSizeLimit: true, fileSizeLimitBytes: 1024 * 1024 * 10)
               .CreateLogger();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            Log.CloseAndFlush(); // 确保所有日志都被写入
            base.OnExit(e);
        }
    }
}
