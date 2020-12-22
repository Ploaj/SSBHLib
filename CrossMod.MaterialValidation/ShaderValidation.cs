using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using System.Linq;

namespace CrossMod.MaterialValidation
{
    public static class ShaderValidation
    {
        /// <summary>
        /// Checks if <paramref name="attributeName"/> is present for in game meshes referencing <paramref name="shaderLabel"/>.
        /// </summary>
        /// <param name="shaderLabel">The name of the shader, including the tag</param>
        /// <param name="attributeName">The name of the attribute</param>
        /// <returns><c>true</c> if the attribute is associated with the shader</returns>
        public static bool HasAttribute(string shaderLabel, string attributeName)
        {
            using var connection = new SqliteConnection("Data Source=SmushAttributes.db");
            connection.Open();

            // TODO: Use the decompiled shader attribute table.
            // Color sets are split between three attributes, 
            // so it isn't always possible to determine if an attribute is used.
            var command = connection.CreateCommand();
            command.CommandText =
                @"
                    SELECT *
                    FROM ShaderMeshAttributes
                    WHERE 
                        MeshAttribute = $attribute AND
                        ShaderLabel = $shaderLabel
                ";
            command.Parameters.AddWithValue("$attribute", attributeName);
            command.Parameters.AddWithValue("$shaderLabel", shaderLabel);

            // Read() returns true if there are any results.
            using var reader = command.ExecuteReader();
            return reader.Read();
        }

        /// <summary>
        /// Checks if <paramref name="shaderLabel"/> declares any color set attributes in the shader code.
        /// Omit the tag when specifying the shader label. Example: "SFX_PBS_0000000000080089".
        /// </summary>
        /// <param name="shaderLabel">The name of the shader without the tag</param>
        /// <returns><c>true</c> if a color set is declared in the shader</returns>
        public static bool HasColorSets(string shaderLabel)
        {
            using var connection = new SqliteConnection("Data Source=SmushAttributes.db");
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText =
                @"
                    SELECT * FROM ShaderAttributes
                    WHERE 
	                    ShaderLabel = $shaderLabel AND 
	                    ShaderLabel IN 
	                    (
		                    SELECT ShaderLabel
		                    FROM ShaderAttributes 
		                    WHERE 
			                    Attribute = 'in_attr8' OR 
			                    Attribute = 'in_attr9' OR
			                    Attribute = 'in_attr10'
	                    )
                ";
            command.Parameters.AddWithValue("$shaderLabel", shaderLabel);

            // Read() returns true if there are any results.
            using var reader = command.ExecuteReader();
            return reader.Read();
        }


        /// <summary>
        /// Checks if <paramref name="shaderLabel"/> is a valid shader name.
        /// </summary>
        /// <param name="shaderLabel">The name of the shader, including the tag</param>
        /// <returns><c>true</c> if a shader exists with the name <paramref name="shaderLabel"/></returns>
        public static bool IsValidShaderLabel(string shaderLabel)
        {
            using var connection = new SqliteConnection("Data Source=SmushAttributes.db");
            connection.Open();

            // The current database only contains correlations 
            // between shaders and mesh attributes based on material assignments.
            // TODO: Use a dump of decompiled shaders to find the actual attributes?
            var command = connection.CreateCommand();
            command.CommandText =
                @"
                    SELECT *
                    FROM ShaderMeshAttributes
                    WHERE 
                        ShaderLabel = $shaderLabel
                ";
            command.Parameters.AddWithValue("$shaderLabel", shaderLabel);

            // Read() returns true if there are any results.
            using var reader = command.ExecuteReader();
            return reader.Read();
        }

        /// <summary>
        /// Checks if <paramref name="expectedAttributes"/> has exactly the same set of attributes
        /// as <paramref name="shaderLabel"/>, ignoring the order of elements.
        /// </summary>
        /// <param name="shaderLabel">The name of the shader, including the tag</param>
        /// <param name="expectedAttributes">The attribute list to check for the shader</param>
        /// <returns><c>true</c> if the attribute lists match</returns>
        public static bool IsValidAttributeList(string shaderLabel, ICollection<string> expectedAttributes)
        {
            using var connection = new SqliteConnection("Data Source=SmushAttributes.db");
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText =
                @"
                    SELECT MeshAttribute
                    FROM ShaderMeshAttributes
                    WHERE 
                        ShaderLabel = $shaderLabel
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

            // Check that the two lists have the same elements, regardless of order.
            return (actualAttributes.Count == expectedAttributes.Count)
                && !actualAttributes.Except(expectedAttributes).Any();
        }
    }
}
