using System;
using System.Collections.Generic;
using System.Reflection;
using Verse;

namespace RimWorld
{
	public static class Detours
	{
		private static List<string> detoured = new List<string>();

		private static List<string> destinations = new List<string>();

		public unsafe static bool TryDetourFromTo(MethodInfo source, MethodInfo destination)
		{
			if ((object)source == null)
			{
				Log.Error("Source MethodInfo is null: Detours");
				return false;
			}
			if ((object)destination == null)
			{
				Log.Error("Destination MethodInfo is null: Detours");
				return false;
			}
			string item = source.DeclaringType.FullName + "." + source.Name + " @ 0x" + source.MethodHandle.GetFunctionPointer().ToString("X" + IntPtr.Size * 2);
			string item2 = destination.DeclaringType.FullName + "." + destination.Name + " @ 0x" + destination.MethodHandle.GetFunctionPointer().ToString("X" + IntPtr.Size * 2);
			detoured.Add(item);
			destinations.Add(item2);
			if (IntPtr.Size == 8)
			{
				long num = source.MethodHandle.GetFunctionPointer().ToInt64();
				long num2 = destination.MethodHandle.GetFunctionPointer().ToInt64();
				byte* ptr = (byte*)num;
				long* ptr2 = (long*)(ptr + 2);
				*ptr = 72;
				ptr[1] = 184;
				*ptr2 = num2;
				ptr[10] = byte.MaxValue;
				ptr[11] = 224;
			}
			else
			{
				int num3 = source.MethodHandle.GetFunctionPointer().ToInt32();
				int num4 = destination.MethodHandle.GetFunctionPointer().ToInt32();
				byte* ptr3 = (byte*)num3;
				int* ptr4 = (int*)(ptr3 + 1);
				int num5 = num4 - num3 - 5;
				*ptr3 = 233;
				*ptr4 = num5;
			}
			return true;
		}
	}
}
