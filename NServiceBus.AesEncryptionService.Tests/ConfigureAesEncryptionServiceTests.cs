﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using NServiceBus.Config;
using NUnit.Framework;

namespace NServiceBus.AesEncryptionServiceTests
{
    [TestFixture]
    public class ConfigureRijndaelEncryptionServiceTests
    {

        [Test]
        public void Should_not_throw_for_empty_keys()
        {
            ConfigureAesEncryptionService.VerifyKeys(new List<string>());
        }

        [Test]
        public void Can_read_from_xml()
        {
            var xml =
@"<?xml version='1.0' encoding='utf-8' standalone='yes'?>
<configuration>
    <configSections>
        <section 
            name='EncryptionServiceConfig' 
            type='NServiceBus.Config.EncryptionServiceConfig, NServiceBus.AesEncryptionService'/>
</configSections>
<EncryptionServiceConfig Key='key1' KeyIdentifier='A' KeyFormat='Base64'>
  <ExpiredKeys>
    <add Key='key2' KeyIdentifier='B' KeyFormat='Base64' />
    <add Key='key3' />
  </ExpiredKeys>
</EncryptionServiceConfig>
</configuration>";

            var section = ReadSectionFromText<EncryptionServiceConfig>(xml);
            var keys = section.ExpiredKeys.Cast<ExpiredKey>()
                .Select(x => x.Key)
                .ToList();
            Assert.AreEqual("key1", section.Key);
            Assert.AreEqual(2, keys.Count);
            Assert.Contains("key2", keys);
            Assert.AreEqual("A", section.KeyIdentifier);
            Assert.AreEqual(KeyFormat.Base64, section.KeyFormat);
            Assert.AreEqual("B", section.ExpiredKeys["key2"].KeyIdentifier);
            Assert.AreEqual(KeyFormat.Base64, section.ExpiredKeys["key2"].KeyFormat, "Expired key KeyFormat");
        }

        static T ReadSectionFromText<T>(string s) where T : ConfigurationSection
        {
            var xml = s.Replace("'", "\"");
            var tempPath = Path.GetTempFileName();
            try
            {
                File.WriteAllText(tempPath, xml);

                var fileMap = new ExeConfigurationFileMap
                {
                    ExeConfigFilename = tempPath
                };

                var configuration = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);
                return (T)configuration.GetSection(typeof(T).Name);
            }
            finally
            {
                if (File.Exists(tempPath))
                {
                    File.Delete(tempPath);
                }
            }
        }

        [Test]
        public void Should_throw_for_overlapping_keys()
        {
            var keys = new List<string>
            {
                "key1",
                "key2",
                "key1"
            };
            var exception = Assert.Throws<ArgumentException>(() => ConfigureAesEncryptionService.VerifyKeys(keys));
            StringAssert.StartsWith("Overlapping keys defined. Ensure that no keys overlap.", exception.Message);
            Assert.AreEqual("expiredKeys", exception.ParamName);
        }

        [Test]
        public void Should_throw_for_whitespace_key()
        {
            var keys = new List<string>
            {
                "key1",
                "",
                "key2"
            };
            var exception = Assert.Throws<ArgumentException>(() => ConfigureAesEncryptionService.VerifyKeys(keys));
            StringAssert.StartsWith("Empty encryption key detected in position 1.", exception.Message);
            Assert.AreEqual("expiredKeys", exception.ParamName);
        }

        [Test]
        public void Should_throw_for_null_key()
        {
            var keys = new List<string>
            {
                "key1",
                null,
                "key2"
            };
            var exception = Assert.Throws<ArgumentException>(() => ConfigureAesEncryptionService.VerifyKeys(keys));
            StringAssert.StartsWith("Empty encryption key detected in position 1.", exception.Message);
            Assert.AreEqual("expiredKeys", exception.ParamName);
        }

        [Test]
        public void Should_throw_for_no_key_in_config()
        {
            var config = new EncryptionServiceConfig();
            var exception = Assert.Throws<Exception>(() => ConfigureAesEncryptionService.ConvertConfigToAesService(config));
            Assert.AreEqual("The EncryptionServiceConfig has an empty 'Key' property.", exception.Message);
        }

        [Test]
        public void Should_throw_for_whitespace_key_in_config()
        {
            var config = new EncryptionServiceConfig
            {
                Key = " "
            };
            var exception = Assert.Throws<Exception>(() => ConfigureAesEncryptionService.ConvertConfigToAesService(config));
            Assert.AreEqual("The EncryptionServiceConfig has an empty 'Key' property.", exception.Message);
        }

