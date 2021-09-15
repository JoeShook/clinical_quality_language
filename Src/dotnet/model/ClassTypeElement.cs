using System;

namespace model.cql.hl7.org
{
    public class ClassTypeElement
    {
        private string name;
        private DataType type;
        private bool prohibited;
        private bool oneBased;
        private string target;

        public ClassTypeElement(String name, DataType type, bool prohibited, bool oneBased, string target)
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
            this.prohibited = prohibited != null ? prohibited : false;
            this.oneBased = oneBased != null ? oneBased : false;
            this.target = target;
        }

        public ClassTypeElement(String name, DataType type) : this(name, type, false, false, null)
        {
        }

        public string getName()
        {
            return this.name;
        }

        public DataType getType()
        {
            return this.type;
        }

        public bool isProhibited()
        {
            return prohibited;
        }

        public bool isOneBased()
        {
            return oneBased;
        }

        public string GetTarget()
        {
            return target;
        }

        /// <summary>Determines whether the specified object is equal to the current object.</summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>true if the specified object  is equal to the current object; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            if (this == obj)
            {
                return true;
            }
            if (!(obj is ClassTypeElement)) {
                return false;
            }

            ClassTypeElement that = (ClassTypeElement)obj;

            if (target != null && !target.Equals(that.target))
            {
                return false;
            }
            if (oneBased != that.oneBased)
            {
                return false;
            }
            if (prohibited != that.prohibited)
            {
                return false;
            }
            if (!name.Equals(that.name))
            {
                return false;
            }
            if (!type.Equals(that.type))
            {
                return false;
            }

            return true;
        }

        /// <summary>Serves as the default hash function.</summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            int result = name.GetHashCode();
            result = 31 * result + type.GetHashCode();
            result = 31 * result + (prohibited ? 1 : 0);
            result = 31 * result + (oneBased ? 1 : 0);
            if (target != null)
            {
                result = 31 * result + (target.GetHashCode());
            }
            return result;
        }

        public bool IsSubTypeOf(ClassTypeElement that)
        {
            return this.getName().Equals(that.getName()) && this.getType().IsSubTypeOf(that.getType());
        }

        public bool IsSuperTypeOf(ClassTypeElement that)
        {
            return this.getName().Equals(that.getName()) && this.getType().IsSuperTypeOf(that.getType());
        }

    public override string ToString()
        {
            return String.Format("{0}:{1}{2}{3}{4}",
                    this.name,
                    this.type,
                    this.prohibited ? " (prohibited)" : "",
                    this.oneBased ? " (one-based)" : "",
                    this.target != null ? " (target: " + this.target + ")" : ""
            );
        }
    }

}
