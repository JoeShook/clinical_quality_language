using System.Collections.Generic;

namespace model.cql.hl7.org
{
    public class ModelContext
    {
        public ModelContext(string name, ClassType type, IEnumerable<string> keys, string birthDateElement)
        {
            this.name = name;
            this.type = type;
            this.birthDateElement = birthDateElement;
            if (keys != null)
            {
                foreach (string key in keys)
                {
                    this.keys.Add(key);
                }
            }
        }

        private string name;
        public string getName()
        {
            return name;
        }

        private ClassType type;
        public ClassType getType()
        {
            return type;
        }

        private string birthDateElement;
        public string getBirthDateElement()
        {
            return birthDateElement;
        }

        private List<string> keys = new List<string>();
        public IEnumerable<string> getKeys()
        {
            return keys;
        }
    }
}