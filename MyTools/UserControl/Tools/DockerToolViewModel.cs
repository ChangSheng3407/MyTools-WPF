using AngleSharp.Text;
using HandyControl.Controls;
using ICSharpCode.AvalonEdit.Document;
using Masuit.Tools;
using MyTools.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MyTools.UserControl.Tools
{
    public class DockerToolViewModel : BaseViewModel
    {
        private DockerConfig _InsertConfig = new DockerConfig();
        private IDocument _Command = new TextDocument();
        private bool _Detach;
        private bool _InteractiveTTy;
        private string _ImageName;

        public DockerConfig AddConfig
        {
            get => _InsertConfig;
            set
            {
                _InsertConfig = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<DockerConfig> DockerConfigs { get; set; } = new ObservableCollection<DockerConfig>();
        public IDocument Command
        {
            get => _Command;
            set
            {
                _Command = value;
                OnPropertyChanged();
            }
        }
        public bool Detach
        {
            get => _Detach;
            set
            {
                _Detach = value;
                OnPropertyChanged();
            }
        }
        public bool InteractiveTTy
        {
            get => _InteractiveTTy;
            set
            {
                _InteractiveTTy = value;
                OnPropertyChanged();
            }
        }
        public string ImageName
        {
            get => _ImageName;
            set
            {
                _ImageName = value;
                OnPropertyChanged();
            }
        }

        public ICommand Add => new MyCommand(o =>
        {
            if (AddConfig.Value.IsNullOrEmpty())
            {
                Growl.Error("参数错误");
                return;
            }
            DockerConfigs.Add(AddConfig.DeepClone());
            AddConfig = new DockerConfig
            {
                Parameter = AddConfig.Parameter
            };
            BuildDockerRunCommand();
        });

        public ICommand Delete => new MyCommand(o =>
        {
            DockerConfigs.Remove((DockerConfig)o);
            BuildDockerRunCommand();
        });

        public ICommand ReBuildDockerRunCommand => new MyCommand(o =>
        {
            BuildDockerRunCommand();
        });
        public ICommand AnalysisDockerRunCommand => new MyCommand(o =>
        {
            Detach = false;
            InteractiveTTy = false;
            var option = DockerCommandParser.Parse(Command.Text);
            DockerConfigs.Clear();
            foreach (var item in option.Args)
            {
                DockerConfigs.Add(DockerConfig.Create(item.Key, item.Value));
            }
            foreach (var item in option.Flags)
            {
                if (item == "-d")
                {
                    Detach = true;
                    continue;
                }
                if (item == "-it")
                {
                    InteractiveTTy = true;
                    continue;
                }
                if (item == "-dit" || item == "-itd")
                {
                    Detach = true;
                    InteractiveTTy = true;
                    continue;
                }
                DockerConfigs.Add(DockerConfig.Create("", item));
            }
            ImageName = option.ImageName;
        });

        public void BuildDockerRunCommand()
        {
            var command = "docker run";
            if (Detach && InteractiveTTy) command += " -dit";
            else if (Detach) command += " -d";
            else if (InteractiveTTy) command += " -it";
            foreach (var config in DockerConfigs)
            {
                command += $" {config.Parameter} {config.Value}";
            }
            command += $" {ImageName}";
            Command.Text = command;
        }
    }
    public class DockerConfig
    {

        public string Parameter { get; set; }
        public string Value { get; set; }
        public static DockerConfig Create(string parameter, string value)
        {
            return new DockerConfig
            {
                Parameter = parameter,
                Value = value
            };
        }
    }
}
