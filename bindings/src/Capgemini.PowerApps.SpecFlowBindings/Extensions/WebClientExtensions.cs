namespace Capgemini.PowerApps.SpecFlowBindings.Extensions
{
    using System;
    using System.Linq;
    using Microsoft.Dynamics365.UIAutomation.Api.UCI;
    using Microsoft.Dynamics365.UIAutomation.Browser;
    using OpenQA.Selenium;

    /// <summary>
    /// Extensions to the <see cref="WebClient"/> class.
    /// </summary>
    internal static class WebClientExtensions
    {
        /// <summary>
        /// Temporary workaround until https://github.com/microsoft/EasyRepro/issues/1087 is resolved.
        /// </summary>
        /// <param name="webClient">The <see cref="WebClient"/>.</param>
        /// <param name="subGridName">The name of the subgrid.</param>
        /// <param name="name">The name of the command.</param>
        /// <param name="subName">The name of the sub-command.</param>
        /// <param name="subSecondName">The name of the second sub-command.</param>
        /// <returns>The <see cref="BrowserCommandResult{TReturn}"/>.</returns>
        internal static BrowserCommandResult<bool> ClickSubGridCommandV2(this WebClient webClient, string subGridName, string name, string subName = null, string subSecondName = null)
        {
            return webClient.Execute(GetOptions("Click SubGrid Command"), driver =>
            {
                var subGrid = driver.FindElement(By.XPath(AppElements.Xpath[AppReference.Entity.SubGridContents].Replace("[NAME]", subGridName)));

                if (subGrid.TryFindElement(By.XPath(AppElements.Xpath[AppReference.Entity.SubGridCommandBar].Replace("[NAME]", subGridName)), out var subGridCommandBar))
                {
                    var items = subGridCommandBar.FindElements(By.TagName("button"));
                    if (items.Any(x => x.GetAttribute("aria-label").Equals(name, StringComparison.OrdinalIgnoreCase)))
                    {
                        items.FirstOrDefault(x => x.GetAttribute("aria-label").Equals(name, StringComparison.OrdinalIgnoreCase)).Click(true);
                        driver.WaitForTransaction();
                    }
                    else
                    {
                        if (items.Any(x => x.GetAttribute("aria-label").Contains("More Commands", StringComparison.OrdinalIgnoreCase)))
                        {
                            items.FirstOrDefault(x => x.GetAttribute("aria-label").Contains("More Commands", StringComparison.OrdinalIgnoreCase)).Click(true);
                            driver.WaitForTransaction();

                            var overflowContainer = driver.FindElement(By.XPath(AppElements.Xpath[AppReference.Entity.SubGridOverflowContainer]));

                            if (overflowContainer.HasElement(By.XPath(AppElements.Xpath[AppReference.Entity.SubGridOverflowButton].Replace("[NAME]", name))))
                            {
                                overflowContainer.FindElement(By.XPath(AppElements.Xpath[AppReference.Entity.SubGridOverflowButton].Replace("[NAME]", name))).Click(true);
                                driver.WaitForTransaction();
                            }
                            else
                            {
                                throw new InvalidOperationException($"No command with the name '{name}' exists inside of {subGridName} Commandbar.");
                            }
                        }
                        else
                        {
                            throw new InvalidOperationException($"No command with the name '{name}' exists inside of {subGridName} CommandBar.");
                        }
                    }

                    if (subName != null)
                    {
                        var overflowContainer = driver.FindElement(By.XPath(AppElements.Xpath[AppReference.Entity.SubGridOverflowContainer]));

                        if (overflowContainer.HasElement(By.XPath(AppElements.Xpath[AppReference.Entity.SubGridOverflowButton].Replace("[NAME]", subName))))
                        {
                            overflowContainer.FindElement(By.XPath(AppElements.Xpath[AppReference.Entity.SubGridOverflowButton].Replace("[NAME]", subName))).Click(true);
                            driver.WaitForTransaction();
                        }
                        else
                        {
                            throw new InvalidOperationException($"No command with the name '{subName}' exists under the {name} command inside of {subGridName} Commandbar.");
                        }

                        if (subSecondName != null)
                        {
                            overflowContainer = driver.FindElement(By.XPath(AppElements.Xpath[AppReference.Entity.SubGridOverflowContainer]));

                            if (overflowContainer.HasElement(By.XPath(AppElements.Xpath[AppReference.Entity.SubGridOverflowButton].Replace("[NAME]", subSecondName))))
                            {
                                overflowContainer.FindElement(By.XPath(AppElements.Xpath[AppReference.Entity.SubGridOverflowButton].Replace("[NAME]", subSecondName))).Click(true);
                                driver.WaitForTransaction();
                            }
                            else
                            {
                                throw new InvalidOperationException($"No command with the name '{subSecondName}' exists under the {subName} command inside of {name} on the {subGridName} SubGrid Commandbar.");
                            }
                        }
                    }
                }
                else
                {
                    throw new InvalidOperationException($"Unable to locate the Commandbar for the {subGrid} SubGrid.");
                }

                return true;
            });
        }

        /// <summary>
        /// Temporary workaround until https://github.com/microsoft/EasyRepro/issues/1087 is resolved.
        /// </summary>
        /// <param name="webClient">The <see cref="WebClient"/>.</param>
        /// <param name="name">The name of the command.</param>
        /// <param name="subname">The name of the sub-command.</param>
        /// <param name="subSecondName">The name of the second sub-command.</param>
        /// <param name="thinkTime">The think time.</param>
        /// <returns>The <see cref="BrowserCommandResult{TReturn}"/>.</returns>
        internal static BrowserCommandResult<bool> ClickCommandV2(this WebClient webClient, string name, string subname = null, string subSecondName = null, int thinkTime = Constants.DefaultThinkTime)
        {
            return webClient.Execute(GetOptions($"Click Command"), driver =>
            {
                var ribbon = driver.WaitUntilAvailable(
                    By.XPath(AppElements.Xpath[AppReference.CommandBar.Container]),
                    TimeSpan.FromSeconds(5));

                if (ribbon == null)
                {
                    ribbon = driver.WaitUntilAvailable(
                        By.XPath(AppElements.Xpath[AppReference.CommandBar.ContainerGrid]),
                        TimeSpan.FromSeconds(5),
                        "Unable to find the ribbon.");
                }

                var items = ribbon.FindElements(By.TagName("button"));

                if (items.Any(x => x.GetAttribute("aria-label").Equals(name, StringComparison.OrdinalIgnoreCase)))
                {
                    items.FirstOrDefault(x => x.GetAttribute("aria-label").Equals(name, StringComparison.OrdinalIgnoreCase)).Click(true);
                    driver.WaitForTransaction();
                }
                else
                {
                    if (items.Any(x => x.GetAttribute("aria-label").Contains("More Commands", StringComparison.OrdinalIgnoreCase)))
                    {
                        items.FirstOrDefault(x => x.GetAttribute("aria-label").Contains("More Commands", StringComparison.OrdinalIgnoreCase)).Click(true);
                        driver.WaitForTransaction();

                        if (driver.HasElement(By.XPath(AppElements.Xpath[AppReference.CommandBar.Button].Replace("[NAME]", name))))
                        {
                            driver.FindElement(By.XPath(AppElements.Xpath[AppReference.CommandBar.Button].Replace("[NAME]", name))).Click(true);
                            driver.WaitForTransaction();
                        }
                        else
                        {
                            throw new InvalidOperationException($"No command with the name '{name}' exists inside of Commandbar.");
                        }
                    }
                    else
                    {
                        throw new InvalidOperationException($"No command with the name '{name}' exists inside of Commandbar.");
                    }
                }

                if (!string.IsNullOrEmpty(subname))
                {
                    var submenu = driver.WaitUntilAvailable(By.XPath(AppElements.Xpath[AppReference.CommandBar.MoreCommandsMenu]));

                    var subbutton = submenu.FindElements(By.TagName("button")).FirstOrDefault(x => x.Text == subname);

                    if (subbutton != null)
                    {
                        subbutton.Click(true);
                    }
                    else
                    {
                        throw new InvalidOperationException($"No sub command with the name '{subname}' exists inside of Commandbar.");
                    }

                    if (!string.IsNullOrEmpty(subSecondName))
                    {
                        var subSecondmenu = driver.WaitUntilAvailable(By.XPath(AppElements.Xpath[AppReference.CommandBar.MoreCommandsMenu]));

                        var subSecondbutton = subSecondmenu.FindElements(By.TagName("button")).FirstOrDefault(x => x.Text == subSecondName);

                        if (subSecondbutton != null)
                        {
                            subSecondbutton.Click(true);
                        }
                        else
                        {
                            throw new InvalidOperationException($"No sub command with the name '{subSecondName}' exists inside of Commandbar.");
                        }
                    }
                }

                driver.WaitForTransaction();

                return true;
            });
        }

        private static BrowserCommandOptions GetOptions(string commandName)
        {
            return new BrowserCommandOptions(
                Constants.DefaultTraceSource,
                commandName,
                Constants.DefaultRetryAttempts,
                Constants.DefaultRetryDelay,
                null,
                true,
                typeof(NoSuchElementException),
                typeof(StaleElementReferenceException));
        }
    }
}