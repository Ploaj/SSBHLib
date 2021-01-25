using Microsoft.Data.Sqlite;
using SSBHLib.Formats.Materials;
using System.Collections.Generic;
using System.Linq;

namespace CrossMod.MaterialValidation
{
    public static class ShaderValidation
    {
        const string connectionString = "Data Source=Nufxb.db";

        /// <summary>
        /// Gets the names of the vertex attributes used by <paramref name="shaderLabel"/>.
        /// This does not include Position0, Normal0, or Tangent0.
        /// </summary>
        /// <param name="shaderLabel">The name of the shader, including the render pass tag</param>
        /// 
        /// <returns>The vertex attribute names for <paramref name="shaderLabel"/></returns>
        public static List<string> GetAttributes(string shaderLabel)
        {
            using var connection = new SqliteConnection(connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText =
                @"
                    SELECT AttributeName FROM 
                    VertexAttribute
                    WHERE ShaderProgramID IN 
                    (
	                    SELECT ID
	                    FROM ShaderProgram
	                    WHERE 
		                    Name = $shaderLabel
                    )
                ";
            command.Parameters.AddWithValue("$shaderLabel", shaderLabel);

            var attributes = new List<string>();
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    attributes.Add(reader.GetString(0));
                }
            }

            return attributes;
        }

        /// <summary>
        /// Gets the <see cref="MatlEnums.ParamId"/> used by <paramref name="shaderLabel"/>.
        /// </summary>
        /// <param name="shaderLabel">The name of the shader, including the render pass tag</param>
        /// 
        /// <returns>The material parameters for <paramref name="shaderLabel"/></returns>
        public static List<MatlEnums.ParamId> GetParameters(string shaderLabel)
        {
            using var connection = new SqliteConnection(connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText =
                @"
                    SELECT ParamID FROM 
                    MaterialParameter
                    WHERE ShaderProgramID IN 
                    (
	                    SELECT ID
	                    FROM ShaderProgram
	                    WHERE 
		                    Name = $shaderLabel
                    )
                ";
            command.Parameters.AddWithValue("$shaderLabel", shaderLabel);

            var parameters = new List<MatlEnums.ParamId>();
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    // These are in game values, so the conversion should be safe.
                    parameters.Add((MatlEnums.ParamId)reader.GetInt64(0));
                }
            }
            return parameters;
        }

        /// <summary>
        /// Checks if <paramref name="attributeName"/> is present for in game meshes referencing <paramref name="shaderLabel"/>.
        /// </summary>
        /// <param name="shaderLabel">The name of the shader, including the render pass tag</param>
        /// <param name="attributeName">The name of the attribute</param>
        /// <returns><c>true</c> if the attribute is associated with the shader</returns>
        public static bool HasAttribute(string shaderLabel, string attributeName)
        {
            using var connection = new SqliteConnection("Data Source=Nufxb.db");
            connection.Open();

            // TODO: Use the decompiled shader attribute table.
            // Color sets are split between three attributes, 
            // so it isn't always possible to determine if an attribute is used.
            var command = connection.CreateCommand();
            command.CommandText =
                @"
                    SELECT * FROM 
                    VertexAttribute
                    WHERE VertexAttribute.AttributeName = $attribute AND ShaderProgramID IN 
                    (
	                    SELECT ID
	                    FROM ShaderProgram
	                    WHERE 
		                    Name = $shaderLabel
                    )
                ";
            command.Parameters.AddWithValue("$attribute", attributeName);
            command.Parameters.AddWithValue("$shaderLabel", shaderLabel);

            // Read() returns true if there are any results.
            using var reader = command.ExecuteReader();
            return reader.Read();
        }

        /// <summary>
        /// Checks if <paramref name="shaderLabel"/> is a valid shader name.
        /// </summary>
        /// <param name="shaderLabel">The name of the shader, including the render pass tag</param>
        /// <returns><c>true</c> if a shader exists with the name <paramref name="shaderLabel"/></returns>
        public static bool IsValidShaderLabel(string shaderLabel)
        {
            using var connection = new SqliteConnection(connectionString);
            connection.Open();

            // The current database only contains correlations 
            // between shaders and mesh attributes based on material assignments.
            // TODO: Use a dump of decompiled shaders to find the actual attributes?
            var command = connection.CreateCommand();
            command.CommandText =
                @"
                    SELECT *
                    FROM ShaderProgram
                    WHERE 
                        Name = $shaderLabel
                ";
            command.Parameters.AddWithValue("$shaderLabel", shaderLabel);

            // Read() returns true if there are any results.
            using var reader = command.ExecuteReader();
            return reader.Read();
        }

        /// <summary>
        /// Checks if <paramref name="expectedParameters"/> has exactly the same set of material parameters
        /// as <paramref name="shaderLabel"/>, ignoring the order of elements.
        /// </summary>
        /// <param name="shaderLabel">The name of the shader, including the render pass tag</param>
        /// <param name="expectedParameters">The material parameter list to check for the shader</param>
        /// <returns><c>true</c> if the parameter lists match</returns>
        public static bool IsValidParameterList(string shaderLabel, ICollection<MatlEnums.ParamId> expectedParameters)
        {
            using var connection = new SqliteConnection(connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText =
                @"
                    SELECT ParamID FROM 
                    MaterialParameter
                    WHERE ShaderProgramID IN 
                    (
	                    SELECT ID
	                    FROM ShaderProgram
	                    WHERE 
		                    Name = $shaderLabel
                    )
                ";
            command.Parameters.AddWithValue("$shaderLabel", shaderLabel);

            var actualParameters = new List<MatlEnums.ParamId>();
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    // These are in game values, so the conversion should be safe.
                    actualParameters.Add((MatlEnums.ParamId)reader.GetInt64(0));
                }
            }

            // Check that the two lists have the same elements, regardless of order.
            return (actualParameters.Count == expectedParameters.Count)
                && !actualParameters.Except(expectedParameters).Any();
        }

        /// <summary>
        /// Checks if <paramref name="expectedAttributes"/> has exactly the same set of attributes
        /// as <paramref name="shaderLabel"/>, ignoring the order of elements.
        /// </summary>
        /// <param name="shaderLabel">The name of the shader, including the render pass tag</param>
        /// <param name="expectedAttributes">The attribute list to check for the shader</param>
        /// <returns><c>true</c> if the attribute lists match</returns>
        public static bool IsValidAttributeList(string shaderLabel, ICollection<string> expectedAttributes)
        {
            using var connection = new SqliteConnection(connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText =
                @"
                    SELECT AttributeName FROM 
                    VertexAttribute
                    WHERE ShaderProgramID IN 
                    (
	                    SELECT ID
	                    FROM ShaderProgram
	                    WHERE 
		                    Name = $shaderLabel
                    )
                ";
            command.Parameters.AddWithValue("$shaderLabel", shaderLabel);

            var actualAttributes = new List<string>();
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    actualAttributes.Add(reader.GetString(0));
                }
            }

            // TODO: Don't assume that shaders have position, normals, and tangents.
            // Only color sets and texture coordinates are in the current database.
            actualAttributes.Add("Position0");
            actualAttributes.Add("Normal0");
            actualAttributes.Add("Tangent0");

            // Check that the two lists have the same elements, regardless of order.
            return (actualAttributes.Count == expectedAttributes.Count)
                && !actualAttributes.Except(expectedAttributes).Any();
        }
    }
}
