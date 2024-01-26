using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AssCS
{
    public class PropertiesManager
    {
        private readonly Dictionary<string, ProjectProperty> properties;

        public void Set(string name, string value)
        {
            properties[name] = new ProjectProperty(name, value);
        }

        public void Set(string name, double value)
        {
            var s = Convert.ToString(value);
            properties[name] = new ProjectProperty(name, s);
        }

        public void Set(string name, int value)
        {
            var s = Convert.ToString(value);
            properties[name] = new ProjectProperty(name, s);
        }

        public void Set(string name, bool value)
        {
            var s = Convert.ToString(value);
            properties[name] = new ProjectProperty(name, s);
        }

        public string GetString(string name)
        {
            if (!properties.ContainsKey(name)) throw new KeyNotFoundException($"Project Properties: String {name} does not exist");
            return properties[name].Value;
        }

        public double GetDouble(string name)
        {
            if (!properties.ContainsKey(name)) throw new KeyNotFoundException($"Project Properties: Double {name} does not exist");
            return Convert.ToDouble(properties[name].Value);
        }

        public int GetInt(string name)
        {
            if (!properties.ContainsKey(name)) throw new KeyNotFoundException($"Project Properties: Int {name} does not exist");
            return Convert.ToInt32(properties[name].Value);
        }

        public bool GetBool(string name)
        {
            if (!properties.ContainsKey(name)) throw new KeyNotFoundException($"Project Properties: Bool {name} does not exist");
            return Convert.ToBoolean(properties[name].Value);
        }

        public bool Has(string name)
        {
            return properties.ContainsKey(name);
        }

        public bool Remove(string name)
        {
            return properties.Remove(name);
        }

        public void Clear()
        {
            properties.Clear();
        }

        public List<ProjectProperty> GetAll()
        {
            return properties.Values.ToList();
        }

        public void LoadDefault()
        {
            Clear();
            Set("video_zoom", 0.0d);
            Set("ar_value", 0.0d);
            Set("scroll_position", 0);
            Set("active_row", 0);
            Set("ar_mode", 0);
            Set("video_position", 0);
        }

        public PropertiesManager()
        {
            properties = new Dictionary<string, ProjectProperty>();
        }
        public PropertiesManager(File source)
        {
            properties = new Dictionary<string, ProjectProperty>(source.PropertiesManager.properties);
        }
    }
}
