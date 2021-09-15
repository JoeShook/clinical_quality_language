using System.Collections.Generic;

namespace model.cql.hl7.org
{
    public class Relationship
    {
        public Relationship(ModelContext context, IEnumerable<string> relatedKeys)
        {
            this.context = context;
            foreach (string key in relatedKeys)
            {
                this.relatedKeys.Add(key);
            }
        }

        private ModelContext context;
        public ModelContext getContext()
        {
            return context;
        }

        private List<string> relatedKeys = new List<string>();
        public IEnumerable<string> getRelatedKeys()
        {
            return relatedKeys;
        }
    }
}
