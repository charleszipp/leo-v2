using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Leo.Core.Settings
{
    public class SettingsProvider : ISettingsProvider
    {
        protected readonly IDictionary<string, string> _settings;

        public SettingsProvider(NameValueCollection settings)
        {
            _settings = new Dictionary<string, string>();
            foreach (string key in settings.Keys)
            {
                _settings.Add(key, settings[key]);
            }
        }

        public SettingsProvider(IDictionary<string, string> settings)
        {
            _settings = settings ?? new Dictionary<string, string>();
        }

        public string Get(string key, string defaultValue)
        {
            string rvalue = OnGet(key);
            if (string.IsNullOrEmpty(rvalue))
            {
                rvalue = defaultValue;
            }

            return rvalue;
        }

        public T Get<T>(string prefix)
            where T : new()
        {
            T rvalue = new T();

            foreach(var prop in typeof(T).GetProperties())
            {
                var key = $"{prefix.ToLower()}.{prop.Name.ToLower()}";
                var value = prop.GetValue(rvalue);
                if(prop.PropertyType == typeof(bool))
                {
                    prop.SetValue(rvalue, GetAsBool(key, (bool)value));
                }
                else if(prop.PropertyType == typeof(int))
                {
                    prop.SetValue(rvalue, GetAsInt(key, (int)value));
                }
                else if(prop.PropertyType == typeof(decimal))
                {
                    prop.SetValue(rvalue, GetAsDecimal(key, (decimal)value));
                }
                else if(prop.PropertyType == typeof(string))
                {
                    prop.SetValue(rvalue, Get(key, (string)value));
                }
                else
                {
                    throw new Exception($"Object property type is unsupported : {prop.PropertyType.FullName}");
                }
            }

            return rvalue;
        }

        public bool GetAsBool(string key, bool defaultValue)
        {
            bool rvalue;
            string rvalueAsString = OnGet(key);

            if (!bool.TryParse(rvalueAsString, out rvalue))
            {
                rvalue = defaultValue;
            }

            return rvalue;
        }

        public decimal GetAsDecimal(string key, decimal defaultValue)
        {
            decimal rvalue;
            string rvalueAsString = OnGet(key);

            if (!decimal.TryParse(rvalueAsString, out rvalue))
            {
                rvalue = defaultValue;
            }

            return rvalue;
        }

        public int GetAsInt(string key, int defaultValue)
        {
            int rvalue;
            string rvalueAsString = OnGet(key);

            if (!int.TryParse(rvalueAsString, out rvalue))
            {
                rvalue = defaultValue;
            }

            return rvalue;
        }

        protected virtual string OnGet(string key)
        {
            string rvalue = null;

            if (_settings.ContainsKey(key))
            {
                rvalue = _settings[key];
            }

            return rvalue;
        }
    }
}