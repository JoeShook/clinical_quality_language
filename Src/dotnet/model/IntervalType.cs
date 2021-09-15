using System;

namespace model.cql.hl7.org
{
    public class IntervalType : DataType
    {
        private readonly DataType pointType;

        public IntervalType(DataType pointType)
        {
            this.pointType = pointType ?? throw new ArgumentException("pointType");
        }

        public DataType GetPointType()
        {
            return this.pointType;
        }

        /// <summary>Serves as the default hash function.</summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            return 53 * pointType.GetHashCode();
        }

        /// <summary>Determines whether the specified object is equal to the current object.</summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>true if the specified object  is equal to the current object; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            if (obj is IntervalType that)
            {
                return this.pointType.Equals(that.pointType);
            }

            return false;
        }



        public override bool IsSubTypeOf(DataType other)
        {
            if (other is IntervalType)
            {
                IntervalType that = (IntervalType)other;
                return this.pointType.IsSubTypeOf(that.pointType);
            }

            return base.IsSubTypeOf(other);
        }

        public override bool IsSuperTypeOf(DataType other)
        {
            if (other is IntervalType)
            {
                IntervalType that = (IntervalType)other;
                return this.pointType.IsSuperTypeOf(that.pointType);
            }

            return base.IsSuperTypeOf(other);
        }


        public override string ToString()
        {
            return $"interval<{pointType}>";
        }

        public override string ToLabel()
        {
            return $"Interval of {pointType.ToLabel()}";
        }


        public override bool IsGeneric()
        {
            return pointType.IsGeneric();
        }



        public override bool IsInstantiable(DataType callType, IInstantiationContext context)
        {
            if (callType is IntervalType)
            {
                IntervalType intervalType = (IntervalType)callType;
                return pointType.IsInstantiable(intervalType.pointType, context);
            }

            bool isAlreadyInstantiable = false;

            foreach (IntervalType targetIntervalType in context.GetIntervalConversionTargets(callType))
            {
                bool isInstantiable = pointType.IsInstantiable(targetIntervalType.pointType, context);

                if (isInstantiable)
                {
                    if (isAlreadyInstantiable)
                    {
                        throw new ArgumentException(string.Format("Ambiguous generic instantiation involving {0} to {1}.",
                                callType, targetIntervalType));
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
            return new IntervalType(pointType.Instantiate(context));
        }
    }

}
