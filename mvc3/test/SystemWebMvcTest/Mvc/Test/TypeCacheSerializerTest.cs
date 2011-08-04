﻿namespace System.Web.Mvc.Test {
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Web.Mvc;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class TypeCacheSerializerTest {

        private const string _expectedDeserializationFormat = @"<?xml version=""1.0"" encoding=""utf-16""?>
<!--This file is automatically generated. Please do not modify the contents of this file.-->
<typeCache lastModified=""__IGNORED__"" mvcVersionId=""{0}"">
  <assembly name=""{1}"">
    <module versionId=""{2}"">
      <type>System.String</type>
      <type>System.Object</type>
    </module>
  </assembly>
</typeCache>";

        private static readonly string _mscorlibAsmFullName = typeof(object).Assembly.FullName;

        [TestMethod]
        public void DeserializeTypes_ReturnsNullIfModuleVersionIdDoesNotMatch() {
            // Arrange
            string expected = String.Format(_expectedDeserializationFormat,
                GetMvidForType(typeof(TypeCacheSerializer)) /* mvcVersionId */,
                _mscorlibAsmFullName /* assembly.name */,
                Guid.Empty /* module.versionId */
                );

            TypeCacheSerializer serializer = new TypeCacheSerializer();
            StringReader input = new StringReader(expected);

            // Act
            List<Type> deserializedTypes = serializer.DeserializeTypes(input);

            // Assert
            Assert.IsNull(deserializedTypes);
        }

        [TestMethod]
        public void DeserializeTypes_ReturnsNullIfMvcVersionIdDoesNotMatch() {
            // Arrange
            string expected = String.Format(_expectedDeserializationFormat,
                Guid.Empty /* mvcVersionId */,
                _mscorlibAsmFullName /* assembly.name */,
                GetMvidForType(typeof(object)) /* module.versionId */
                );

            TypeCacheSerializer serializer = new TypeCacheSerializer();
            StringReader input = new StringReader(expected);

            // Act
            List<Type> deserializedTypes = serializer.DeserializeTypes(input);

            // Assert
            Assert.IsNull(deserializedTypes);
        }

        [TestMethod]
        public void DeserializeTypes_ReturnsNullIfTypeNotFound() {
            string expectedFormat = @"<?xml version=""1.0"" encoding=""utf-16""?>
<!--This file is automatically generated. Please do not modify the contents of this file.-->
<typeCache lastModified=""__IGNORED__"" mvcVersionId=""{0}"">
  <assembly name=""{1}"">
    <module versionId=""{2}"">
      <type>System.String</type>
      <type>This.Type.Does.Not.Exist</type>
    </module>
  </assembly>
</typeCache>";

            // Arrange
            string expected = String.Format(expectedFormat,
                GetMvidForType(typeof(TypeCacheSerializer)) /* mvcVersionId */,
                _mscorlibAsmFullName /* assembly.name */,
                GetMvidForType(typeof(object)) /* module.versionId */
                );

            TypeCacheSerializer serializer = new TypeCacheSerializer();
            StringReader input = new StringReader(expected);

            // Act
            List<Type> deserializedTypes = serializer.DeserializeTypes(input);

            // Assert
            Assert.IsNull(deserializedTypes);
        }

        [TestMethod]
        public void DeserializeTypes_Success() {
            // Arrange
            string expected = String.Format(_expectedDeserializationFormat,
                GetMvidForType(typeof(TypeCacheSerializer)) /* mvcVersionId */,
                _mscorlibAsmFullName /* assembly.name */,
                GetMvidForType(typeof(object)) /* module.versionId */
                );

            TypeCacheSerializer serializer = new TypeCacheSerializer();
            StringReader input = new StringReader(expected);

            Type[] expectedTypes = new Type[]{
                typeof(string),
                typeof(object)
            };

            // Act
            List<Type> deserializedTypes = serializer.DeserializeTypes(input);

            // Assert
            CollectionAssert.AreEquivalent(expectedTypes, deserializedTypes);
        }

        [TestMethod]
        public void SerializeTypes() {
            // Arrange
            DateTime expectedDate = new DateTime(2001, 1, 1, 0, 0, 0, DateTimeKind.Utc); // Jan 1, 2001 midnight UTC
            string expectedFormat = @"<?xml version=""1.0"" encoding=""utf-16""?>
<!--This file is automatically generated. Please do not modify the contents of this file.-->
<typeCache lastModified=""{0}"" mvcVersionId=""{1}"">
  <assembly name=""{2}"">
    <module versionId=""{3}"">
      <type>System.String</type>
      <type>System.Object</type>
    </module>
  </assembly>
</typeCache>";
            string expected = String.Format(expectedFormat,
                expectedDate /* lastModified */,
                GetMvidForType(typeof(TypeCacheSerializer)) /* mvcVersionId */,
                _mscorlibAsmFullName /* assembly.name */,
                GetMvidForType(typeof(object)) /* module.versionId */
                );

            Type[] typesToSerialize = new Type[]{
                typeof(string),
                typeof(object)
            };

            TypeCacheSerializer serializer = new TypeCacheSerializer();
            serializer.CurrentDateOverride = expectedDate;

            StringWriter output = new StringWriter();

            // Act
            serializer.SerializeTypes(typesToSerialize, output);
            string outputString = output.ToString();

            // Assert
            Assert.AreEqual(expected, outputString);
        }

        private static Guid GetMvidForType(Type type) {
            return type.Module.ModuleVersionId;
        }

    }
}