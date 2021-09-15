using System;

namespace model.cql.hl7.org
{
    public class TupleTypeElement
    {
        private String name;
        private DataType type;
        private bool oneBased;

        public TupleTypeElement(String name, DataType type, bool oneBased)
        {
            if (name == null || name.Equals(""))
            {
                throw new ArgumentException("name");
            }

            if (type == null)
            {
                throw new ArgumentException("type");
            }

            this.name = name;
            this.type = type;
            this.oneBased = oneBased;
        }

        public TupleTypeElement(String name, DataType type) : this(name, type, false)
        {
        }

        public String getName()
        {
            return this.name;
        }

        public DataType getType()
        {
            return this.type;
        }
        
        /// <summary>Serves as the default hash function.</summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            return (17 * this.name.GetHashCode())
                   + (33 * this.type.GetHashCode())
                   + (31 * (this.oneBased ? 1 : 0));
        }

        /// <summary>Determines whether the specified object is equal to the current object.</summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>true if the specified object  is equal to the current object; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            if (obj is TupleTypeElement) {
                TupleTypeElement that = (TupleTypeElement)obj;
                return this.name.Equals(that.name)
                       && this.type.Equals(that.type)
                       && (this.oneBased == that.oneBased);
            }

            return false;
        }

        public bool IsSubTypeOf(TupleTypeElement that)
        {
            return this.getName().Equals(that.getName()) && this.getType().IsSubTypeOf(that.getType());
        }

        public bool IsSuperTypeOf(TupleTypeElement that)
        {
            return this.getName().Equals(that.getName()) && this.getType().IsSuperTypeOf(that.getType());
        }
        
        public override string ToString()
        {
            return $"{this.name}:{this.type.ToString()}";
        }

        public string ToLabel()
        {
            return $"{this.name}: {this.type.ToLabel()}";
        }
    }
}
