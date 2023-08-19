#region License (GPL v2)
/*
    CookStorage - Allow any item to be added to campfires, ovens, furnaces, etc.
    Copyright (c) 2023 RFC1920 <desolationoutpostpve@gmail.com>

    This program is free software; you can redistribute it and/or
    modify it under the terms of the GNU General Public License v2.0.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program; if not, write to the Free Software
    Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.

    Optionally you can also view the license at <http://www.gnu.org/licenses/>.
*/
#endregion License (GPL v2)
using System;
using Harmony;
//Reference: 0Harmony

namespace Oxide.Plugins
{
    [Info("Cook Storage", "RFC1920", "0.0.1")]
    [Description("Allow non-food items in ovens and furnaces")]
    internal class CookStorage : RustPlugin
    {
        HarmonyInstance _harmony;

        private void OnServerInitialized()
        {
            _harmony = HarmonyInstance.Create(Name + "PATCH");
            Type patchType = AccessTools.Inner(typeof(CookStorage), "GetAllowedSlotsPatch");
            new PatchProcessor(_harmony, patchType, HarmonyMethod.Merge(patchType.GetHarmonyMethods())).Patch();

            Puts($"Applied Patch: {patchType.Name}");
        }

        private void Unload() => _harmony.UnpatchAll(Name + "PATCH");

        [HarmonyPatch(typeof(BaseOven), "GetAllowedSlots")]
        public static class GetAllowedSlotsPatch
        {
            [HarmonyPrefix]
            static bool Prefix(BaseOven __instance, ref Item item, ref BaseOven.MinMax? __result)
            {
                if (!__instance.IsBurnableItem(item) && !__instance.IsOutputItem(item) && !__instance.IsMaterialInput(item) && !__instance.IsMaterialOutput(item))
                {
                    int allowed = __instance.fuelSlots + __instance.inputSlots + __instance.outputSlots;
                    __result = new BaseOven.MinMax?(new BaseOven.MinMax(0, allowed));
                    return false;
                }
                return true;
            }
        }
    }
}
