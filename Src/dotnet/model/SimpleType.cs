using System;

namespace model.cql.hl7.org
{
    public class SimpleType : DataType
    {
        private string name;
        private string target;

        public SimpleType(string name, DataType baseType) : base(baseType)
        {
            if (name == null || name.Equals(""))
            {
                throw new ArgumentException("name");
            }
            this.name = name;
        }

        public SimpleType(string name) : this(name, null)
        {
        }

        /// <summary>Serves as the default hash function.</summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            return name.GetHashCode();
        }

        /// <summary>Determines whether the specified object is equal to the current object.</summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>true if the specified object  is equal to the current object; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            if (obj is SimpleType)
            {
                var that = (SimpleType) obj;

                return this.name.Equals(that.name);
            }

            return false;
        }

        public override bool IsGeneric()
        {
            throw new NotImplementedException();
        }

        public override bool IsInstantiable(DataType callType, IInstantiationContext context)
        {
            if (IsSuperTypeOf(callType))
            {
                return true;
            }

            bool isAlreadyInstantiable = false;
            foreach (SimpleType targetSimpleType in context.GetSimpleConversionTargets(callType))
            {
                bool isInstantiable = true; // If it came back from this call, we can instantiate it...
                if (isInstantiable)
                {
                    if (isAlreadyInstantiable)
                    {
                        throw new ArgumentException(String.Format("Ambiguous generic instantiation involving {0} to {1}.",
                            callType, targetSimpleType));
                    }
                    isAlreadyInstantiable = true;
                }
            }

            if (isAlreadyInstantiable)
            {
                return true;
            }

            return false;
        }

        public override DataType Instantiate(IInstantiationContext context)
        {
            return this;
        }
    }
}
