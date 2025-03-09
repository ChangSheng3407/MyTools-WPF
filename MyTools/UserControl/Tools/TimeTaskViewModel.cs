using HandyControl.Controls;
using HandyControl.Data;
using HandyControl.Tools.Extension;
using LiteDB;
using Masuit.Tools;
using MyTools.Helpers;
using MyTools.Jobs;
using MyTools.Model;
using MyTools.UserControl.FormControl;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MyTools.UserControl.Tools
{
    public class TimeTaskViewModel : BaseViewModel
    {
        private ObservableCollection<TimeTaskJobConfig> _JobConfigs = new ObservableCollection<TimeTaskJobConfig>();
        private TimeTaskJobConfig _Config = new TimeTaskJobConfig();
        private string _SchedulerContent = "启动";

        public string SchedulerContent
        {
            get => _SchedulerContent; set
            {
                _SchedulerContent = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 任务配置集合
        /// </summary>
        public ObservableCollection<TimeTaskJobConfig> JobConfigs
        {
            get => _JobConfigs; set
            {
                _JobConfigs = value;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// 当前配置
        /// </summary>
        public TimeTaskJobConfig Config
        {
            get => _Config; set
            {
                _Config = value;
                OnPropertyChanged();
            }
        }
        public TimeTaskViewModel()
        {
            JobConfigs = LiteDBHelper.DBFind<TimeTaskJobConfig>().ToObservableCollection();
        }
        /// <summary>
        /// 添加任务配置
        /// </summary>
        /// <param name="parameter"></param>
        public ICommand Add => new MyCommand(o =>
        {
            Dialog.Show<TimeTaskForm>()
                  .Initialize<TimeTaskFormViewModel>(vm => vm.Title = "新增")
                  .GetResultAsync<TimeTaskJobConfig>()
                  .ContinueWith(result =>
                  {
                      //Growl.Info(JsonConvert.SerializeObject(result.Result.Status));
                      if (result.Result != null)
                      {
                          QuartzHelper.AddJob<TimeTaskJob>(result.Result);
                          JobConfigs = LiteDBHelper.DBFind<TimeTaskJobConfig>().ToObservableCollection();
                          CommonHelper.ClearAllGrowlNotifications();
                      }
                  });
        });
        /// <summary>
        /// 编辑任务配置
        /// </summary>
        /// <param name="parameter"></param>
        public ICommand Edit => new MyCommand(o =>
        {
            Dialog.Show<TimeTaskForm>()
                  .Initialize<TimeTaskFormViewModel>(vm => { vm.Title = "编辑"; vm.Config = Config.DeepClone(); })
                  .GetResultAsync<TimeTaskJobConfig>()
                  .ContinueWith(result =>
                  {
                      //Growl.Info(JsonConvert.SerializeObject(result.Result));
                      if (result.Result != null)
                      {
                          QuartzHelper.RemoveJob(Config);
                          QuartzHelper.AddJob<TimeTaskJob>(result.Result);
                          JobConfigs = LiteDBHelper.DBFind<TimeTaskJobConfig>().ToObservableCollection();
                          CommonHelper.ClearAllGrowlNotifications();
                      }
                  });
        });
        /// <summary>
        /// 删除任务配置
        /// </summary>
        /// <param name="parameter"></param>
        public ICommand Del => new MyCommand(o =>
        {
            var result = MessageBox.Show("确定删除吗？", "提示", System.Windows.MessageBoxButton.YesNo);
            if (result == System.Windows.MessageBoxResult.Yes)
            {
                QuartzHelper.RemoveJob(Config);
                Config.DBDelete();
                JobConfigs = LiteDBHelper.DBFind<TimeTaskJobConfig>().ToObservableCollection();
            }
        });
        /// <summary>
        /// 启动/停止调度器
        /// </summary>
        public ICommand ChangeScheduler => new MyCommand(o =>
        {
            if (QuartzHelper.ScheduleIsRunning())
            {
                QuartzHelper.ShutdownQuartz();
                SchedulerContent = "启动";
            }
            else
            {
                QuartzHelper.StartQuartz<TimeTaskJobConfig, TimeTaskJob>(JobConfigs);
                SchedulerContent = "终止";
            }
        });
    }
}
