using Masuit.Tools;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Newtonsoft.Json;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace MyTools.Helpers
{
    public static class QuartzHelper
    {
        private static StdSchedulerFactory? _factory;
        private static string jobPath = "Job";
        /// <summary>
        /// 调度器是否启动
        /// </summary>
        public static bool ScheduleIsRunning() => Scheduler?.IsStarted ?? false;
        /// <summary>
        /// Job配置数据列表
        /// </summary>
        public static List<QuartzJobConfig> JobConfigList = new List<QuartzJobConfig>();
        public static StdSchedulerFactory? factory
        {
            get
            {
                if (_factory == null)
                {
                    InitQuartz();
                }
                return _factory;
            }
            set => _factory = value;
        }
        public static IScheduler Scheduler
        {
            get
            {
                var scheduler = factory.GetScheduler().Result;
                if (scheduler == null)
                {
                    InitQuartz();
                }
                return scheduler!;
            }
        }
        /// <summary>
        /// 初始化Quartz
        /// </summary>
        /// <returns></returns>
        public static void InitQuartz()
        {
            factory = new StdSchedulerFactory();
        }
        /// <summary>
        /// 启动Quartz
        /// </summary>
        /// <returns></returns>
        public static void StartQuartz<T, T2>(ICollection<T> jobConfigs) where T : QuartzJobConfig where T2 : IJob
        {
            Scheduler.Start().Wait();
            foreach (var item in jobConfigs)
            {
                if (item.Enable)
                {
                    AddJob<T2>(item);
                }
            }
        }
        /// <summary>
        /// 关闭Quartz
        /// </summary>
        /// <returns></returns>
        public static void ShutdownQuartz()
        {
            Scheduler.Shutdown().Wait();
            factory = null;
            JobConfigList.Clear();
        }
        /// <summary>
        /// 添加Job
        /// </summary>
        /// <param name="jobDetail"></param>
        /// <param name="trigger"></param>
        /// <returns></returns>
        public static void AddJob(IJobDetail jobDetail, ITrigger trigger)
        {
            Scheduler.ScheduleJob(jobDetail, trigger).Wait();
        }
        /// <summary>
        /// 添加Job
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jobName">任务名</param>
        /// <param name="intervalEnum">触发时间类型：秒、分钟、小时</param>
        /// <param name="crontab">Cron表达式</param>
        /// <param name="interval">间隔时间</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static void AddJob<T>(QuartzJobConfig jobConfig) where T : IJob
        {
            //调度器未启动则不加载Job
            if (!ScheduleIsRunning() || jobConfig.Enable == false) return;
            var crontabStr = jobConfig.ExpString;
            if (string.IsNullOrEmpty(crontabStr)) throw new ArgumentException("未设置Cron表达式");
            var jobDetail = BuildJobDetail<T>(jobConfig.Id.ToString());
            ITrigger trigger = BuildTrigger(jobConfig.Id.ToString(), crontabStr);
            Scheduler.ScheduleJob(jobDetail, trigger).Wait();
            JobConfigList.Add(jobConfig);
        }
        /// <summary>
        /// 移除Job
        /// </summary>
        /// <param name="jobName"></param>
        /// <returns></returns>
        public static void RemoveJob(QuartzJobConfig jobConfig)
        {
            Scheduler.DeleteJob(new JobKey(jobConfig.Id.ToString())).Wait();
            JobConfigList.Remove(jobConfig);
        }
        /// <summary>
        /// 暂停Job
        /// </summary>
        /// <param name="jobName"></param>
        /// <returns></returns>
        public static void PauseJob(string jobName)
        {
            Scheduler.PauseJob(new JobKey(jobName)).Wait();
        }
        /// <summary>
        /// 恢复Job
        /// </summary>
        /// <param name="jobName"></param>
        /// <returns></returns>
        public static void ResumeJob(string jobName)
        {
            Scheduler.ResumeJob(new JobKey(jobName)).Wait();
        }
        /// <summary>
        /// 构建JobDetail
        /// </summary>
        /// <typeparam name="TJob"></typeparam>
        /// <param name="jobName"></param>
        /// <returns></returns>
        public static IJobDetail BuildJobDetail<TJob>(string jobName) where TJob : IJob
        {
            IJobDetail job = JobBuilder.Create<TJob>()
              .WithIdentity(jobName)
              .Build();
            return job;
        }
        /// <summary>
        /// 构建Trigger 单位：秒
        /// </summary>
        /// <param name="triggerName"></param>
        /// <param name="intervalEnum"></param>
        /// <param name="interval">秒</param>
        /// <returns></returns>
        public static ITrigger BuildTrigger(string triggerName, int interval = 0)
        {
            if (interval == 0) throw new ArgumentException("interval必须大于0");
            ITrigger trigger = TriggerBuilder.Create()
                                             .WithIdentity(triggerName)
                                             .StartNow()
                                             .WithSimpleSchedule(x => x.WithIntervalInSeconds(interval).RepeatForever())
                                             .Build();
            return trigger;
        }
        /// <summary>
        /// 通过Cron构建Trigger
        /// </summary>
        /// <param name="triggerName"></param>
        /// <param name="intervalEnum"></param>
        /// <param name="interval"></param>
        /// <returns></returns>
        public static ITrigger BuildTrigger(string triggerName, string crontab = "")
        {
            if (!CronExpression.IsValidExpression(crontab)) throw new ArgumentException("crontab格式不正确");
            ITrigger trigger = TriggerBuilder.Create().WithIdentity(triggerName).StartNow().WithCronSchedule(crontab).Build();
            return trigger;
        }
        /// <summary>
        /// Cron表达式是否有效
        /// </summary>
        /// <param name="crontab"></param>
        /// <returns></returns>
        public static bool CronValid(this string crontab) => CronExpression.IsValidExpression(crontab);
        /// <summary>
        /// 获取Job配置数据
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        /// <exception cref="FileNotFoundException"></exception>
        public static QuartzJobConfig GetJobConfig(this IJobExecutionContext context)
        {
            var jobName = context.JobDetail.Key.Name;
            string dirPath = Path.Combine(jobPath, jobName);
            if (!Directory.Exists(dirPath)) throw new FileNotFoundException($"Job {jobName}目录不存在");
            string dataPath = Path.Combine(dirPath, "data.json");
            if (!File.Exists(dataPath)) throw new FileNotFoundException($"Job {jobName}配置数据不存在");
            string json = File.ReadAllText(dataPath);
            QuartzJobConfig config = JsonConvert.DeserializeObject<QuartzJobConfig>(json) ?? new QuartzJobConfig();
            return config;
        }
        /// <summary>
        /// 获取Cron下次运行时间，默认获取5次
        /// </summary>
        /// <param name="crontab"></param>
        /// <param name="times"></param>
        /// <returns></returns>
        public static string GetCronNextRunTime(string crontab, int times = 5)
        {
            if (!crontab.CronValid()) throw new ArgumentException("crontab格式不正确");
            string result = "";
            CronExpression expression = new CronExpression(crontab);
            DateTimeOffset? nextRunTime = DateTime.Now;
            for (int i = 0; i < times; i++)
            {
                nextRunTime = expression.GetNextValidTimeAfter((DateTimeOffset)nextRunTime);
                if (!nextRunTime.HasValue) throw new Exception("运行失败");
                result += nextRunTime.Value.AddHours(8).ToString("yyyy-MM-dd HH:mm:ss") + "\n";
            }
            return result;
        }
    }
    /// <summary>
    /// 执行类型枚举
    /// </summary>
    public enum ExecuteTypeEnum
    {
        /// <summary>
        /// Cmd
        /// </summary>
        CMD,
        /// <summary>
        /// Http
        /// </summary>
        HTTP,
        /// <summary>
        /// 提醒
        /// </summary>
        REMIND
    }
    /// <summary>
    /// Job配置数据
    /// </summary>
    public class QuartzJobConfig
    {
        public object Id { get; set; }
        /// <summary>
        /// 任务名
        /// </summary>
        public string JobName { get; set; }
        /// <summary>
        /// 触发时间类型
        /// </summary>
        //public IntervalEnum IntervalType { get; set; }
        /// <summary>
        /// 触发时间表达式
        /// Cron表达式：0/5 * * * * ?
        /// 其他：1
        /// </summary>
        public string ExpString { get; set; } = "0/5 * * * * ?";
        /// <summary>
        /// 执行类型
        /// </summary>
        public ExecuteTypeEnum ExecuteType { get; set; }
        /// <summary>
        /// 执行内容
        /// </summary>
        public string ExecuteContent { get; set; }
        /// <summary>
        /// 启用/禁用
        /// </summary>
        public bool Enable { get; set; }
        /// <summary>
        /// 执行
        /// </summary>
        public virtual void Execute() { }
    }
}
