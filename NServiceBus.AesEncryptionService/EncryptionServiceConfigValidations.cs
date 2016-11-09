namespace NServiceBus
{
    using System.Linq;
    using Config;

    static class EncryptionServiceConfigValidations
    {
        public static bool ConfigurationHasDuplicateKeyIdentifiers(EncryptionServiceConfig section)
        {
            // Combine all key identifier values, filter the empty ones, split them
            return section
                .ExpiredKeys
                .Cast<ExpiredKey>()
                .Select(x => x.KeyIdentifier)
                .Union(new[]
                {
                    section.KeyIdentifier
                })
                .Where(x => !string.IsNullOrEmpty(x))
                .Select(x => x.Split(';'))
                .SelectMany(x => x)
                .GroupBy(x => x)
                .Any(x => x.Count() > 1);
        }

        public static bool ExpiredKeysHaveDuplicateKeys(EncryptionServiceConfig section)
        {
            var items = section
                .ExpiredKeys
                .Cast<ExpiredKey>()
                .ToList();

            return items.Count != items.Select(x => x.Key).Distinct().Count();
        }

        public static bool EncryptionKeyListedInExpiredKeys(EncryptionServiceConfig section)
        {
            return section
                .ExpiredKeys
                .Cast<ExpiredKey>()
                .Any(x => x.Key == section.Key);
        }

        public static bool OneOrMoreExpiredKeysHaveNoKeyIdentifier(EncryptionServiceConfig section)
        {
            return section
                .ExpiredKeys
                .Cast<ExpiredKey>()
                .Any(x => string.IsNullOrEmpty(x.KeyIdentifier));
        }

        public static bool ExpiredKeysHaveWhiteSpace(EncryptionServiceConfig section)
        {
            return section
                .ExpiredKeys
                .Cast<ExpiredKey>()
                .Any(x => string.IsNullOrWhiteSpace(x.Key));
        }
    }
}