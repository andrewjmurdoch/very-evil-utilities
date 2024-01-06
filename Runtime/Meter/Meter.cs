using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace VED.Utilities
{
    [Serializable]
    public class Meter
    {
        [Serializable]
        public class Rule
        {
            // on change function here is used to validate a pending change, and alter it if need be
            public delegate int OnChangeFunction(int current, int next);

            public OnChangeFunction OnValueChange = null;
            public OnChangeFunction OnMaximumChange = null;
            public OnChangeFunction OnMinimumChange = null;
            [SerializeField] public int Priority = 0;

            public Rule(OnChangeFunction onCountChange = null, OnChangeFunction onMaximumChange = null, OnChangeFunction onMinimumChange = null, int priority = 0)
            {
                OnValueChange = onCountChange;
                OnMaximumChange = onMaximumChange;
                OnMinimumChange = onMinimumChange;
                Priority = priority;
            }
        }

        [Serializable]
        public class Event
        {
            // on change function here is used to indicate a change has been validated and occured
            public delegate void OnChangeFunction(int previous, int current);

            public OnChangeFunction OnValueChange = null;
            public OnChangeFunction OnMaximumChange = null;
            public OnChangeFunction OnMinimumChange = null;
            [SerializeField] public int Priority = 0;

            public Event(OnChangeFunction onCountChange = null, OnChangeFunction onMaximumChange = null, OnChangeFunction onMinimumChange = null, int priority = 0)
            {
                OnValueChange = onCountChange;
                OnMaximumChange = onMaximumChange;
                OnMinimumChange = onMinimumChange;
                Priority = priority;
            }
        }

        public List<Rule> Rules => _rules;
        [SerializeField] private List<Rule> _rules = new List<Rule>();

        public List<Event> Events => _events;
        [SerializeField] private List<Event> _events = new List<Event>();

        public int Value { get { return _value; } set { if (value != _value) SetValue(value); } }
        [SerializeField] private int _value = DEFAULT_MAXIMUM;

        public int Maximum { get { return _maximum; } set { if (value != _maximum) SetMaximum(value); } }
        private const int DEFAULT_MAXIMUM = 100;
        [SerializeField] private int _maximum = DEFAULT_MAXIMUM;

        public int Minimum { get { return _minimum; } set { if (value != _minimum) SetMinimum(value); } }
        private const int DEFAULT_MINIMUM = 0;
        [SerializeField] private int _minimum = DEFAULT_MINIMUM;

        public int Range => Maximum - Minimum;

        public float Normalised => Mathf.Clamp01((float)(_value - _minimum) / (float)Range);
        public float Normalise(int value) => Mathf.Clamp01((float)(value - _minimum) / (float)Range);

        private int Clamp(int count) => Math.Min(Math.Max(count, _minimum), _maximum);

        public Meter(MeterSettings data)
        {
            _minimum = data.Minimum;
            _maximum = data.Maximum;
            _value   = Clamp(data.Initial);
        }

        public Meter(int minimum = DEFAULT_MINIMUM, int maximum = DEFAULT_MAXIMUM, int initial = DEFAULT_MAXIMUM)
        {
            _minimum = minimum;
            _maximum = maximum;
            _value   = Clamp(initial);
        }

        public Rule AddRule(Rule.OnChangeFunction onCountChange = null, Rule.OnChangeFunction onMaximumChange = null, Rule.OnChangeFunction onMinimumChange = null, int priority = 0)
        {
            return AddRule(new Rule(onCountChange, onMaximumChange, onMinimumChange, priority));
        }

        public Rule AddRule(Rule meterRule)
        {
            _rules.Add(meterRule);
            _rules = _rules.OrderBy(r => r.Priority).ToList();
            return meterRule;
        }

        public bool RemoveRule(Rule meterRule) => _rules.Remove(meterRule);

        public Event AddEvent(Event.OnChangeFunction onCountChange = null, Event.OnChangeFunction onMaximumChange = null, Event.OnChangeFunction onMinimumChange = null, int priority = 0)
        {
            return AddEvent(new Event(onCountChange, onMaximumChange, onMinimumChange, priority));
        }

        public Event AddEvent(Event meterEvent)
        {
            _events.Add(meterEvent);
            _events = _events.OrderBy(e => e.Priority).ToList();
            return meterEvent;
        }

        public bool RemoveEvent(Event meterEvent) => _events.Remove(meterEvent);

        private void SetValue(int next)
        {
            next = Clamp(next);
            if (_value == next) return;

            // validate all rules
            for (int i = _rules.Count - 1; i >= 0; i--)
            {
                next = Clamp(_rules[i].OnValueChange(_value, next));
            }

            int previous = _value;
            _value = next;

            // trigger all events
            for (int i = _events.Count - 1; i >= 0; i--)
            {
                _events[i].OnValueChange(previous, _value);
            }
        }

        private void SetMaximum(int next)
        {
            next = Mathf.Max(next, _minimum + 1);
            if (_maximum == next) return;

            // validate all rules
            for (int i = _rules.Count - 1; i >= 0; i--)
            {
                if (_rules[i].OnMaximumChange != null)
                {
                    next = Clamp(_rules[i].OnMaximumChange(_maximum, next));
                }
            }

            int previous = _maximum;
            _maximum = next;

            // trigger all events
            for (int i = _events.Count - 1; i >= 0; i--)
            {
                if (_events[i].OnMaximumChange != null)
                {
                    _events[i].OnMaximumChange(previous, _maximum);
                }
            }
        }

        private void SetMinimum(int next)
        {
            next = Mathf.Min(next, _maximum - 1);
            if (_minimum == next) return;

            // validate all rules
            for (int i = _rules.Count - 1; i >= 0; i--)
            {
                if (_rules[i].OnMinimumChange != null)
                {
                    next = Clamp(_rules[i].OnMinimumChange(_minimum, next));
                }
            }

            int previous = _minimum;
            _minimum = next;

            // trigger all events
            for (int i = _events.Count - 1; i >= 0; i--)
            {
                if (_events[i].OnMinimumChange != null)
                {
                    _events[i].OnMinimumChange(previous, _minimum);
                }
            }
        }
    }
}