namespace ProductManagement.Framework.SearchAccessor
{
    public class IndexDesc<T> where T : new()
    {
        string _indexId;

        public string IndexId => _indexId;

        T _indexValue;

        public T IndexValue => _indexValue;

        public IndexDesc<T> Value(T value)
        {
            _indexValue = value;
            return this;
        }

        public IndexDesc<T> Id(string id)
        {
            _indexId = id;
            return this;
        }
    }
}
