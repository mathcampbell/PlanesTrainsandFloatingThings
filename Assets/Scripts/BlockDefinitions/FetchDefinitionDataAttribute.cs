using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlockDefinitions
{

	/// <summary>
	/// An attribute that specifies that this field or property in a <see cref="BlockDefinition"/> should be loaded from a resource.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
	internal class FetchDefinitionDataAttribute : Attribute
	{
		/// <summary>
		/// The name of the field that contains the path to the resource to load.
		/// </summary>
		internal string nameOfFieldWithResourcePath;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="nameOfFieldWithResourcePath">The name of the field that contains the path to the resource to load.
		/// Use <see cref="nameof()"/> to get it, so that it will survive a rename of the field.</param>
		public FetchDefinitionDataAttribute(string nameOfFieldWithResourcePath)
		{
			this.nameOfFieldWithResourcePath = nameOfFieldWithResourcePath
			                                ?? throw new ArgumentNullException(nameof(nameOfFieldWithResourcePath));
		}
	}
}