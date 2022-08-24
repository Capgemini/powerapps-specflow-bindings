namespace Capgemini.PowerApps.SpecFlowBindings.Steps
{
    using System;
    using System.Linq;
    using Microsoft.Dynamics365.UIAutomation.Api.UCI;
    using Microsoft.Dynamics365.UIAutomation.Browser;
    using OpenQA.Selenium;
    using TechTalk.SpecFlow;

    /// <summary>
    /// Steps relating to timelines.
    /// </summary>
    [Binding]
    public class TimelineSteps : PowerAppsStepDefiner
    {
        /// <summary>
        /// Adds an appointment to the timeline. TODO: Improve duration regex.
        /// </summary>
        /// <param name="subject">Appointment subject.</param>
        /// <param name="description">Appointment description.</param>
        /// <param name="duration">Appointment duration.</param>
        /// <param name="location">Appointment location.</param>
        [When("I add an appointment to the timeline with the subject '(.*)', the description '(.*)', the duration '(.*)', and the location '(.*)'")]
        public static void WhenIAddAnAppointmentToTheTimeline(string subject, string description, string duration, string location)
        {
            XrmApp.Timeline.AddAppointment(subject, description, duration, location);
            XrmApp.Timeline.SaveAndCloseAppointment();
        }

        /// <summary>
        /// Adds a note to the timeline.
        /// </summary>
        /// <param name="title">Note title.</param>
        /// <param name="body">Note body.</param>
        [When("I add a note to the timeline with the title '(.*)' and the body '(.*)'")]
        public static void WhenIAddANoteToTheTimeline(string title, string body)
        {
            XrmApp.Timeline.AddNote(title, body);
        }

        /// <summary>
        /// Adds a phone call to the timeline. TODO: Improve duration regex.
        /// </summary>
        /// <param name="subject">Phone call subject.</param>
        /// <param name="description">Phone call description.</param>
        /// <param name="duration">Phone call duration.</param>
        /// <param name="number">Phone call number.</param>
        [When(@"I add a phone call to the timeline with the subject '(.*)', the description '(.*)', the duration '(.*)', and the number '(\d*)'")]
        public static void WhenIAddAPhoneCallToTheTimeline(string subject, string description, string duration, string number)
        {
            XrmApp.Timeline.AddPhoneCall(subject, number, description, duration);
            XrmApp.Timeline.SaveAndClosePhoneCall();
        }

        /// <summary>
        /// Adds a post to the timeline.
        /// </summary>
        /// <param name="body">The body of the post.</param>
        [When(@"I post '(.*)' to the timeline")]
        public static void WhenIPostToTheTimeline(string body)
        {
            XrmApp.Timeline.AddPost(body);
        }

        /// <summary>
        /// Adds a task to the timeline. TODO: Improve duration regex.
        /// </summary>
        /// <param name="subject">Task subject.</param>
        /// <param name="description">Task description.</param>
        /// <param name="duration">Task duration.</param>
        [When(@"I add a task to the timeline with the subject '(.*)', the description '(.*)' and the duration '(.*)'")]
        public static void WhenIAddATaskToTheTimeline(string subject, string description, string duration)
        {
            XrmApp.Timeline.AddTask(subject, description, duration);
            XrmApp.Timeline.SaveAndCloseTask();
        }

        /// <summary>
        /// Adds an email to the timeline. TODO: Improve duration regex.
        /// </summary>
        /// <param name="subject">Task subject.</param>
        /// <param name="duration">Task duration.</param>
        /// <param name="contacts">List of contacts.</param>
        [When(@"I add an email to the timeline with the subject '(.*)', the duration '(.*)', and the following contacts")]
        public static void WhenIAddAnEmailToTheTimeline(string subject, string duration, Table contacts)
        {
            if (contacts is null)
            {
                throw new ArgumentNullException(nameof(contacts));
            }

            XrmApp.Timeline.ClickEmailMenuItem();
            XrmApp.Timeline.AddEmailSubject(subject);
            XrmApp.Timeline.AddEmailDuration(duration);

            EmailContacts(
                Reference.Timeline.EmailTo,
                contacts.Rows.Where(r => r[0] == "To").Select(r => r["Name"]).ToArray());
            EmailContacts(
                Reference.Timeline.EmailCC,
                contacts.Rows.Where(r => r[0] == "CC").Select(r => r["Name"]).ToArray());
            EmailContacts(
                Reference.Timeline.EmailBcc,
                contacts.Rows.Where(r => r[0] == "BCC").Select(r => r["Name"]).ToArray());
        }

        /// <summary>
        /// Clicks the create button for the given activity.
        /// </summary>
        /// <param name="activityName">The name of the activity entity.</param>
        [When(@"I click create for (?:a|an) '(.*)' on the timeline")]
        public static void WhenIClickCreateOnTheTimeline(string activityName)
        {
            Driver.ClickWhenAvailable(By.XPath(Elements.Xpath[Reference.Timeline.Popout]));
            Driver.ClickWhenAvailable(By.XPath($"//li[contains(@id,\"notescontrol-createNewRecord_flyoutMenuItem_{activityName}\")]"));
        }

        private static void EmailContacts(string reference, string[] contacts)
        {
            if (contacts.Length == 0)
            {
                return;
            }

            XrmApp.Timeline.AddEmailContacts(
                contacts.Select(c => new LookupItem { Name = Elements.ElementId[reference], Value = c }).ToArray(),
                true);
        }
    }
}
