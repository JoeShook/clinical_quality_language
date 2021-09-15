using System;


namespace model.cql.hl7.org
{
    public class TypeParameter : DataType
    {
        private readonly string identifier;

        public enum TypeParameterConstraint
        {
            /**
             * Indicates the type parameter has no constraint and be bound to any type
             */
            NONE,

            /**
             * Indicates the type parameter can only be bound to class types
             */
            CLASS,

            /**
             * Indicates the type parameter can only be bound to value types (simple types)
             */
            VALUE,

            /**
             * Indicates the type parameter can only be bound to tuple types
             */
            TUPLE,

            /**
             * Indicates the type parameter can only be bound to interval types
             */
            INTERVAL,

            /**
             * Indicates the type parameter can only be bound to choice types
             */
            CHOICE,

            /**
             * Indicates the type parameter can only be bound to the constraint type or a type derived from the constraint type
             */
            TYPE
        }

        public TypeParameter(string identifier)
        {
            if (identifier == null || identifier.Equals(""))
            {
                throw new ArgumentException("identifier is null");
            }

            this.identifier = identifier;
        }

        public TypeParameter(string identifier, TypeParameterConstraint constraint, DataType constraintType) :
            this(identifier)
        {
            this.constraint = constraint;
            this.constraintType = constraintType;
        }

        public string Identifier => identifier;

        public TypeParameterConstraint constraint = TypeParameterConstraint.NONE;

        public TypeParameterConstraint getConstraint()
        {
            return constraint;
        }

        private DataType constraintType;

        public DataType GetConstraintType()
        {
            return constraintType;
        }

        /**
     * @param callType
     * @return True if the given callType can be bound to this parameter (i.e. it satisfied any constraints defined for the type parameter)
     */
        public bool CanBind(DataType callType)
        {
            switch (constraint)
            {
                case TypeParameterConstraint.CHOICE:
                    return callType is ChoiceType;
                case TypeParameterConstraint.TUPLE:
                    return callType is TupleType;
                case TypeParameterConstraint.INTERVAL:
                    return callType is IntervalType;
                case TypeParameterConstraint.CLASS:
                    return callType is ClassType;
                case TypeParameterConstraint.VALUE:
                    return callType is SimpleType && !callType.Equals(DataType.ANY);
                case TypeParameterConstraint.TYPE:
                    return callType.IsSubTypeOf(constraintType);
                case TypeParameterConstraint.NONE:
                default:
                    return true;
            }
        }


        /// <summary>Serves as the default hash function.</summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            return identifier.GetHashCode();
        }


        public override bool Equals(Object o)
        {
            if (o is TypeParameter)
            {
                TypeParameter that = (TypeParameter)o;
                return this.identifier.Equals(that.identifier);
            }

            return false;
        }


        public override string ToString()
        {
            return identifier;
        }

        public override bool IsGeneric()
        {
            return true;
        }

        public override bool IsInstantiable(DataType callType, IInstantiationContext context)
        {
            return context.IsInstantiable(this, callType);
        }

        public override DataType Instantiate(IInstantiationContext context)
        {
            return context.Instantiate(this);
        }
    }
}
