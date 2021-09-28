using System;
using System.Linq;
using System.Reflection;
using Verse;

namespace RimWorld
{
	[StaticConstructorOnStartup]
	internal static class DetourInjector
	{
		private const BindingFlags UniversalBindingFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

		private static Assembly Assembly => Assembly.GetAssembly(typeof(DetourInjector));

		private static string AssemblyName => Assembly.FullName.Split(',').First();

		private static bool DoInject()
		{
			return DoDetour(typeof(WorkGiver_HunterHunt), typeof(Overwrite), "HasShieldAndRangedWeapon");
		}

		static DetourInjector()
		{
			LongEventHandler.QueueLongEvent((Action)Inject, "Initializing", true, (Action<Exception>)null);
		}

		private static void Inject()
		{
			if (DoInject())
			{
				Log.Message(AssemblyName + " injected.");
			}
			else
			{
				Log.Error(AssemblyName + " failed to get injected properly.");
			}
		}

		private static bool DoDetour(Type rimworld, Type mod, string method)
		{
			MethodInfo method2 = rimworld.GetMethod(method, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			MethodInfo method3 = mod.GetMethod(method, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			return Detours.TryDetourFromTo(method2, method3);
		}
	}
}
