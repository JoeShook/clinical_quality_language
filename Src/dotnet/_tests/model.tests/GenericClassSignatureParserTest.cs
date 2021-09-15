using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace model.cql.hl7.org
{
    public class GenericClassSignatureParserTest
    {
        [Fact]
        public void parseTest1()
        {
            GenericClassSignatureParser genericClassSignatureParser =
                new GenericClassSignatureParser("MyType<M,N>", null);

            Assert.NotNull(genericClassSignatureParser);
            Assert.True(genericClassSignatureParser.IsValidGenericSignature());

            ClassType signature = genericClassSignatureParser.ParseGenericSignature();

            Assert.Equal("MyType", signature.getName());
            Assert.Equal(2, signature.getGenericParameters().Count);
            Assert.Equal("M", signature.getGenericParameters()[0].Identifier);
            Assert.Equal("N", signature.getGenericParameters()[1].Identifier);
        }

        [Fact]
        public void parseTest2()
        {
            ClassType collectionType = new ClassType("Collection", null, null);
            Hashtable resolvedTypes = new Hashtable();
            resolvedTypes.Add("Collection", collectionType);
            
            
            //TODO: stopping here::  This is Java parsing so I need to understand what this code really does before spending more time here.
            GenericClassSignatureParser genericClassSignatureParser = new GenericClassSignatureParser("MyType<M extends Collection,N>", resolvedTypes);
            
            Assert.True(genericClassSignatureParser.IsValidGenericSignature());
            
            ClassType signature = genericClassSignatureParser.ParseGenericSignature();

            Assert.Equal("MyType", signature.getName());
            Assert.Equal(2, signature.getGenericParameters().Count);
            Assert.Equal("M", signature.getGenericParameters()[0].Identifier);
            Assert.Equal(TypeParameter.TypeParameterConstraint.TYPE, signature.getGenericParameters()[0].constraint);
            Assert.Equal("Collection", ((ClassType)signature.getGenericParameters()[0].GetConstraintType()).getName());
            Assert.Equal("N", signature.getGenericParameters()[1].Identifier);
        }

        [Fact]
        public void parseTest3()
        {

        }
    }
}