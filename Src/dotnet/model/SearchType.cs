using System;


namespace model.cql.hl7.org
{

    public class SearchType
    {
        private readonly string name;
        
        public SearchType(string name, string path, DataType type)
        {
            if (name == null || string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("A name is required to construct a Search");
            }
            if (path == null || string.IsNullOrEmpty(path))
            {
                throw new ArgumentException("A path is required to construct a Search");
            }

            this.name = name;
            this.path = path;
            this.type = type;
        }
        
        public string getName()
        {
            return name;
        }

        private string path;
        public string getPath()
        {
            return path;
        }

        private DataType type;
        public DataType getType()
        {
            return type;
        }
    }

}
