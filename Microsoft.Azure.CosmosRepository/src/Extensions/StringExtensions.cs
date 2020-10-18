// Copyright © IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System.Globalization;

namespace Microsoft.Azure.CosmosRepository.Extensions
{
	/// <summary>
	/// The string extension methods class.
	/// </summary>
	public static class StringExtensions
	{
		/// <summary>
		/// Returns the instance as a formatted string using the specified parameters.
		/// </summary>
		/// <param name="instance">The format string.</param>
		/// <param name="parameters">The parameters.</param>
		/// <returns>The formatted string.</returns>
		public static string Format(this string instance, params object[] parameters) =>
			string.Format(CultureInfo.CurrentCulture, instance, parameters);
	}
}
