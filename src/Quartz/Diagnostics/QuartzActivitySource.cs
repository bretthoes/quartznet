using System.Diagnostics;

using Quartz.Impl;

namespace Quartz.Diagnostics;

internal static class QuartzActivitySource
{
    internal static readonly ActivitySource Instance = new(ActivityOptions.DefaultListenerName, ActivityOptions.Version);

    public static StartedActivity StartJobExecute(JobExecutionContextImpl jec, long startTime)
    {
        Activity? activity = Instance.CreateActivity(OperationName.Job.Execute, ActivityKind.Internal);
        if (activity == null)
        {
            return new StartedActivity(activity: null);
        }

        activity.SetStartTime(new DateTime(startTime, DateTimeKind.Utc));
        activity.EnrichFrom(jec);
        activity.Start();

        return new StartedActivity(activity);
    }

    internal static void EnrichFrom(this Activity activity, IJobExecutionContext context)
    {
        if (activity == null)
        {
            return;
        }

        if (activity.IsAllDataRequested)
        {
            activity.AddTag(ActivityOptions.SchedulerName, context.Scheduler.SchedulerName);
            activity.AddTag(ActivityOptions.SchedulerId, context.Scheduler.SchedulerInstanceId);
            activity.AddTag(ActivityOptions.JobType, context.JobDetail.JobType.ToString());
            activity.AddTag(ActivityOptions.FireInstanceId, context.FireInstanceId);
        }

        activity.AddTag(ActivityOptions.TriggerGroup, context.Trigger.Key.Group);
        activity.AddTag(ActivityOptions.TriggerName, context.Trigger.Key.Name);
        activity.AddTag(ActivityOptions.JobGroup, context.JobDetail.Key.Group);
        activity.AddTag(ActivityOptions.JobName, context.JobDetail.Key.Name);
    }
}

internal readonly struct StartedActivity
{
    private readonly Activity? activity;

    public StartedActivity(Activity? activity)
    {
        this.activity = activity;
    }

    public void Stop(long endTime, JobExecutionException? jobExEx)
    {
        if (activity == null)
        {
            return;
        }

        activity.SetEndTime(new DateTime(endTime, DateTimeKind.Utc));

        if (jobExEx != null)
        {
            activity.SetStatus(ActivityStatusCode.Error, jobExEx.Message);
        }
        activity.Stop();
    }
}