using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace model.cql.hl7.org
{
    public class TupleType : DataType
    {
        private List<TupleTypeElement> elements = new List<TupleTypeElement>();
        private SortedList<string, TupleTypeElement> sortedElements = null;

        public TupleType(IList<TupleTypeElement> elements) : base()
        {
            if (elements != null)
            {
                this.elements.AddRange(elements);
            }
        }

        public TupleType() : this(null)
        {
        }

        public IEnumerable<TupleTypeElement> getElements()
        {
            return elements;
        }

        public void addElement(TupleTypeElement element)
        {
            this.elements.Add(element);
            sortedElements = null;
        }

        public void addElements(Collection<TupleTypeElement> elements)
        {
            this.elements.AddRange(elements);
            sortedElements = null;
        }

        private List<TupleTypeElement> getSortedElements()
        {
            if (sortedElements == null)
            {
                sortedElements = new SortedList<string, TupleTypeElement>();
                elements.ForEach(e => sortedElements.Add(e.getName(), e));
            }

            //TODO: Note to future Joe: find unit test or create to see if this actually maintains sort 
            // Probably should just return SortedList but I am just in Porting mode right now.
            return sortedElements.Select(e => e.Value).ToList();
        }

        /// <summary>Serves as the default hash function.</summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            int result = 13;
            for (int i = 0; i < elements.Count; i++)
            {
                result += (37 * elements[i].GetHashCode());
            }

            return result;

        }

        /// <summary>Determines whether the specified object is equal to the current object.</summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>true if the specified object  is equal to the current object; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            if (obj is TupleType)
            {
                TupleType that = (TupleType)obj;

                if (this.elements.Count == that.elements.Count)
                {
                    List<TupleTypeElement> theseElements = this.getSortedElements();
                    List<TupleTypeElement> thoseElements = that.getSortedElements();
                    for (int i = 0; i < theseElements.Count; i++)
                    {
                        if (!theseElements[i].Equals(thoseElements[i]))
                        {
                            return false;
                        }
                    }

                    return true;
                }
            }

            return false;
        }


        public override bool IsSubTypeOf(DataType other)
        {
            if (other is TupleType)
            {
                TupleType that = (TupleType)other;

                if (this.elements.Count == that.elements.Count)
                {
                    List<TupleTypeElement> theseElements = this.getSortedElements();
                    List<TupleTypeElement> thoseElements = that.getSortedElements();
                    for (int i = 0; i < theseElements.Count; i++)
                    {
                        if (!theseElements[i].IsSubTypeOf(thoseElements[i]))
                        {
                            return false;
                        }
                    }

                    return true;
                }
            }

            return base.IsSubTypeOf(other);
        }


        public override bool IsSuperTypeOf(DataType other)
        {
            if (other is TupleType) {
                TupleType that = (TupleType)other;

                if (this.elements.Count == that.elements.Count)
                {
                    List<TupleTypeElement> theseElements = this.getSortedElements();
                    List<TupleTypeElement> thoseElements = that.getSortedElements();
                    for (int i = 0; i < theseElements.Count; i++)
                    {
                        if (!theseElements[i].IsSuperTypeOf(thoseElements[i]))
                        {
                            return false;
                        }
                    }

                    return true;
                }
            }

            return base.IsSuperTypeOf(other);
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("tuple{");
            for (int i = 0; i < elements.Count; i++)
            {
                if (i > 0)
                {
                    builder.Append(",");
                }

                builder.Append(elements[i].ToString());
            }

            builder.Append("}");
            return builder.ToString();
        }

        public override string ToLabel()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("tuple of ");
            for (int i = 0; i < elements.Count; i++)
            {
                if (i > 0)
                {
                    builder.Append(", ");
                }

                builder.Append(elements[i].ToLabel());
            }

            return builder.ToString();
        }

        public override bool IsCompatibleWith(DataType other)
        {
            if (other is ClassType)
            {
                ClassType classType = (ClassType)other;
                return this.Equals(classType.getTupleType());
            }

            return base.IsCompatibleWith(other);
        }


        public override bool IsGeneric()
        {
            foreach (TupleTypeElement e in elements)
            {
                if (e.getType().IsGeneric())
                {
                    return true;
                }
            }

            return false;
        }

        public override bool IsInstantiable(DataType callType, IInstantiationContext context)
        {
            if (callType is TupleType)
            {
                TupleType tupleType = (TupleType)callType;
                if (elements.Count == tupleType.elements.Count)
                {
                    List<TupleTypeElement> theseElements = getSortedElements();
                    List<TupleTypeElement> thoseElements = tupleType.getSortedElements();
                    for (int i = 0; i < theseElements.Count; i++)
                    {
                        if (!(theseElements[i].getName().Equals(thoseElements[i].getName())
                              && theseElements[i].getType().IsInstantiable(thoseElements[i].getType(), context)))
                        {
                            return false;
                        }
                    }

                    return true;
                }
            }

            return false;
        }

        public override DataType Instantiate(IInstantiationContext context)
        {
            if (!IsGeneric())
            {
                return this;
            }

            TupleType result = new TupleType();
            for (int i = 0; i < elements.Count; i++)
            {
                result.addElement(new TupleTypeElement(elements[i].getName(),
                    elements[i].getType().Instantiate(context)));
            }

            return result;
        }
    }
}
