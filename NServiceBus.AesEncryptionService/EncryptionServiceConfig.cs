namespace NServiceBus.Config
{
    using System.Configuration;

    /// <summary>
    /// Used to configure keys for encryption services.
    /// </summary>
    public class EncryptionServiceConfig : ConfigurationSection
    {
        /// <summary>
        /// The encryption key.
        /// </summary>
        [ConfigurationProperty("Key", IsRequired = true)]
        public string Key
        {
            get { return this["Key"] as string; }
            set { this["Key"] = value; }
        }

        /// <summary>
        /// Identifies this key for it to be used for decryption.
        /// </summary>
        [ConfigurationProperty("KeyIdentifier", IsRequired = false)]
        public string KeyIdentifier
        {
            get { return (string) this["KeyIdentifier"]; }
            set { this["KeyIdentifier"] = value; }
        }

        /// <summary>
        /// Contains the encryption keys to use.
        /// </summary>
        [ConfigurationProperty("ExpiredKeys", IsRequired = false)]
        public ExpiredKeyCollection ExpiredKeys
        {
            get { return this["ExpiredKeys"] as ExpiredKeyCollection; }
            set { this["ExpiredKeys"] = value; }
        }


        /// <summary>
        /// The data format in which the key is stored.
        /// </summary>
        [ConfigurationProperty("KeyFormat", IsRequired = false)]
        public KeyFormat KeyFormat
        {
            get { return (KeyFormat) this["KeyFormat"]; }
            set { this["KeyFormat"] = value; }
        }
    }
}