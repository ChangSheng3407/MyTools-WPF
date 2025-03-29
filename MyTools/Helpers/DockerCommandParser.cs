using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTools.Helpers
{
    public class DockerCommandParser
    {
        public static DockerCommandOptions Parse(string command)
        {
            var options = new DockerCommandOptions
            {
                Flags = new List<string>(),
                Args = new List<KeyValuePair<string, string>>()
            };

            // 分割命令字符串
            var args = SplitCommand(command);

            // 解析参数
            for (int i = 0; i < args.Length; i++)
            {
                var arg = args[i];
                var argNext = args[i];
                if (i < args.Length - 1) argNext = args[i + 1];
                if (arg.StartsWith("-"))
                {
                    if (argNext.StartsWith("-"))
                    {
                        options.Flags.Add(arg);
                    }
                    else
                    {
                        options.Args.Add(KeyValuePair.Create(arg.Trim().ToLower(), argNext.Trim()));
                        i++;
                    }
                }
                if (i == args.Length - 1) options.ImageName = arg;
            }
            return options;
        }

        /// <summary>
        /// 将命令字符串分割成参数列表，支持带引号的字符串
        /// </summary>
        private static string[] SplitCommand(string command)
        {
            var commandLins = command.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries).Select(o => o.Trim('\\').Trim());
            commandLins = commandLins.Where(o => !o.StartsWith("#"));
            command = string.Join(" ", commandLins);
            var matches = Regex.Matches(command, "(\"[^\"]+\")|(\\S+)");
            return matches.Cast<Match>().Select(m => m.Value.Trim()).ToArray();
        }
    }
    public class DockerCommandOptions
    {
        /// <summary>
        /// 镜像名
        /// </summary>
        public string ImageName { get; set; }
        /// <summary>
        /// 短选项（如 -it、--rm）
        /// </summary>
        public List<string> Flags { get; set; }
        /// <summary>
        /// 基础参数（如 -v、-p、-e）
        /// </summary>
        public List<KeyValuePair<string, string>> Args { get; set; }
    }
}
