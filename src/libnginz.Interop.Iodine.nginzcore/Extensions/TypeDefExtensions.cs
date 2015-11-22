using System;
using Iodine.Runtime;

namespace nginz.Interop.Iodine.nginzcore
{
	public static class TypeDefExtensions
	{
		public static void AutoimplementEnum<TEnum> (this IodineTypeDefinition typeDef)
			where TEnum : struct, IComparable, IConvertible, IFormattable {

			var names = Enum.GetNames (typeof (TEnum));
			var values = (TEnum[]) Enum.GetValues (typeof (TEnum));
			for (var i = 0; i < names.Length; i++) {
				var elem = Convert.ToInt64 (values [i]);
				typeDef.SetAttribute (names [i], new IodineInteger (elem));
			}
		}
	}
}

