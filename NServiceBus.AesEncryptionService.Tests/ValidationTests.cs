using System.Configuration;
using NServiceBus.Config;
using NUnit.Framework;

namespace NServiceBus.AesEncryptionServiceTests
{
    [TestFixture]
    public class ValidationTests
    {
        [Test]
        public void Should_be_false_when_encryption_key_not_in_expired_keys()
        {
            var section = new EncryptionServiceConfig()
            {
                Key = "Key",
                ExpiredKeys = new ExpiredKeyCollection
                {
                    new ExpiredKey{Key="AnotherKey"},
                }
            };

            Assert.IsFalse(EncryptionServiceConfigValidations.EncryptionKeyListedInExpiredKeys(section));
        }

        [Test]
        public void Should_be_true_when_encryption_key_in_expirede_keys()
        {
            var section = new EncryptionServiceConfig
            {
                Key = "Key",
                ExpiredKeys = new ExpiredKeyCollection
                {
                    new ExpiredKey{Key="Key"},
                }
            };

            Assert.IsTrue(EncryptionServiceConfigValidations.EncryptionKeyListedInExpiredKeys(section));
        }


        [Test]
        public void Should_be_false_when_no_duplicate_key_identifiers()
        {
            var section = new EncryptionServiceConfig
            {
                KeyIdentifier = "2",
                ExpiredKeys =
                {
                    new ExpiredKey{ Key="A", KeyIdentifier = "1" },
                    new ExpiredKey{ Key="B", KeyIdentifier = "3;4" }
                }
            };

            Assert.IsFalse(EncryptionServiceConfigValidations.ConfigurationHasDuplicateKeyIdentifiers(section));
        }

        [Test]
        public void Should_be_true_when_duplicate_key_identifier_in_concat_root_and_expired_keys()
        {
            var section = new EncryptionServiceConfig
            {
                KeyIdentifier = "4;2",
                ExpiredKeys =
                {
                    new ExpiredKey{ Key="A", KeyIdentifier = "1" },
                    new ExpiredKey{ Key="B", KeyIdentifier = "3;2" }
                }
            };

            Assert.IsTrue(EncryptionServiceConfigValidations.ConfigurationHasDuplicateKeyIdentifiers(section));
        }
        [Test]
        public void Should_be_true_when_duplicate_key_identifier_in_concat_expired_keys()
        {
            var section = new EncryptionServiceConfig
            {
                KeyIdentifier = "2",
                ExpiredKeys =
                {
                    new ExpiredKey{ Key="A", KeyIdentifier = "1;4" },
                    new ExpiredKey{ Key="B", KeyIdentifier = "3;4" }
                }
            };

            Assert.IsTrue(EncryptionServiceConfigValidations.ConfigurationHasDuplicateKeyIdentifiers(section));
        }

        [Test]
        public void Should_throw_when_expired_keys_has_duplicate_keys()
        {
            var section = new EncryptionServiceConfig
            {
                ExpiredKeys =
                {
                    new ExpiredKey{ Key = "Key" },
                }
            };

            Assert.Throws<ConfigurationErrorsException>(() =>
            {
                section.ExpiredKeys.Add(new ExpiredKey
                {
                    Key = "Key",
                    KeyIdentifier = "ID"
                });
            }, "The entry 'Key' has already been added.");
        }


        [Test]
        public void Should_be_false_when_key_has_no_whitespace()
        {
            var section = new EncryptionServiceConfig
            {
                ExpiredKeys =
                {
                    new ExpiredKey{ Key = "Key" }
                }
            };

            Assert.IsFalse(EncryptionServiceConfigValidations.ExpiredKeysHaveWhiteSpace(section));
        }
        [Test]
        public void Should_be_true_when_key_has_whitespace()
        {
            var section = new EncryptionServiceConfig
            {
                ExpiredKeys =
                {
                    new ExpiredKey{ Key = " " }
                }
            };

            Assert.IsTrue(EncryptionServiceConfigValidations.ExpiredKeysHaveWhiteSpace(section));
        }

        [Test]
        public void Should_be_false_when_key_identifier_in_expired_keys()
        {
            var section = new EncryptionServiceConfig
            {
                ExpiredKeys =
                {
                    new ExpiredKey{ KeyIdentifier = "ID", Key = "Key" }
                }
            };

            Assert.IsFalse(EncryptionServiceConfigValidations.OneOrMoreExpiredKeysHaveNoKeyIdentifier(section));
        }

        [Test]
        public void Should_be_true_when_key_identifier_not_in_expired_keys()
        {
            var section = new EncryptionServiceConfig
            {
                ExpiredKeys =
                {
                    new ExpiredKey { Key = "Key" }
                }
            };

            Assert.IsTrue(EncryptionServiceConfigValidations.OneOrMoreExpiredKeysHaveNoKeyIdentifier(section));
        }

    }
}