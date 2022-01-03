using System;
using NUnit.Framework;
using NUnit.Framework.Interfaces;

[assembly: Spec.Isolation.PerTestRun.StartDatabaseAction]

namespace Spec.Isolation.PerTestRun;

[AttributeUsage(AttributeTargets.Interface)]
public sealed class ResetDatabaseAfterTest : TestActionAttribute
{
    public override void AfterTest(ITest test) =>
        GlobalSystemUnderTest.Instance.Reset();

    public override ActionTargets Targets => ActionTargets.Test;
}

[AttributeUsage(AttributeTargets.Interface)]
public sealed class LogDatabase : TestActionAttribute
{
    public override void AfterTest(ITest test)
    {
        if (TestContext.CurrentContext.Result.Outcome != ResultState.Success)
        {
            var databaseLog = GlobalSystemUnderTest.Instance.Log();
            if (databaseLog != null)
            {
                TestContext.Out.WriteLine(databaseLog.Value.stdOut);
                TestContext.Error.WriteLine(databaseLog.Value.errorOut);
            }
        }
    }

    public override ActionTargets Targets => ActionTargets.Test;
}

[ResetDatabaseAfterTest, LogDatabase]
public interface IResetDatabaseAfterTest
{
}

[AttributeUsage(AttributeTargets.Assembly)]
public sealed class StartDatabaseAction : Attribute, ITestAction
{
    public void BeforeTest(ITest test) =>
        GlobalSystemUnderTest.Instance.Setup();

    public void AfterTest(ITest test) =>
        GlobalSystemUnderTest.Instance.ShutDown();

    public ActionTargets Targets => ActionTargets.Suite;
}