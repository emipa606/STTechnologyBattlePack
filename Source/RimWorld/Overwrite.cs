using System.Collections.Generic;
using Myth;
using Verse;

namespace RimWorld
{
	public static class Overwrite
	{
		public static bool HasShieldAndRangedWeapon(Pawn p)
		{
			if (p.equipment.Primary != null && !p.equipment.Primary.def.Verbs[0].get_MeleeRange())
			{
				List<Apparel> wornApparel = p.apparel.WornApparel;
				for (int i = 0; i < wornApparel.Count; i++)
				{
					if (wornApparel[i] is ShieldBelt && !(wornApparel[i] is EnergyShield) && !(wornApparel[i] is Thinkingshield))
					{
						return true;
					}
				}
			}
			return false;
		}
	}
}
