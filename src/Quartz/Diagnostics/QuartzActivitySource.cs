using System.Diagnostics;

namespace Quartz.Diagnostics;

internal static class QuartzActivitySource
{
    internal static readonly ActivitySource Instance = new(DiagnosticHeaders.DefaultListenerName, "1.0.0");

    internal static void EnrichFrom(this Activity activity, IJobExecutionContext context)
    {
        if (activity == null)
        {
            return;
        }

        if (activity.IsAllDataRequested)
        {
            activity.AddTag(DiagnosticHeaders.SchedulerName, context.Scheduler.SchedulerName);
            activity.AddTag(DiagnosticHeaders.SchedulerId, context.Scheduler.SchedulerInstanceId);
            activity.AddTag(DiagnosticHeaders.JobType, context.JobDetail.JobType.ToString());
            activity.AddTag(DiagnosticHeaders.FireInstanceId, context.FireInstanceId);
        }

        activity.AddTag(DiagnosticHeaders.TriggerGroup, context.Trigger.Key.Group);
        activity.AddTag(DiagnosticHeaders.TriggerName, context.Trigger.Key.Name);
        activity.AddTag(DiagnosticHeaders.JobGroup, context.JobDetail.Key.Group);
        activity.AddTag(DiagnosticHeaders.JobName, context.JobDetail.Key.Name);
    }
}