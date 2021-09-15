using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace model.cql.hl7.org
{
    public interface INamedType
    {
        string getName();
        string getNamespace();
        string getSimpleName();
        string getTarget();
    }

    public class ClassType : DataType, INamedType
    {
        private List<ClassTypeElement> elements = new List<ClassTypeElement>();
        private SortedList<string, ClassTypeElement> sortedElements = null;
        private Dictionary<string, ClassTypeElement> baseElementMap = null;


        public ClassType(string name, DataType baseType, ICollection<ClassTypeElement> elements, ICollection<TypeParameter> parameters)
            :  base(baseType)
        {
            if (name == null || name.Equals(""))
            {
                throw new ArgumentException("name is null");
            }
            
            this.name = name;

            if (parameters != null)
            {
                this.genericParameters.AddRange(parameters);
            }

            if (elements != null)
            {
                this.elements.AddRange(elements);
            }
        }

        public ClassType() : this(null, null, null, null)
        {
        }

        public ClassType(string name) : this(name, null, null, null)
        {
        }

        public ClassType(string name, DataType baseType) : this(name, baseType, null, null)
        {
        }

        public ClassType(string name, DataType baseType, ICollection<ClassTypeElement> elements) 
            : this(name, baseType, elements, null)
        {
        }

        private string name;

        public string getName()
        {
            return this.name;
        }

        public string getNamespace()
        {
            if (this.name != null)
            {
                int qualifierIndex = this.name.IndexOf('.');//TODO Should this not be the last occurrence rather than the first occurrence?
                if (qualifierIndex > 0)
                {
                    return this.name.Substring(0, qualifierIndex);
                }
            }

            return "";
        }

        public string getSimpleName()
        {
            if (this.name != null)
            {
                int qualifierIndex = this.name.IndexOf('.');//TODO Should this not be the last occurrence rather than the first occurrence?
                if (qualifierIndex > 0)
                {
                    return this.name.Substring(qualifierIndex + 1);
                }
            }

            return this.name;
        }

        private string identifier;
        public string getIdentifier() { return identifier; }
        public void setIdentifier(string identifier) { this.identifier = identifier; }

        private string label;
        public string getLabel() { return label; }
        public void setLabel(string label) { this.label = label; }

        private string target;
        public string getTarget() { return target; }
        public void setTarget(string target) { this.target = target; }

        private bool retrievable;
        public bool isRetrievable()
        {
            return retrievable;
        }
        public void setRetrievable(bool retrievable)
        {
            this.retrievable = retrievable;
        }

        private string primaryCodePath;
        public string getPrimaryCodePath()
        {
            return primaryCodePath;
        }
        public void setPrimaryCodePath(string primaryCodePath)
        {
            this.primaryCodePath = primaryCodePath;
        }

        private string primaryValueSetPath;
        public string getPrimaryValueSetPath() { return primaryValueSetPath; }
        public void setPrimaryValueSetPath(string primaryValueSetPath)
        {
            this.primaryValueSetPath = primaryValueSetPath;
        }

        private List<Relationship> relationships = new List<Relationship>();
        public IEnumerable<Relationship> getRelationships()
        {
            return relationships;
        }

        public void addRelationship(Relationship relationship)
        {
            relationships.Add(relationship);
        }

        private List<Relationship> targetRelationships = new List<Relationship>();
        public IEnumerable<Relationship> getTargetRelationships()
        {
            return targetRelationships;
        }

        public void addTargetRelationship(Relationship relationship)
        {
            targetRelationships.Add(relationship);
        }

        private List<SearchType> searches = new List<SearchType>();
        public IEnumerable<SearchType> getSearches()
        {
            return searches;
        }

        public void addSearch(SearchType search)
        {
            searches.Add(search);
        }

        public SearchType findSearch(string searchPath)
        {
            if (searches != null)
            {
                foreach (SearchType search in searches)
                {
                    if (search.getName().Equals(searchPath))
                    {
                        return search;
                    }
                }
            }

            return null;
        }

        /**
         * Generic class parameters such 'S', 'T extends MyType'.
         */
        private List<TypeParameter> genericParameters = new List<TypeParameter>();

        /**
         * Returns the generic parameters for the generic type. For instance,
         * for the generic type Map&lt;K,V extends Person&gt;, two generic parameters
         * will be returned: K and V extends Person. The latter parameter has a constraint
         * restricting the type of the bound type to be a valid subtype of Person.
         *
         * @return Class' generic parameters
         */
        public IList<TypeParameter> getGenericParameters()
        {
            return genericParameters;
        }

        /**
         * Sets the generic parameters for the generic type. For instance,
         * for the generic type Map&lt;K,V extends Person&gt;, two generic parameters
         * should be set: K and V extends Person. The latter parameter has a constraint
         * restricting the type of the bound type to be a valid subtype of Person.
         *
         * @param genericParameters
         */
        public void setGenericParameters(List<TypeParameter> genericParameters)
        {
            this.genericParameters = genericParameters;
        }

        /**
         * Adds a parameter declaration to the generic type.
         *
         * @param genericParameter
         */
        public void addGenericParameter(TypeParameter genericParameter)
        {
            this.genericParameters.Add(genericParameter);
        }

        /**
         * Adds collection of type parameters to existing set.
         * @param parameters
         */
        public void addGenericParameter(ICollection<TypeParameter> parameters)
        {
            foreach (TypeParameter parameter in parameters)
            {
                internalAddParameter(parameter);
            }

            sortedElements = null;
            tupleType = null;
        }

        /**
         * Returns the parameter with the given parameter identifier.
         * If not found in the given class, it looks in the parent class.
         *
         * @param identifier
         * @return Generic parameter with the given name in the current class or in the base class. Null if none found.
         */
        public TypeParameter getGenericParameterByIdentifier(string identifier)
        {
            return getGenericParameterByIdentifier(identifier, false);
        }

        /**
         * Returns the parameter with the given parameter identifier.
         * If inCurrentClassOnly is false, if not found in the given class, then it looks in the parent class.
         * If inCurrentClassOnly is true, only looks for parameter in the given class.
         *
         * @param identifier
         * @param inCurrentClassOnly
         * @return Class' generic parameter
         */
        public TypeParameter getGenericParameterByIdentifier(string identifier, bool inCurrentClassOnly)
        {
            TypeParameter param = null;
            foreach (TypeParameter genericParameter in genericParameters)
            {
                if (identifier.Equals(genericParameter.Identifier, StringComparison.OrdinalIgnoreCase))
                {
                    param = genericParameter;
                    break;
                }
            }
            if (!inCurrentClassOnly && param == null)
            {
                if (param == null && BaseType is ClassType) {
                    param = ((ClassType)BaseType).getGenericParameterByIdentifier(identifier);
                }
            }
            return param;
        }
        

        public IList<ClassTypeElement> getElements()
        {
            return elements;
        }

        private Dictionary<string, ClassTypeElement> getBaseElementMap()
        {
            if (baseElementMap == null)
            {
                baseElementMap = new Dictionary<string, ClassTypeElement>();
                if (BaseType is ClassType) {
                    ((ClassType)BaseType).gatherElements(baseElementMap);
                }
            }

            return baseElementMap;
        }

        private void gatherElements(Dictionary<string, ClassTypeElement> elementMap)
        {
            if (BaseType is ClassType) {
                ((ClassType)BaseType).gatherElements(elementMap);
            }

            foreach (ClassTypeElement element in elements)
            {
                elementMap.Add(element.getName(), element);
            }
        }

        public IList<ClassTypeElement> getAllElements()
        {
            // Get the baseClass elements into a map by name
            Dictionary<string, ClassTypeElement> elementMap = new Dictionary<string, ClassTypeElement>(getBaseElementMap());

            // Add this class's elements, overwriting baseClass definitions where applicable
            foreach (ClassTypeElement el in elements)
            {
                elementMap.Add(el.getName(), el);
            }

            return new List<ClassTypeElement>(elementMap.Values);
        }

        private void internalAddElement(ClassTypeElement element)
        {
            if (getBaseElementMap().TryGetValue(element.getName(), out var existingElement))
            {
                if (!(existingElement.getType() is TypeParameter)
                    && (
                    !(
                        element.getType().IsSubTypeOf(existingElement.getType())
                        || (
                            existingElement.getType() is ListType
                            && element.getType().IsSubTypeOf(((ListType)existingElement.getType()).getElementType())
                            )
                        || (
                                existingElement.getType() is IntervalType
                            && element.getType().IsSubTypeOf(((IntervalType)existingElement.getType()).GetPointType())
                            )
                        || (
                                existingElement.getType() is ChoiceType
                            && element.getType().IsCompatibleWith(existingElement.getType())
                            )
                    )
                ))
                {
                    throw new InvalidRedeclarationException(this, existingElement, element);
                }
            }

            
        }

        private void internalAddParameter(TypeParameter parameter)
        {
            //TODO Flesh out and retain method only if needed.

            this.genericParameters.Add(parameter);
        }

        public void addElement(ClassTypeElement element)
        {
            internalAddElement(element);
            sortedElements = null;
            tupleType = null;
        }

        public void addElements(ICollection<ClassTypeElement> elements)
        {
            foreach (ClassTypeElement element in elements)
            {
                internalAddElement(element);
            }

            sortedElements = null;
            tupleType = null;
        }

        private List<ClassTypeElement> getSortedElements()
        {
            if (sortedElements == null)
            {
                sortedElements = new SortedList<string, ClassTypeElement>();
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
            return this.name.GetHashCode();
        }

        /// <summary>Determines whether the specified object is equal to the current object.</summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>true if the specified object  is equal to the current object; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            if (obj is ClassType) {
                ClassType that = (ClassType)obj;
                return this.name.Equals(that.name);
            }

            return false;
        }
        
        
    public override string ToString()
        {
            return this.name;
        }

        
    public override string ToLabel()
        {
            return this.label == null ? this.name : this.label;
        }

        private TupleType tupleType;
        public TupleType getTupleType()
        {
            if (tupleType == null)
            {
                tupleType = buildTupleType();
            }

            return tupleType;
        }

        private void addTupleElements(ClassType classType, Dictionary<string, TupleTypeElement> elements)
        {
            // Add base elements first
            DataType baseType = classType.BaseType;
            if (baseType is ClassType) {
                addTupleElements((ClassType)baseType, elements);
            }

            foreach (ClassTypeElement element in classType.getElements())
            {
                if (!element.isProhibited())
                {
                    TupleTypeElement tupleElement = new TupleTypeElement(element.getName(), element.getType());
                    elements.Add(tupleElement.getName(), tupleElement);
                }
            }
        }

        private TupleType buildTupleType()
        {
            Dictionary<string, TupleTypeElement> tupleElements = new Dictionary<string, TupleTypeElement>();

            addTupleElements(this, tupleElements);

            return new TupleType(tupleElements.Values.ToList());
        }

        
    public override bool IsCompatibleWith(DataType other)
        {
            if (other is TupleType) {
                TupleType tupleType = (TupleType)other;
                return getTupleType().Equals(tupleType);
                // Github #115: It's incorrect for a class type to be considered compatible with another class type on the basis of the inferred tuple type alone.
                //} else if (other instanceof ClassType) {
                //    ClassType classType = (ClassType)other;
                //    return getTupleType().Equals(classType.getTupleType());
            }

            return base.IsCompatibleWith(other);
        }
        
    public override bool IsGeneric()
        {
            return genericParameters != null && genericParameters.Count > 0;
        }

    
    public override bool IsInstantiable(DataType callType, IInstantiationContext context)
        {
            if (callType is ClassType) {
                ClassType classType = (ClassType)callType;
                if (elements.Count == classType.elements.Count)
                {
                    List<ClassTypeElement> theseElements = getSortedElements();
                    List<ClassTypeElement> thoseElements = classType.getSortedElements();
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

            ClassType result = new ClassType(getName(), BaseType);
            for (int i = 0; i < elements.Count; i++)
            {
                result.addElement(new ClassTypeElement(elements[i].getName(), elements[i].getType().Instantiate(context)));
            }

            return result;
        }
    }

}
