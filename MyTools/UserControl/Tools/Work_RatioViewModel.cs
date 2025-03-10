using HandyControl.Controls;
using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace MyTools.UserControl.Tools
{
    public class Work_RatioViewModel : BaseViewModel
    {
        private double _salary_day = 320;
        private double _salary = 6960;
        private double _working_hours = 8;
        private double _commuting_hours = 1;
        private double _fishing_hours = 3;
        private double _comprehensive = 0.9;
        private Dictionary<string, double> _Education_List = new Dictionary<string, double>
        {
            {"专科及以下0.8",0.8},
            {"普通本科1.0",1.0},
            {"高级本科1.2",1.2},
            {"普通硕士1.4",1.4},
            {"高级硕士1.6",1.6},
            {"普通博士1.8",1.8},
            {"高级博士2.0",2},
        };
        private double _Education_Ratio = 1.0;
        private Dictionary<string, double> _Work_Enviorment_List = new Dictionary<string, double>
        {
            {"偏僻地区0.8", 0.8},
            {"工厂户外0.9", 0.9},
            {"普通1.0", 1.0},
            {"体制内1.1", 1.1},
        };
        private double _Work_Enviorment_Ratio = 1.0;
        private Dictionary<string, double> _Sex_List = new Dictionary<string, double>
        {
            {"没有0.9", 0.9},
            {"不多不少1.0", 1.0},
            {"很多1.1", 1.1},
        };
        private double _Sex_Ratio = 0.9;
        private Dictionary<string, double> _Colleague_Enviorment_List = new Dictionary<string, double>
        {
            { "SB很多0.95",0.95},
            { "普通很多1.0",1.0},
            { "优秀很多1.05",1.05},
        };
        private double _Colleague_Enviorment_Ratio = 1.0;
        private Dictionary<string, double> _Certificate_Qualifications_List = new Dictionary<string, double>
        {
            {"无要求、二级1.0", 1.0},
            {"建造造价监理1.05", 1.05},
            {"建筑岩土结构1.1", 1.1},
            {"主任医师、教授1.15", 1.15},
        };
        private double _Certificate_Qualifications_Ratio = 1.0;
        private Dictionary<string, double> _StartWork_List = new Dictionary<string, double>
        {
            {"是",0.95 },
            {"否",1.00 },
        };
        private double _StartWork_Ratio = 1;
        private double _Result = 1.1;

        public event EventHandler? CanExecuteChanged;

        /// <summary>
        /// 平均日薪酬
        /// </summary>
        public double salary_day
        {
            get => _salary_day; set
            {
                _salary_day = value;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// 平均月薪酬
        /// </summary>
        public double salary
        {
            get => _salary; set
            {
                _salary = value;
                salary_day = (int)(_salary / 21.75 * 100) / 100.0;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// 工作时长
        /// </summary>
        public double working_hours
        {
            get => _working_hours; set
            {
                _working_hours = value;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// 通勤时长
        /// </summary>
        public double commuting_hours
        {
            get => _commuting_hours; set
            {
                _commuting_hours = value;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// 摸鱼时长
        /// </summary>
        public double fishing_hours
        {
            get => _fishing_hours; set
            {
                _fishing_hours = value;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// 综合系数
        /// </summary>
        public double comprehensive
        {
            get => _comprehensive; set
            {
                _comprehensive = value;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// 学历系数列表
        /// </summary>
        public Dictionary<string, double> Education_List
        {
            get => _Education_List; set
            {
                _Education_List = value;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// 学历系数
        /// </summary>
        public double Education_Ratio
        {
            get => _Education_Ratio; set
            {
                _Education_Ratio = value;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// 工作环境系数列表
        /// </summary>
        public Dictionary<string, double> Work_Enviorment_List
        {
            get => _Work_Enviorment_List; set
            {
                _Work_Enviorment_List = value;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// 工作环境系数
        /// </summary>
        public double Work_Enviorment_Ratio
        {
            get => _Work_Enviorment_Ratio; set
            {
                _Work_Enviorment_Ratio = value;
                comprehensive = 1 * Work_Enviorment_Ratio * Sex_Ratio * Colleague_Enviorment_Ratio;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// 异性系数列表
        /// </summary>
        public Dictionary<string, double> Sex_List
        {
            get => _Sex_List; set
            {
                _Sex_List = value;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// 异性系数
        /// </summary>
        public double Sex_Ratio
        {
            get => _Sex_Ratio; set
            {
                _Sex_Ratio = value;
                comprehensive = 1 * Work_Enviorment_Ratio * Sex_Ratio * Colleague_Enviorment_Ratio;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// 同事环境系数列表
        /// </summary>
        public Dictionary<string, double> Colleague_Enviorment_List
        {
            get => _Colleague_Enviorment_List; set
            {
                _Colleague_Enviorment_List = value;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// 同事环境系数
        /// </summary>
        public double Colleague_Enviorment_Ratio
        {
            get => _Colleague_Enviorment_Ratio; set
            {
                _Colleague_Enviorment_Ratio = value;
                comprehensive = 1 * Work_Enviorment_Ratio * Sex_Ratio * Colleague_Enviorment_Ratio;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// 职业资格系数列表
        /// </summary>
        public Dictionary<string, double> Certificate_Qualifications_List
        {
            get => _Certificate_Qualifications_List; set
            {
                _Certificate_Qualifications_List = value;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// 职业资格系数
        /// </summary>
        public double Certificate_Qualifications_Ratio
        {
            get => _Certificate_Qualifications_Ratio; set
            {
                _Certificate_Qualifications_Ratio = value;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// 上班时间列表
        /// </summary>
        public Dictionary<string, double> StartWork_List
        {
            get => _StartWork_List; set
            {
                _StartWork_List = value;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// 上班时间系数
        /// </summary>
        public double StartWork_Ratio
        {
            get => _StartWork_Ratio; set
            {
                _StartWork_Ratio = value;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// 结果
        /// </summary>
        public double Result
        {
            get => _Result; set
            {
                _Result = value;
                OnPropertyChanged();
            }
        }
        public ICommand StartCommand => new MyCommand(StartAction);
        public void StartAction(object parameter)
        {
            var res = (salary_day * comprehensive) / (35 * (working_hours + commuting_hours - 0.5 * fishing_hours) * Education_Ratio * Certificate_Qualifications_Ratio)*StartWork_Ratio;
            Result = Math.Round(res, 2);
        }
    }
}