namespace Net.Core.TextReader
{
    public abstract class TextRecord
    {
        /// <summary>
        /// The columns values.
        /// </summary>

        public string[] Columns { get; private set; }

        /// <summary>
        /// Gets string at column index.
        /// </summary>
        protected string GetString(int index)
        {
            return this.Columns[index];
        }

        /// <summary>
        /// Gets int at column index.
        /// </summary>
        protected int GetInt(int index)
        {
            var str = this.Columns[index];
            if (!string.IsNullOrEmpty(str) && int.TryParse(str, out var result))
            {
                return result;
            }

            return 0;
        }

        /// <summary>
        /// Gets float at column index.
        /// </summary>
        protected float GetFloat(int index)
        {
            var str = this.Columns[index];
            if (!string.IsNullOrEmpty(str) && float.TryParse(str, out var result))
            {
                return result;
            }

            return 0;
        }

        /// <summary>
        /// Gets bool at column index.
        /// </summary>
        protected bool GetBool(int index)
        {
            var str = this.Columns[index];
            if (!string.IsNullOrEmpty(str) && bool.TryParse(str, out var result))
            {
                return result;
            }

            return false;
        }

        /// <summary>
        /// Sets data and initialize record.
        /// </summary>
        public void SetData(string[] columns)
        {
            this.Columns = columns;
            this.Initialize();
        }

        /// <summary>
        /// Initialize text record.
        /// </summary>
        protected abstract void Initialize();
    }
}

