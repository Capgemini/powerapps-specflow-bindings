﻿namespace Capgemini.PowerApps.SpecFlowBindings.UiTests.Hooks
{
    using Microsoft.Xrm.Tooling.Connector;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;
    using Reqnroll;

    /// <summary>
    /// Hooks related to the mock solution used for testing.
    /// </summary>
    [Binding]
    public class MockSolutionHooks : PowerAppsStepDefiner
    {
        private const string SolutionName = "sb_PowerAppsSpecFlowBindings_Mock";

        private static string SolutionPath => Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), $"{SolutionName}.zip");

        /// <summary>
        /// Installs the mock solution to test against.
        /// If the solution is already installed in the target environment, you may wish to comment out this binding if you're running the tests locally to save time.
        /// </summary>
        [BeforeTestRun]
        public static void ImportMockSolution()
        {
            using (var serviceClient = GetServiceClient())
            {
                if (serviceClient == null && !serviceClient.IsReady)
                {
                    throw serviceClient.LastCrmException;
                }

                InstallSolution(serviceClient, SolutionPath, TimeSpan.FromMinutes(5), TimeSpan.FromSeconds(30));
            }
        }

        private static CrmServiceClient GetServiceClient()
        {
            var admin = TestConfig.GetUser("an admin");

            var connectionString =
                $"AuthType=OAuth;" +
                $"Username={admin.Username};" +
                $"Password={admin.Password};" +
                $"Url={TestConfig.GetTestUrl()};" +
                $"AppId=51f81489-12ee-4a9e-aaae-a2591f45987d;" +
                $"RedirectUri=app://58145B91-0C36-4500-8554-080854F2AC97;" +
                $"LoginPrompt=Auto";

            return new CrmServiceClient(connectionString);
        }

        private static void InstallSolution(CrmServiceClient serviceClient, string solutionPath, TimeSpan timeout, TimeSpan pollingPeriod)
        {
            var stopwatch = Stopwatch.StartNew();
            var asyncOperationId = serviceClient.ImportSolutionToCrmAsync(solutionPath, out var _);

            var solutionInstalled = false;
            do
            {
                System.Threading.Thread.Sleep((int)pollingPeriod.TotalMilliseconds);

                var result = serviceClient.GetEntityDataById("asyncoperation", asyncOperationId, new List<string> { "statecode" });
                if (result.TryGetValue("statecode", out var statecode) && statecode?.ToString() == "Completed")
                {
                    solutionInstalled = true;
                    break;
                }
                if (stopwatch.ElapsedMilliseconds > timeout.TotalMilliseconds)
                {
                    throw new TimeoutException($"Solution install timed out after {stopwatch.ElapsedMilliseconds}ms.");
                }

            } while (!solutionInstalled);

            Console.WriteLine($"Solution install completed in {stopwatch.ElapsedMilliseconds}ms.");
        }
    }
}