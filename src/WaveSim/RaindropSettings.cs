using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace WaveSim
{
    /// <summary>
    /// Persist raindrop screen saver settings in memory and provide support
    /// for loading from file and persisting to file.
    /// </summary>
    public class RaindropSettings
    {
        public const string SettingsFile = "Raindrops.xml";

        public double RaindropPeriodInMS { get; set; }   
        public double SplashAmplitude { get; set; } 
        public int DropSize { get; set; }
        public double Damping { get; set; }

        /// <summary>
        /// Instantiate the class, loading settings from a specified file.
        /// If the file doesn't exist, use default values.
        /// </summary>
        /// <param name="sSettingsFilename"></param>
        public RaindropSettings() 
        {
            SetDefaults();      // Clean object, start w/defaults
        }

        /// <summary>
        /// Set all values to their defaults
        /// </summary>
        public void SetDefaults()
        {
            RaindropPeriodInMS = 35.0;
            SplashAmplitude = -3.0;
            DropSize = 1;
            Damping = 0.96;
        }

        /// <summary>
        /// Save current settings to external file
        /// </summary>
        /// <param name="sSettingsFilename"></param>
        public void Save(string sSettingsFilename)
        {
            try
            {
                XmlSerializer serial = new XmlSerializer(typeof(RaindropSettings));

                FileStream fs = new FileStream(sSettingsFilename, FileMode.Create);
                TextWriter writer = new StreamWriter(fs, new UTF8Encoding());
                serial.Serialize(writer, this);
                writer.Close();
            }
            catch { }
        }

        /// <summary>
        /// Attempt to load settings from external file.  If the file doesn't 
        /// exist, or if there is a problem, no settings are changed.
        /// </summary>
        /// <param name="sSettingsFilename"></param>
        public static RaindropSettings Load(string sSettingsFilename)
        {
            RaindropSettings settings = null;

            try
            {
                XmlSerializer serial = new XmlSerializer(typeof(RaindropSettings));
                FileStream fs = new FileStream(sSettingsFilename, FileMode.OpenOrCreate);
                TextReader reader = new StreamReader(fs);
                settings = (RaindropSettings)serial.Deserialize(reader);
            }
            catch {
                // If we can't load, just create a new object, which gets default values
                settings = new RaindropSettings();      
            }

            return settings;
        }
    }
}
