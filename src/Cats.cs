
using System;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Server;
using Vintagestory.GameContent;

namespace Cats
{
    public class Cats : ModSystem
    {

        private ICoreServerAPI sapi;
        private readonly  string[] mainColors = new string[] { "black", "gray", "brown", "purple" };
        private readonly  string[] secondaryColors = new string[] { "orange", "red", "yellow", "white", "blue", "green" };
        public override void Start(ICoreAPI api)
        {
            base.Start(api);

            sapi = api as ICoreServerAPI;

            AiTaskRegistry.Register<AiTaskFreezeNear>("freezeNear");
            AiTaskRegistry.Register<AiTaskPlayWithEntity>("playWithEntity");

            api.Event.OnEntitySpawn += giveSomeCatsHats;
        }

        private void giveSomeCatsHats(Entity entity)
        {
            if (("cat-male" == entity.Code.Path || "cat-female" == entity.Code.Path) && sapi != null && sapi.World.Rand.NextDouble() < 0.1)
            {
                var slot = new DummySlot(new ItemStack(sapi.World.GetItem(new AssetLocation("cats", String.Format("witchhat-{0}-{1}", mainColors[sapi.World.Rand.Next(0, mainColors.Length)], secondaryColors[sapi.World.Rand.Next(0, secondaryColors.Length)])))));
                slot.TryPutInto(sapi.World, (entity as EntityAgent).GearInventory.GetBestSuitedSlot(slot).slot);
            }
        }
    }
}