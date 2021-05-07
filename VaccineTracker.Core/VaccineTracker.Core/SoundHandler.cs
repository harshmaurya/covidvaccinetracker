using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using NAudio.Wave;
using VaccineTracker.Core.Domain;
using VaccineTracker.Core.Dto;

namespace VaccineTracker.Core
{
    public class SoundHandler : IAlertHandler
    {
        public void HandleVaccineAvailability(VaccineAvailability vaccineAvailability)
        {
            if (!vaccineAvailability.AvailableSlots.Any()) return;
            PlaySound();
        }

        public void HandlerError(Exception exception)
        {
            
        }

        private static void PlaySound()
        {
            try
            {
                const string soundFile = "alarm.WAV";
                var soundFilePath = Path.Combine(GetBasePath(), soundFile);
                using var audioFile = new AudioFileReader(soundFilePath);
                using var outputDevice = new WaveOutEvent();
                outputDevice.Init(audioFile);
                outputDevice.Play();
                while (outputDevice.PlaybackState == PlaybackState.Playing)
                {
                    Thread.Sleep(1000);
                }
            }
            catch (Exception e)
            {
                //
            }
        }

        private static string GetBasePath()
        {
            using var processModule = Process.GetCurrentProcess().MainModule;
            return Path.GetDirectoryName(processModule?.FileName);
        }
    }
}