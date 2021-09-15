using System;


namespace model.cql.hl7.org
{

    public class ListType : DataType
    {
        private DataType elementType;

        public ListType(DataType elementType)
        {

            if (elementType == null)
            {
                throw new ArgumentException("elementType");
            }

            this.elementType = elementType;
        }

        public DataType getElementType()
        {
            return this.elementType;
        }

        /// <summary>Serves as the default hash function.</summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            return 67 * elementType.GetHashCode();

        }

        /// <summary>Determines whether the specified object is equal to the current object.</summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>true if the specified object  is equal to the current object; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            if (obj is ListType)
            {
                ListType that = (ListType)obj;
                return this.elementType.Equals(that.elementType);
            }

            return false;
        }


        public override bool IsSubTypeOf(DataType other)
        {
            if (other is ListType)
            {
                ListType that = (ListType)other;
                return this.elementType.IsSubTypeOf(that.elementType);
            }

            return base.IsSubTypeOf(other);
        }


        public override bool IsSuperTypeOf(DataType other)
        {
            if (other is ListType)
            {
                ListType that = (ListType)other;
                return this.elementType.IsSuperTypeOf(that.elementType);
            }

            return base.IsSuperTypeOf(other);
        }


        public override string ToString()
        {
            return $"list<{elementType.ToString()}>";
        }


        public override string ToLabel()
        {
            return $"List of {elementType.ToLabel()}";
        }


        public override bool IsGeneric()
        {
            return elementType.IsGeneric();
        }

    public override bool IsInstantiable(DataType callType, IInstantiationContext context)
        {
            if (callType is ListType) {
                ListType listType = (ListType)callType;
                return elementType.IsInstantiable(listType.elementType, context);
            }

            bool isAlreadyInstantiable = false;
            foreach (ListType targetListType in context.GetListConversionTargets(callType))
            {
                bool isInstantiable = elementType.IsInstantiable(targetListType.elementType, context);
                if (isInstantiable)
                {
                    if (isAlreadyInstantiable)
                    {
                        throw new ArgumentException(String.Format("Ambiguous generic instantiation involving {0} to {1}.",
                                callType, targetListType));
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
            return new ListType(elementType.Instantiate(context));
        }
    }

}
