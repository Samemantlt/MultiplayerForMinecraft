using System.IO;
using System.Xml.Serialization;

namespace MultiplayerForMinecraft
{
    public class SettingsSerializer
    {
        private const string SettingsPath = @"settings.xml";
        private XmlSerializer _serializer = new XmlSerializer(typeof(Settings));


        public void Save(Settings settings)
        {
            var stream = File.CreateText(SettingsPath);
            try
            {
                _serializer.Serialize(stream, settings);
            }
            finally
            {
                stream.Flush();
                stream.Dispose();
            }
        }

        public Settings Load()
        {
            if (!File.Exists(SettingsPath))
                return new Settings();

            var stream = File.OpenText(SettingsPath);

            try
            {
                return (Settings)_serializer.Deserialize(stream);
            }
            catch
            {
                stream.Dispose();
                File.Delete(SettingsPath);
                return new Settings();
            }
            finally
            {
                stream.Dispose();
            }
        }
    }
}
