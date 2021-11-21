
using Vintagestory.API.Common;
using Vintagestory.GameContent;

namespace Cats {
    public class Cats : ModSystem {
        public override void Start(ICoreAPI api)
        {
            base.Start(api);

            AiTaskRegistry.Register<AiTaskFreezeNear>("freezeNear");
        }
    }
}