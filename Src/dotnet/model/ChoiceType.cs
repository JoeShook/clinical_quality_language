using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace model.cql.hl7.org
{
    public class ChoiceType : DataType
    {
        private List<DataType> _types = new List<DataType>();

        public ChoiceType(IEnumerable<DataType> types)
        {
            // Expand choice types in the constructor, it never makes sense to have a choice of choices
            foreach (DataType type in types)
            {
                AddType(type);
            }
        }

        public IList<DataType> Types => _types.AsReadOnly();

        private void AddType(DataType type)
        {
            if (type is ChoiceType) {

                var choiceType = (ChoiceType)type;

                foreach (DataType choice in choiceType.Types)
                {
                    AddType(choice);
                }
            }
            else
            {
                _types.Add(type);
            }
        }

        public override int GetHashCode()
        {
            int result = 13;
            
            for (int i = 0; i < Types.Count; i++)
            {
                result += (37 * Types[i].GetHashCode());
            }

            return result;
        }

        public override bool Equals(Object o)
        {
            if (o is ChoiceType) {
                ChoiceType that = (ChoiceType)o;

                if (this.Types.Count == that.Types.Count)
                {
                    var theseTypes = this.Types;
                    var thoseTypes = that.Types;
                    for (int i = 0; i < theseTypes.Count; i++)
                    {
                        if (!theseTypes[i].Equals(thoseTypes[i]))
                        {
                            return false;
                        }
                    }

                    return true;
                }
            }

            return false;
        }

        public bool IsSubSetOf(ChoiceType other)
        {
            foreach (DataType type in _types)
            {
                bool currentIsSubType = false;
                foreach (DataType otherType in other.Types)
                {
                    currentIsSubType = type.IsSubTypeOf(otherType);
                    if (currentIsSubType)
                    {
                        break;
                    }
                }

                if (!currentIsSubType)
                {
                    return false;
                }
            }

            return true;
        }

        public bool IsSuperSetOf(ChoiceType other)
        {
            return other.IsSubSetOf(this);
        }

        public override bool IsCompatibleWith(DataType other)
        {
            // This type is compatible with the other type if
            // The other type is a subtype of one of the choice types
            // The other type is a choice type and all the components of this choice are a subtype of some component of the other type
            if (other is ChoiceType)
            {
                return this.IsSubSetOf((ChoiceType)other) || this.IsSuperSetOf((ChoiceType)other);
            }

            foreach (DataType type in Types)
            {
                if (other.IsCompatibleWith(type))
                {
                    return true;
                }
            }

            return base.IsCompatibleWith(other);
        }
        
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("choice<");
            var first = true;

            foreach (DataType type in _types)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    sb.Append(",");
                }
                sb.Append(type);
            }

            sb.Append(">");
            return sb.ToString();
        }

        public override bool IsGeneric()
        {
            // TODO: It hardly makes sense for a choice type to have generics.... ignoring in instantiation semantics for now
            foreach (DataType type in Types)
            {
                if (type.IsGeneric())
                {
                    return true;
                }
            }

            return false;
        }

        public override bool IsInstantiable(DataType callType, IInstantiationContext context)
        {
            return IsSuperTypeOf(callType);
        }

        public override DataType Instantiate(IInstantiationContext context)
        {
            return this;
        }
    }
}