        [Test]
        public void Should_correctly_parse_key_identifiers_containing_multiple_keys()
        {
            var section = new EncryptionServiceConfig
            {
                KeyIdentifier = "1",
                ExpiredKeys =
                {
                    new ExpiredKey
                    {
                        KeyIdentifier = "2",
                        Key = "Key"
                    }
                }
            };

            var keys = ConfigureAesEncryptionService.ExtractKeysFromConfigSection(section);

            ICollection<string> expected = new[]
            {
                "1",
                "2"
            };

            Assert.AreEqual(expected, keys.Keys);
        }

        [Test]
        public void Should_correctly_convert_base64_key()
        {
            byte[] key = { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F };

            var base64 = Convert.ToBase64String(key);

            var section = new EncryptionServiceConfig()
            {
                Key = base64,
                KeyFormat = KeyFormat.Base64,
                KeyIdentifier = "1",
                ExpiredKeys =
                {
                    new ExpiredKey
                    {
                        KeyIdentifier = "2",
                        Key = base64,
                        KeyFormat = KeyFormat.Base64
                    }
                }
            };

            var keys = ConfigureAesEncryptionService.ExtractKeysFromConfigSection(section);

            Assert.AreEqual(key, keys["1"], "Key in configuration root incorrectly converted");
            Assert.AreEqual(key, keys["2"], "Key in expired keys incorrectly converted");
        }

        [Test]
        public void Should_correctly_convert_ascii_key()
        {
            var asciiKey = "0123456789123456";

            var key = Encoding.ASCII.GetBytes("0123456789123456");

            var section = new EncryptionServiceConfig
            {
                Key = asciiKey,
                KeyFormat = KeyFormat.Ascii,
                KeyIdentifier = "1",
                ExpiredKeys =
                {
                    new ExpiredKey
                    {
                        KeyIdentifier = "2",
                        Key = asciiKey,
                        KeyFormat = KeyFormat.Ascii
                    }
                }
            };

            var keys = ConfigureAesEncryptionService.ExtractKeysFromConfigSection(section);

            Assert.AreEqual(key, keys["1"], "Key in configuration root incorrectly converted");
            Assert.AreEqual(key, keys["2"], "Key in expired keys incorrectly converted");
        }

        [Test]
        public void Should_correctly_convert_ascii_key_when_no_value()
        {
            const string asciiKey = "0123456789123456";

            var key = Encoding.ASCII.GetBytes("0123456789123456");

            var section = new EncryptionServiceConfig
            {
                Key = asciiKey,
                KeyIdentifier = "1",
                ExpiredKeys =
                {
                    new ExpiredKey
                    {
                        KeyIdentifier = "2",
                        Key = asciiKey
                    }
                }
            };

            var keys = ConfigureAesEncryptionService.ExtractKeysFromConfigSection(section);

            Assert.AreEqual(key, keys["1"], "Key in configuration root incorrectly converted");
            Assert.AreEqual(key, keys["2"], "Key in expired keys incorrectly converted");
        }

        [Test]
        public void Should_have_correct_number_of_extracted_keys_without_key_identifier()
        {
            var section = new EncryptionServiceConfig
            {
                Key = "a",
                ExpiredKeys =
                {
                    new ExpiredKey
                    {
                        Key = "b"
                    }
                }
            };

            var result = ConfigureAesEncryptionService.ExtractDecryptionKeysFromConfigSection(section);

            Assert.AreEqual(2, result.Count, "Key count");
        }

        [Test]
        public void Should_have_correct_number_of_extracted_keys_with_empty_key_identifier()
        {
            var section = new EncryptionServiceConfig
            {
                Key = "a",
                KeyIdentifier = "a;",
                ExpiredKeys =
                {
                    new ExpiredKey
                    {
                        Key = "b",
                        KeyIdentifier = ";b"
                    }
                }
            };

            var result = ConfigureAesEncryptionService.ExtractDecryptionKeysFromConfigSection(section);

            Assert.AreEqual(2, result.Count, "Key count");
        }

        [Test]
        public void Should_trow_for_duplicate_expiredkeys_key_values()
        {
            Assert.Throws<ConfigurationErrorsException>(() =>
            {
                // ReSharper disable once ObjectCreationAsStatement
                new EncryptionServiceConfig
                {
                    ExpiredKeys =
                    {
                        new ExpiredKey
                        {
                            Key = "b",
                            KeyIdentifier = "1"
                        },
                        new ExpiredKey
                        {
                            Key = "b",
                            KeyIdentifier = "2"
                        }
                    }
                };
            });
        }

        [Test]
        public void Should_have_correct_number_of_extracted_keys_with_key_identifier()
        {
            var section = new EncryptionServiceConfig
            {
                Key = "a",
                KeyIdentifier = "a",
                ExpiredKeys =
                {
                    new ExpiredKey
                    {
                        Key = "b",
                        KeyIdentifier = "b"
                    }
                }
            };

            var result = ConfigureAesEncryptionService.ExtractDecryptionKeysFromConfigSection(section);

            Assert.AreEqual(2, result.Count, "Key count");
        }
    }
}
