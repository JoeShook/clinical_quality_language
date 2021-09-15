using System;

namespace model.cql.hl7.org
{
    public class InvalidRedeclarationException : ArgumentException
    {
        public InvalidRedeclarationException(ClassType classType, ClassTypeElement original, ClassTypeElement redeclared) 
            : base(String.Format("{0}.{1} cannot be redeclared with type {2} because it is not a subtype of the original element type {3}",
                classType.getName(), redeclared.getName(), redeclared.getType(), original.getType()))
        {
        }
    }
}
