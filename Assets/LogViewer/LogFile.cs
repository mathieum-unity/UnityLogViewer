using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace LogViewer
{
    public class LogFile
    {
        [Flags]
        public enum EventTypes 
        { 
            None = 0,
            Message = 1,
            Error = 2
        }

        public class Event
        {
            public readonly string Summary, Content;
            public readonly EventTypes EventType;

            public Event(string content)
            {
                Content = content.Trim(Environment.NewLine.ToCharArray());

                string[] splits = Content.Split('\n');

                Summary = splits[0];

                if (splits.Length > 1)
                    Summary += $"\n{splits[1]}";

                if (Content.Contains("Exception"))
                {
                    EventType = EventTypes.Error;
                }
                else
                {
                    EventType = EventTypes.Message;
                }
            }
        }

        public readonly int MessageCount;
        public readonly int ErrorCount;

        public readonly string Raw;

        public IReadOnlyList<Event> Events => events;

        private readonly List<Event> events = new List<Event>();

        public static LogFile LoadFromFile(string filepath)
        {
            string raw = File.ReadAllText(filepath);

            return new LogFile(raw);
        }

        public LogFile(string raw)
        {
            Raw = raw;

            Regex regexEmptyIL2CPPLines = new Regex(@"\[ line -\d+\]");

            string[] splits = raw.Split(new string[] { Environment.NewLine + Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string split in splits)
            {
                if (string.IsNullOrEmpty(split) ||
                    regexEmptyIL2CPPLines.IsMatch(split))
                {
                    continue;
                }

                Event logEvent = new Event(split);

                events.Add(logEvent);

                if (logEvent.EventType == EventTypes.Message)
                    MessageCount++;
                else
                    ErrorCount++;
            }
        }
    }
}