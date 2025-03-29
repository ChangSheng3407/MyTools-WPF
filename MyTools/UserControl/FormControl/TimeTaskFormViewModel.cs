using HandyControl.Controls;
using HandyControl.Data;
using HandyControl.Tools.Extension;
using LiteDB;
using MyTools.Helpers;
using MyTools.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Input;

namespace MyTools.UserControl.FormControl
{
    public class TimeTaskFormViewModel : BaseViewModel, IDialogResultable<TimeTaskJobConfig>
    {
        private TimeTaskJobConfig _Config = new TimeTaskJobConfig();
        private string _Title;
        private ObservableCollection<ExecuteTypeEnum> _ExecuteTypes = new ObservableCollection<ExecuteTypeEnum>
        {
            ExecuteTypeEnum.CMD,
            ExecuteTypeEnum.HTTP,
            ExecuteTypeEnum.REMIND
        };
        private Dictionary<string, bool> _Enable = new Dictionary<string, bool> { { "启用", true }, { "禁用", false } };

        public TimeTaskJobConfig Config
        {
            get => _Config; set
            {
                _Config = value;
                OnPropertyChanged();
            }
        }
        public string Title
        {
            get => _Title; set
            {
                _Title = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<ExecuteTypeEnum> ExecuteTypes { get => _ExecuteTypes; }
        public Dictionary<string, bool> Enable { get => _Enable; set => _Enable = value; }
        public TimeTaskJobConfig Result { get; set; }

        #region Command

        public Action? CloseAction { get; set; }
        public ICommand Save => new MyCommand(o =>
        {
            if (string.IsNullOrEmpty(Config.JobName) || string.IsNullOrEmpty(Config.ExpString))
            {
                Growl.Warning("请输入必填项");
                return;
            }
            if (!Config.ExpString.CronValid())
            {
                Growl.Error("Crontab表达式不正确");
                throw new ArgumentException("Crontab表达式不正确");
            }
            if (Config.Id == null)
            {
                Config.DBInsert();
            }
            else
            {
                Config.DBUpdate();
            }
            Result = Config;
            CloseAction?.Invoke();
        });
        public ICommand CheckExpString => new MyCommand(o =>
        {
            string message = "";
            try
            {
                message = QuartzHelper.GetCronNextRunTime(Config.ExpString);
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            Growl.Info(message);
        });

        #endregion
    }
}
