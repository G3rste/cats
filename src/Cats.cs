
using System;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Server;
using Vintagestory.GameContent;

namespace Cats
{
    public class Cats : ModSystem
    {
        public override void Start(ICoreAPI api)
        {
            base.Start(api);

            AiTaskRegistry.Register<AiTaskFreezeNear>("freezeNear");
            AiTaskRegistry.Register<AiTaskPlayWithEntity>("playWithEntity");
        }
    }
}