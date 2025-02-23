using HandyControl.Controls;
using LiteDB;
using MyTools.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTools.Model
{
    public class TimeTaskJobConfig : QuartzJobConfig
    {
        public new ObjectId Id { get => base.Id as ObjectId; set => base.Id = value; }

        public override void Execute()
        {
            string result = string.Empty;
            switch (ExecuteType)
            {
                case ExecuteTypeEnum.CMD:
                case ExecuteTypeEnum.HTTP:
                    result = CommonHelper.RunCMD(ExecuteContent);
                    result = $"{JobName}任务执行结束{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}\n{result}";
                    break;
                case ExecuteTypeEnum.REMIND:
                    result = ExecuteContent;
                    break;
                default:
                    Growl.Error("错误的执行类型");
                    break;
            }
            Growl.Info(result);
        }
    }
}
