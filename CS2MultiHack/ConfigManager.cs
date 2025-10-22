using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace CS2MultiHack
{
    public class ConfigManager
    {
        private static readonly string ConfigFolder = @"C:\MissClient\cfg";
        private static readonly string DefaultConfigPath = Path.Combine(ConfigFolder, "default.cfg");

        // Dictionary to store all configuration values
        public Dictionary<string, string> ConfigValues { get; set; } = new Dictionary<string, string>();

        // Singleton instance
        private static ConfigManager _instance;
        public static ConfigManager Instance => _instance ?? (_instance = new ConfigManager());

        private ConfigManager()
        {
            // Create directory if it doesn't exist
            EnsureDirectoryExists();

            // Load default configuration if exists
            if (File.Exists(DefaultConfigPath))
            {
                Load();
            }
        }

        private void EnsureDirectoryExists()
        {
            try
            {
                if (!Directory.Exists(ConfigFolder))
                {
                    Directory.CreateDirectory(ConfigFolder);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating config directory: {ex.Message}");
            }
        }

        // Save configuration to file
        public void Save()
        {
            try
            {
                EnsureDirectoryExists();

                using (StreamWriter writer = new StreamWriter(DefaultConfigPath))
                {
                    foreach (var pair in ConfigValues)
                    {
                        writer.WriteLine($"{pair.Key}={pair.Value}");
                    }
                }
                Console.WriteLine("Configuration saved successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving configuration: {ex.Message}");
            }
        }

        // Load configuration from file
        public void Load()
        {
            try
            {
                ConfigValues.Clear();
                if (File.Exists(DefaultConfigPath))
                {
                    string[] lines = File.ReadAllLines(DefaultConfigPath);
                    foreach (string line in lines)
                    {
                        if (string.IsNullOrWhiteSpace(line) || !line.Contains("="))
                            continue;

                        int separatorIndex = line.IndexOf('=');
                        if (separatorIndex > 0)
                        {
                            string key = line.Substring(0, separatorIndex);
                            string value = line.Substring(separatorIndex + 1);
                            ConfigValues[key] = value;
                        }
                    }
                    Console.WriteLine("Configuration loaded successfully.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading configuration: {ex.Message}");
            }
        }

        // Get a string value from config
        public string GetString(string key, string defaultValue = "")
        {
            return ConfigValues.TryGetValue(key, out string value) ? value : defaultValue;
        }

        // Set a string value in config
        public void SetString(string key, string value)
        {
            ConfigValues[key] = value;
        }

        // Get a boolean value from config
        public bool GetBool(string key, bool defaultValue = false)
        {
            if (ConfigValues.TryGetValue(key, out string value))
            {
                return bool.TryParse(value, out bool result) ? result : defaultValue;
            }
            return defaultValue;
        }

        // Set a boolean value in config
        public void SetBool(string key, bool value)
        {
            ConfigValues[key] = value.ToString().ToLower();
        }

        // Get a float value from config
        public float GetFloat(string key, float defaultValue = 0f)
        {
            if (ConfigValues.TryGetValue(key, out string value))
            {
                return float.TryParse(value, out float result) ? result : defaultValue;
            }
            return defaultValue;
        }

        // Set a float value in config
        public void SetFloat(string key, float value)
        {
            ConfigValues[key] = value.ToString();
        }

        // Get an integer value from config
        public int GetInt(string key, int defaultValue = 0)
        {
            if (ConfigValues.TryGetValue(key, out string value))
            {
                return int.TryParse(value, out int result) ? result : defaultValue;
            }
            return defaultValue;
        }

        // Set an integer value in config
        public void SetInt(string key, int value)
        {
            ConfigValues[key] = value.ToString();
        }

        // Get a Vector4 value from config
        public Vector4 GetVector4(string key, Vector4 defaultValue)
        {
            if (ConfigValues.TryGetValue(key, out string value))
            {
                string[] components = value.Split(',');
                if (components.Length == 4 &&
                    float.TryParse(components[0], out float x) &&
                    float.TryParse(components[1], out float y) &&
                    float.TryParse(components[2], out float z) &&
                    float.TryParse(components[3], out float w))
                {
                    return new Vector4(x, y, z, w);
                }
            }
            return defaultValue;
        }

        // Set a Vector4 value in config
        public void SetVector4(string key, Vector4 value)
        {
            ConfigValues[key] = $"{value.X},{value.Y},{value.Z},{value.W}";
        }
    }
}
