
using System;
using System.Collections.Generic;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;
using Vintagestory.GameContent;

namespace Cats
{
    public class AiTaskFreezeNear : AiTaskBase
    {
        private EntityAgent targetEntity;
        private List<string> entityCodes = new List<string>();
        EntityPartitioning partitionUtil;
        private float seekingRange;
        private long lastSearchTotalMs;

        public AiTaskFreezeNear(EntityAgent entity) : base(entity)
        {
        }
        public override void LoadConfig(JsonObject taskConfig, JsonObject aiConfig)
        {
            base.LoadConfig(taskConfig, aiConfig);
            seekingRange = taskConfig["seekingRange"].AsFloat(8);
            entityCodes.AddRange(taskConfig["entityCodes"].AsArray<string>(new string[0]));
            partitionUtil = entity.Api.ModLoader.GetModSystem<EntityPartitioning>();
        }
        public override bool ShouldExecute()
        {
            if (lastSearchTotalMs + 3000 > entity.World.ElapsedMilliseconds) return false;
            lastSearchTotalMs = entity.World.ElapsedMilliseconds;
            targetEntity = partitionUtil.GetNearestInteractableEntity(entity.ServerPos.XYZ, seekingRange, target => entityCodes.Find(code => code.EndsWith("*") ? target.Code.Path.StartsWith(code.Substring(0, code.Length - 1)) : code == target.Code.Path) != null) as EntityAgent;
            if (targetEntity == null)
            {
                return false;
            }
            return true;
        }

        public override bool ContinueExecute(float dt)
        {
            if (targetEntity == null) return false;
            if (!targetEntity.Alive) return false;

            Vec3f targetVec = new Vec3f(
                (float)(targetEntity.ServerPos.X - entity.ServerPos.X),
                (float)(targetEntity.ServerPos.Y - entity.ServerPos.Y),
                (float)(targetEntity.ServerPos.Z - entity.ServerPos.Z)
            );

            float desiredYaw = (float)Math.Atan2(targetVec.X, targetVec.Z);

            float yawDist = GameMath.AngleRadDistance(entity.ServerPos.Yaw, desiredYaw);
            entity.ServerPos.Yaw += GameMath.Clamp(yawDist, -250 * dt, 250 * dt);
            entity.ServerPos.Yaw = entity.ServerPos.Yaw % GameMath.TWOPI;

            return entity.ServerPos.SquareDistanceTo(targetEntity.ServerPos) < seekingRange * seekingRange;
        }
    }
}