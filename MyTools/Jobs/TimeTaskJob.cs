using MyTools.Helpers;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTools.Jobs
{
    public class TimeTaskJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            var jobDetail = context.JobDetail;
            var jobKey = jobDetail.Key;
            var jobConfig = QuartzHelper.JobConfigList.Where(o => o.Id.ToString() == jobKey.Name).FirstOrDefault();
            if (jobConfig == null) return;
            jobConfig.Execute();
        }
    }
}
