using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace VED.Utilities
{
    [Serializable]
    public class MeterFloat
    {
        [Serializable]
        public class Rule
        {
            // on change function here is used to validate a pending change, and alter it if need be
            public delegate float OnChangeFunction(float current, float next);

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
            public delegate void OnChangeFunction(float previous, float current);

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

        public float Value { get { return _value; } set { if (value != _value) SetValue(value); } }
        [SerializeField] private float _value = DEFAULT_MAXIMUM;

        public float Maximum { get { return _maximum; } set { if (value != _maximum) SetMaximum(value); } }
        private const float DEFAULT_MAXIMUM = 100f;
        [SerializeField] private float _maximum = DEFAULT_MAXIMUM;

        public float Minimum { get { return _minimum; } set { if (value != _minimum) SetMinimum(value); } }
        private const float DEFAULT_MINIMUM = 0f;
        [SerializeField] private float _minimum = DEFAULT_MINIMUM;

        public float Range => Maximum - Minimum;

        public float Normalised => Mathf.Clamp01((_value - _minimum) / Range);
        public float Normalise(float value) => Mathf.Clamp01((value - _minimum) / Range);

        private float Clamp(float count) => Mathf.Min(Mathf.Max(count, _minimum), _maximum);

        public MeterFloat(MeterSettings data)
        {
            _minimum = data.Minimum;
            _maximum = data.Maximum;
            _value   = Clamp(data.Initial);
        }

        public MeterFloat(float minimum = DEFAULT_MINIMUM, float maximum = DEFAULT_MAXIMUM, float initial = DEFAULT_MAXIMUM)
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

        private void SetValue(float next)
        {
            next = Clamp(next);
            if (_value == next) return;

            // validate all rules
            for (int i = _rules.Count - 1; i >= 0; i--)
            {
                next = Clamp(_rules[i].OnValueChange(_value, next));
            }

            float previous = _value;
            _value = next;

            // trigger all events
            for (int i = _events.Count - 1; i >= 0; i--)
            {
                _events[i].OnValueChange(previous, _value);
            }
        }

        private void SetMaximum(float next)
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

            float previous = _maximum;
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

        private void SetMinimum(float next)
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

            float previous = _minimum;
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