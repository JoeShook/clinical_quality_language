using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;

namespace model.cql.hl7.org
{
    /// <summary>
    /// The GenericClassSignatureParser is a convenience class for the parsing of generic signature
    /// and the creation of the corresponding CQL DataTypes, namely, GenericClassType and GenericClassProfileType.
    /// The former is used to capture the declaration of a GenericClass such as List&lt;T&gt;. The latter is used to capture a new type
    /// such as 'IntegerList' formed by binding types to generic parameters such as List&lt;Integer&gt;.
    /// </summary>
    public class GenericClassSignatureParser
    {
        public const char OPEN_BRACKET = '<';
        public const char CLOSE_BRACKET = '>';
        public const string EXTENDS = "extends";

        private int startPos = 0;
        private int endPos = 0;
        private int bracketCount = 0;
        private int currentBracketPosition = 0;
        private Hashtable resolvedTypes;

        /**
         * A generic signature such as List&lt;T&gt; or a bound signature
         * such as List&lt;Person&gt;
         */
        private string genericSignature;

        /**
         * The base type for the class type or the profile.
         */
        private string baseType;

        /**
         * The name of a bound type such as PersonList = List&lt;Person&gt;
         */
        private string boundGenericTypeName;

        public GenericClassSignatureParser(
            string genericSignature,
            string baseType,
            string boundGenericTypeName,
            Hashtable resolvedTypes)
        {
            this.genericSignature = genericSignature;
            this.resolvedTypes = resolvedTypes;
            this.baseType = baseType;
            this.boundGenericTypeName = boundGenericTypeName;
        }

        public GenericClassSignatureParser(
            string genericSignature,
            Hashtable resolvedTypes)
            : this(genericSignature, null, null, resolvedTypes)
        {
        }

        public ClassType ParseGenericSignature()
        {
            string genericTypeName = genericSignature;
            List<string> _params = new List<string>();

            if (IsValidGenericSignature())
            {
                var start = genericSignature.IndexOf('<');
                genericTypeName = genericSignature.Substring(0, start);
                string parameters = genericSignature.Substring(start  + 1,
                    genericSignature.LastIndexOf('>') - start - 1);
                _params = escapeNestedCommas(parameters).Split(',').ToList();
            }

            string baseTypeName = baseType;
            String[] baseTypeParameters = null;
            if (baseType != null && baseType.Contains("<"))
            {
                baseTypeName = baseType.Substring(0, baseType.IndexOf('<'));
                string baseTypeParameterString =
                    baseType.Substring(baseType.IndexOf('<') + 1, baseType.LastIndexOf('>'));
                baseTypeParameters = escapeNestedCommas(baseTypeParameterString).Split(',');
            }

            DataType baseDataType = ResolveTypeName(baseTypeName);
            ClassType genericClassType = new ClassType(genericTypeName, baseDataType);
            foreach (string param in _params)
            {
                TypeParameter paramType = HandleParameterDeclaration(unescapeNestedCommas(param));
                genericClassType.addGenericParameter(paramType);
            }

            if (baseTypeParameters != null)
            {
                int index = 0;
                foreach (string baseTypeParameter in baseTypeParameters)
                {
                    if (baseTypeParameter.Length == 1 &&
                        genericClassType.getGenericParameterByIdentifier(baseTypeParameter) == null)
                    {
                        // TODO: not what I want.  But best port from Java code at the moment.
                        throw new SystemException("Cannot resolve symbol " + baseTypeParameter);
                    }
                    else
                    {
                        DataType boundType = ResolveTypeName(unescapeNestedCommas(baseTypeParameter));
                        ClassType baseTypeClass = (ClassType)baseDataType;
                        IList<ClassTypeElement> baseClassFields = baseTypeClass.getElements();
                        string myParam = baseTypeClass.getGenericParameters()[index].Identifier;
                        Console.WriteLine(boundType + " replaces param " + myParam);

                        foreach (ClassTypeElement baseClassField in baseClassFields)
                        {
                            ClassTypeElement myElement = new ClassTypeElement(baseClassField.getName(), boundType);
                            genericClassType.addElement(myElement);
                        }
                    }

                    index++;
                }
            }

            return genericClassType;
        }


        /**
     * Method handles a generic parameter declaration such as T, or T extends MyType.
     *
     * @param parameterString
     * @return Type parameter for this parameter for this string declaration.
     */
        protected TypeParameter HandleParameterDeclaration(String parameterString)
        {
            string[] paramComponents = Regex.Split(parameterString, "\\s+");
            if (paramComponents.Length == 1)
            {
                return new TypeParameter(parameterString.Trim(), TypeParameter.TypeParameterConstraint.NONE, null);
            }
            else if (paramComponents.Length == 3)
            {
                if (paramComponents[1].Equals(EXTENDS, StringComparison.OrdinalIgnoreCase))
                {
                    return new TypeParameter(paramComponents[0], TypeParameter.TypeParameterConstraint.TYPE,
                        ResolveTypeName(paramComponents[2]));
                }
                else
                {
                    // TODO: not what I want.  But best port from Java code at the moment.
                    throw new SystemException("Invalid parameter syntax: " + parameterString);
                }
            }
            else
            {
                // TODO: not what I want.  But best port from Java code at the moment.
                throw new SystemException("Invalid parameter syntax: " + parameterString);
            }
        }

        /**
     * Identifies the data type for the named type. If the argument is null,
     * the return type will be null.
     *
     * @param parameterType
     * @return The parameter's type
     */
        protected DataType ResolveTypeName(String parameterType)
        {
            if (IsValidGenericSignature(parameterType))
            {
                return HandleBoundType(parameterType);
            }
            else
            {
                if (parameterType == null)
                {
                    return null;
                }
                else
                {
                    return ResolveType(parameterType);
                }
            }
        }

