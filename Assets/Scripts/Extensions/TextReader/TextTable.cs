using System;
using System.Collections.Generic;
using UnityEngine;

namespace Net.Core.TextReader
{
    public enum LoadType
    {
        FromString,
        FromResourcePath,
        FromPersistentDataPath,
    }

    public abstract class TextTable<T> where T : TextRecord
    {
        public T[] Rows { get; private set; }

        public readonly LoadType LoadType;
        public readonly string PathOrContent;
        private readonly string Content;
        public readonly int ColumnsCount;

        private readonly Dictionary<string, T> hash = new Dictionary<string, T>();
        private bool containsHeader;
        private int primaryKeyIndex;
        private Type genericType;
        private bool isLoaded = false;

        /// <summary>
        /// Create a new text table.
        /// </summary>
        public TextTable(LoadType type, string pathOrContent, int columns, int primaryKeyIndex, bool containsHeader)
        {
            this.LoadType = type;
            this.PathOrContent = pathOrContent;
            this.ColumnsCount = columns;
            this.primaryKeyIndex = primaryKeyIndex;
            this.containsHeader = containsHeader;
            this.genericType = typeof(T);
        }

        /// <summary>
        /// Load the text from disk.
        /// </summary>
        public void Load(bool force = false)
        {
            //check if need to load.
            if (!force && this.isLoaded)
                return;

            //check if is loading.
            if (this.isLoaded)
                return;

            string content = null;
            switch (this.LoadType)
            {
                case LoadType.FromString:
                    {
                        content = this.PathOrContent;
                        break;
                    }

                case LoadType.FromResourcePath:
                    {
                        var textAsset = Resources.Load<TextAsset>(this.PathOrContent);
                        if (textAsset != null) content = textAsset.text;
                        break;
                    }

                case LoadType.FromPersistentDataPath:
                    throw new System.Exception("Not implement load from FromPersistentDataPath.");
            }

            //now load.
            if (!string.IsNullOrEmpty(content))
            {
                string[] lines = content.Split(new string[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

                //set start line.
                int i = 0;
                if (this.containsHeader)
                    i++;

                var rows = new List<T>();
                for (; i < lines.Length; i++)
                {
                    var columns = lines[i].Split('\t');
                    if (columns.Length != this.ColumnsCount)
                        throw new System.Exception($"Invalid row {i} on text config {this.PathOrContent}");

                    var record = (T)Activator.CreateInstance(this.genericType);
                    record.SetData(columns);
                    rows.Add(record);

                    if (!hash.ContainsKey(columns[this.primaryKeyIndex]))
                    {
                        this.hash.Add(columns[this.primaryKeyIndex], record);
                    }
                    
                }

                this.Rows = rows.ToArray();
            }
            else
            {
                Debug.LogError("No txt file on " + this.PathOrContent);
            }
        }

        /// <summary>
        /// Gets a record by its key.
        /// </summary>
        public bool GetRecordByKey(object primaryKey, out T result)
        {
            return this.hash.TryGetValue(primaryKey.ToString(), out result);
        }
    }
}