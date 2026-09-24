// Analytics.cs
// One wrapper for every analytics SDK (SOP section 10). SDK adapters implement IAnalyticsSink.
// Nothing is sent before consent. Names not in AnalyticsEvents.All are refused loudly, because at least one
// SDK refuses them silently. Choke-point events accumulate by reason and flush on a boundary
// (a save, a pause, a scene change): 10,000 coin pickups become one event, same total.
using System;
using System.Collections.Generic;
using UnityEngine;

namespace __NAMESPACE__
{
    public interface IAnalyticsSink
    {
        void Send(string eventName, Dictionary<string, object> properties);
        void SetUserProperty(string key, string value);
    }

    public static class Analytics
    {
        static readonly List<IAnalyticsSink> sinks = new List<IAnalyticsSink>();
        static readonly Dictionary<string, double> batched = new Dictionary<string, double>();

        public static void Register(IAnalyticsSink sink) { if (sink != null && !sinks.Contains(sink)) sinks.Add(sink); }

        public static void Track(string eventName, Dictionary<string, object> properties = null)
        {
            if (!Consent.Granted) return;
            if (Array.IndexOf(AnalyticsEvents.All, eventName) < 0)
            {
                Debug.LogWarning("[Analytics] refused unregistered event '" + eventName + "'. Add it to AnalyticsEvents.All and the SDK allow-list.");
                return;
            }
            for (int i = 0; i < sinks.Count; i++) sinks[i].Send(eventName, properties);
        }

        public static void Accumulate(string reason, double amount)
        {
            batched.TryGetValue(reason, out double current);
            batched[reason] = current + amount;
        }

        public static void Flush(string eventName)
        {
            foreach (var kv in batched)
                Track(eventName, new Dictionary<string, object> { { "reason", kv.Key }, { "amount", kv.Value } });
            batched.Clear();
        }

        public static void SetUserProperty(string key, string value)
        {
            if (!Consent.Granted) return;
            for (int i = 0; i < sinks.Count; i++) sinks[i].SetUserProperty(key, value);
        }
    }
}
