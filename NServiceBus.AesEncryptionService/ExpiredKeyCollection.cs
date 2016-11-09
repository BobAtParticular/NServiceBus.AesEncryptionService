namespace NServiceBus.Config
{
    using System.Configuration;

    /// <summary>
    /// A configuration element collection of <see cref="ExpiredKey" />s.
    /// </summary>
    public class ExpiredKeyCollection : ConfigurationElementCollection
    {
        /// <summary>
        /// Returns AddRemoveClearMap.
        /// </summary>
        public override ConfigurationElementCollectionType CollectionType => ConfigurationElementCollectionType.AddRemoveClearMap;

        /// <summary>
        /// Gets/sets the <see cref="ExpiredKey" /> at the given index.
        /// </summary>
        public ExpiredKey this[int index]
        {
            get { return (ExpiredKey) BaseGet(index); }
            set
            {
                if (BaseGet(index) != null)
                {
                    BaseRemoveAt(index);
                }
                BaseAdd(index, value);
            }
        }

        /// <summary>
        /// Gets the <see cref="ExpiredKey" /> for the given key.
        /// </summary>
        new public ExpiredKey this[string key] => (ExpiredKey) BaseGet(key);

        /// <summary>
        /// Creates a new <see cref="ExpiredKey" />.
        /// </summary>
        protected override ConfigurationElement CreateNewElement()
        {
            return new ExpiredKey();
        }

        /// <summary>
        /// Creates a new <see cref="ExpiredKey" />, setting its <see cref="ExpiredKey.Key" /> property to the
        /// given value.
        /// </summary>
        protected override ConfigurationElement CreateNewElement(string elementName)
        {
            return new ExpiredKey
            {
                Key = elementName
            };
        }

        /// <summary>
        /// Returns the Messages property of the given <see cref="ExpiredKey" /> element.
        /// </summary>
        protected override object GetElementKey(ConfigurationElement element)
        {
            var encryptionKey = (ExpiredKey) element;

            return encryptionKey.Key;
        }

        /// <summary>
        /// Calls BaseIndexOf on the given <see cref="ExpiredKey" />.
        /// </summary>
        public int IndexOf(ExpiredKey encryptionKey)
        {
            return BaseIndexOf(encryptionKey);
        }

        /// <summary>
        /// Calls BaseAdd.
        /// </summary>
        public void Add(ExpiredKey mapping)
        {
            BaseAdd(mapping);
        }

        /// <summary>
        /// Calls BaseAdd with true as the additional parameter.
        /// </summary>
        protected override void BaseAdd(ConfigurationElement element)
        {
            BaseAdd(element, true);
        }

        /// <summary>
        /// If the key exists, calls BaseRemove on it.
        /// </summary>
        public void Remove(ExpiredKey mapping)
        {
            if (BaseIndexOf(mapping) >= 0)
            {
                BaseRemove(mapping.Key);
            }
        }

        /// <summary>
        /// Calls BaseRemoveAt.
        /// </summary>
        public void RemoveAt(int index)
        {
            BaseRemoveAt(index);
        }

        /// <summary>
        /// Calls BaseRemove.
        /// </summary>
        public void Remove(string name)
        {
            BaseRemove(name);
        }

        /// <summary>
        /// Calls BaseClear.
        /// </summary>
        public void Clear()
        {
            BaseClear();
        }

        /// <summary>
        /// True if the collection is readonly.
        /// </summary>
        public override bool IsReadOnly()
        {
            return false;
        }
    }
}