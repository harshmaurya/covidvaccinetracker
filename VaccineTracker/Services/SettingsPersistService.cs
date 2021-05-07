using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace VaccineTracker.Services
{
    public class SettingsPersistService
    {
        private readonly string _settingsFile;

        public SettingsPersistService()
        {
            var dir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

            var appDirectory = Path.Combine(dir, "VaccineTracker");

            if (!Directory.Exists(appDirectory))
            {
                Directory.CreateDirectory(appDirectory);
            }

            _settingsFile = Path.Combine(dir, appDirectory, "settings.json");
        }

        public async Task<UserSettings> LoadSettingsAsync()
        {
            try
            {
                if (File.Exists(_settingsFile))
                {
                    var json = await File.ReadAllTextAsync(_settingsFile);
                    var settings = JsonSerializer.Deserialize<UserSettings>(json);
                    return settings;
                }
            }
            catch (Exception)
            {
                // ignored
            }

            return new UserSettings();
        }

        public async Task<bool> SaveSettingsAsync(UserSettings settings)
        {
            try
            {
                await using var stream = new FileStream(_settingsFile, FileMode.Create);
                await JsonSerializer.SerializeAsync(stream, settings);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
