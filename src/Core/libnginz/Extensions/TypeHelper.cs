﻿using System;
using System.Collections.Concurrent;
using System.Reflection.Emit;

namespace nginz
{
	[CLSCompliant (false)]
	public static class TypeHelper
	{
		public static int SizeOf<T> (T? obj) where T : struct {
			if (obj == null) throw new ArgumentNullException ("obj");
			return SizeOf (typeof (T?));
		}

		public static int SizeOf<T> (T obj) {
			// Analysis disable once CompareNonConstrainedGenericWithNull
			if (obj == null) throw new ArgumentNullException ("obj");
			return SizeOf (obj.GetType ());
		}

		public static int SizeOf (Type t) {
			if (t == null) throw new ArgumentNullException ("t");

			return _cache.GetOrAdd (t, t2 => {
				var dm = new DynamicMethod ("$", typeof (int), Type.EmptyTypes);
				ILGenerator il = dm.GetILGenerator ();
				il.Emit (OpCodes.Sizeof, t2);
				il.Emit (OpCodes.Ret);

				var func = (Func<int>) dm.CreateDelegate (typeof (Func<int>));
				return func ();
			});
		}

		private static readonly ConcurrentDictionary<Type, int>
			_cache = new ConcurrentDictionary<Type, int> ();
	}
}
