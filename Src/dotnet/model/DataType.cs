namespace model.cql.hl7.org
{
    public abstract class DataType 
    {
        private DataType baseType;

        public DataType() : this(null)
        {
        }

        public DataType(DataType baseType)
        {
            this.baseType = baseType == null ? DataType.ANY : baseType;
        }

        public DataType BaseType => baseType;

        public virtual string ToLabel()
        {
            return ToString();
        }

        public virtual bool IsSubTypeOf(DataType other)
        {
            DataType currentType = this;

            while (currentType != null)
            {
                if (currentType.Equals(other))
                {
                    return true;
                }

                currentType = currentType.baseType;
            }

            return false;
        }

        public virtual bool IsSuperTypeOf(DataType other)
        {
            while (other != null)
            {
                if (this.Equals(other))
                {
                    return true;
                }

                other = other.baseType;
            }

            return false;
        }


        // Note that this is not how implicit/explicit conversions are defined, the notion of
        // type compatibility is used to support implicit casting, such as casting a "null"
        // literal to any other type, or casting a class to an equivalent tuple.
        public virtual bool IsCompatibleWith(DataType other)
        {
            // A type is compatible with a choice type if it is a subtype of one of the choice types
            if (other is ChoiceType) {
                foreach (DataType choice in ((ChoiceType)other).Types)
                {
                    if (this.IsSubTypeOf(choice))
                    {
                        return true;
                    }
                }
            }

            return this.Equals(other); // Any data type is compatible with itself
        }

        public abstract bool IsGeneric();

        public abstract bool IsInstantiable(DataType callType, IInstantiationContext context);

        public abstract DataType Instantiate(IInstantiationContext context);

        public static readonly SimpleType ANY = new SimpleType("System.Any");
    }
}