        /**
         * Method resolves bound type if it exists or else creates it and
         * adds it to the resolved type index.
         *
         * @param boundGenericSignature
         * @return The bound type created or resolved
         */
        protected DataType HandleBoundType(string boundGenericSignature)
        {
            ClassType resolvedType;

            if (resolvedTypes.Contains(EscapeNestedAngleBrackets(boundGenericSignature)))
            {
                resolvedType = resolvedTypes[EscapeNestedAngleBrackets(boundGenericSignature)] as ClassType;

                return resolvedType;
            }
            else
            {
                string genericTypeName = boundGenericSignature.Substring(0, boundGenericSignature.IndexOf('<'));
                resolvedType = (ClassType) ResolveType(genericTypeName);
                if (resolvedType == null)
                {
                    // TODO: not what I want.  But best port from Java code at the moment.
                    throw new SystemException("Unknown type " + genericTypeName);
                }

                ClassType newType = new ClassType(EscapeNestedAngleBrackets(boundGenericSignature), resolvedType);
                String parameters = boundGenericSignature.Substring(boundGenericSignature.IndexOf('<') + 1,
                    boundGenericSignature.LastIndexOf('>'));
                string[] _params = escapeNestedCommas(parameters).Split(',');
                int index = 0;
                foreach (string param in _params)
                {
                    DataType boundParam = null;
                    var paramCopy = unescapeNestedCommas(param);
                    if (IsValidGenericSignature(paramCopy))
                    {
                        boundParam = HandleBoundType(paramCopy);
                    }
                    else
                    {
                        boundParam = ResolveType(paramCopy);
                    }

                    TypeParameter typeParameter = resolvedType.getGenericParameters()[index];

                    foreach (ClassTypeElement classTypeElement in resolvedType.getElements())
                    {
                        if (classTypeElement.getType() is TypeParameter)
                        {
                            if (((TypeParameter) classTypeElement.getType()).Identifier
                                .Equals(typeParameter.Identifier, StringComparison.OrdinalIgnoreCase))
                            {
                                ClassTypeElement newElement =
                                    new ClassTypeElement(classTypeElement.getName(), boundParam);
                                newType.addElement(newElement);
                            }
                        }
                    }

                    index++;
                }

                resolvedTypes.Add(newType.getName(), newType);

                return newType;
            }
        }

        /// <summary>
        /// Returns true if the generic signature passed as an argument is well-formed.
        /// </summary>
        /// <returns>True if the generic signature is valid</returns>
        public bool IsValidGenericSignature()
        {
            return IsValidGenericSignature(genericSignature);
        }

        /// <summary>
        /// Returns true if the generic signature passed as an argument is well-formed.
        /// </summary>
        /// <param name="genericSignature"></param>
        /// <returns>True if the generic signature is valid</returns>
        public bool IsValidGenericSignature(string genericSignature)
        {
            return AreBracketsPaired(genericSignature) && ClosingBracketsComeAfterOpeningBrackets(genericSignature);
        }


        private bool AreBracketsPaired(string signatureString)
        {
            bool paired = false;
            if (signatureString != null)
            {
                int openCount = OpenBracketCount(signatureString);
                int closeCount = CloseBracketCount(signatureString);
                paired = (openCount == closeCount) && (openCount > 0);
            }

            return paired;
        }

        private int OpenBracketCount(string signatureString)
        {
            int matchCount = 0;
            if (signatureString != null)
            {
                matchCount = signatureString.Count(c => c == OPEN_BRACKET);
            }

            return matchCount;
        }

        private int CloseBracketCount(string signatureString)
        {
            int matchCount = 0;
            if (signatureString != null)
            {
                matchCount = signatureString.Count(c => c == CLOSE_BRACKET);
            }

            return matchCount;
        }

        private bool ClosingBracketsComeAfterOpeningBrackets(string signatureString)
        {
            return signatureString != null && signatureString.LastIndexOf('<') < signatureString.IndexOf('>');
        }

        /**
     * Convenience method for the parsing of nested comma-separated parameters.
     * Call before unescapeNestedCommas when done processing the top level of nested parameters.
     *
     * @param signature
     * @return
     */
        private string escapeNestedCommas(String signature)
        {
            char[] signatureCharArray = signature.ToCharArray();
            int openBracketCount = 0;
            for (int index = 0; index < signatureCharArray.Length; index++)
            {
                char c = signatureCharArray[index];
                if (c == '<')
                {
                    openBracketCount++;
                }
                else if (c == '>')
                {
                    openBracketCount--;
                }
                else if (c == ',' && openBracketCount > 0)
                {
                    signatureCharArray[index] = '|';
                }
            }

            return new String(signatureCharArray);
        }

        /**
         * Convenience method for the parsing of nested comma-separated parameters.
         * Call after escapeNestedCommas when handling the top level of nested parameters.
         *
         * @param escapedSignature
         * @return
         */
        private String unescapeNestedCommas(String escapedSignature)
        {
            return escapedSignature.Replace("\\|", ",");
        }

        /**
         * Method looks up data type of typeName is index.
         *
         * @param typeName
         * @return
         */
        private DataType ResolveType(String typeName)
        {
            if (!resolvedTypes.Contains(typeName))
            {
                // TODO: not what I want.  But best port from Java code at the moment.
                throw new SystemException("Unable to resolve " + typeName);
            }

            return resolvedTypes[typeName] as DataType;
        }

        private String EscapeNestedAngleBrackets(String genericSignature)
        {
            return genericSignature.Replace("<", "[").Replace(">", "]");
        }

    }
}





