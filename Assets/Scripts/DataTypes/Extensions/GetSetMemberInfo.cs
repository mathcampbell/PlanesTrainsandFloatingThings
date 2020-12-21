using System;
using System.Reflection;

namespace DataTypes.Extensions
{
	/// <summary>
	/// A wrapper for <see cref="MemberInfo"/> that allows getting and setting the value of the underlying field or property.
	/// </summary>
	public class GetSetMemberInfo
	{
		private readonly FieldInfo field;
		private readonly PropertyInfo property;
		private readonly MethodInfo getter;
		private readonly MethodInfo setter;

		private readonly bool isField;

		/// <summary>
		/// The name of the member.
		/// </summary>
		public readonly string Name;

		/// <summary>
		/// The defined <see cref="Type"/> of the value held by the member.
		/// </summary>
		public readonly Type MemberDataType;

		/// <summary>
		///
		/// </summary>
		/// <param name="member">The <see cref="MemberInfo"/> instance to wrap</param>
		public GetSetMemberInfo(MemberInfo member)
		{
			field = member as FieldInfo;
			property = member as PropertyInfo;

			isField = field != null;

			if(null == field && null == property) throw new ArgumentException("Argument was neither an field nor a property.");

			Name = member.Name;

			if (isField)
			{
				MemberDataType = field.FieldType;
			}
			else
			{
				MemberDataType = property.PropertyType;
				getter = property.GetGetMethod(true)
				      ?? property.DeclaringType.GetProperty(property.Name).GetGetMethod(true);
				setter = property.GetSetMethod(true)
				      ?? property.DeclaringType.GetProperty(property.Name).GetSetMethod(true);
			}
		}

		/// <summary>
		/// Set the value of the member on <paramref name="instance"/> to <paramref name="value"/>
		/// </summary>
		/// <param name="instance">The instance to operate on</param>
		/// <param name="value">The value to set</param>
		public void SetValue(object instance, object value)
		{
			if (isField)
			{
				field.SetValue(instance, value);
			}
			else
			{
				setter.Invoke(instance, new[] { value });
			}
		}

		/// <summary>
		/// Get the value of the member on <paramref name="instance"/> and cast it to <typeparamref name="TData"/>
		/// </summary>
		/// <typeparam name="TData">The type of the data</typeparam>
		/// <param name="instance">the instance to retrieve from</param>
		/// <returns>The value of the member on <paramref name="instance"/></returns>
		/// <exception cref="InvalidCastException">if data was null or <typeparamref name="TData"/>was wrong</exception>
		public TData GetValueStrict<TData>(object instance)
		{
			if (isField)
			{
				return (TData) field.GetValue(instance);
			}
			else
			{
				return (TData) getter.Invoke(instance, null);
			}
		}

		/// <summary>
		/// Get the value of the member on <paramref name="instance"/>
		/// </summary>
		/// <typeparam name="TData">The type of the data</typeparam>
		/// <param name="instance">the instance to retrieve from</param>
		/// <returns>The value of the member on <paramref name="instance"/>, or null</returns>
		public TData GetValue<TData>(object instance) where TData : class
		{
			if (isField)
			{
				return field.GetValue(instance) as TData;
			}
			else
			{
				return getter.Invoke(instance, null) as TData;
			}
		}

	}
}
