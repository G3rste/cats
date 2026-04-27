
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
        readonly private List<string> entityCodes = [];
        readonly EntityPartitioning partitionUtil;
        readonly private float seekingRange;
        private long lastSearchTotalMs;

        public AiTaskFreezeNear(EntityAgent entity, JsonObject taskConfig, JsonObject aiConfig) : base(entity, taskConfig, aiConfig)
        {
            seekingRange = taskConfig["seekingRange"].AsFloat(8);
            entityCodes.AddRange(taskConfig["entityCodes"].AsArray<string>([]));
            partitionUtil = entity.Api.ModLoader.GetModSystem<EntityPartitioning>();
        }
        public override bool ShouldExecute()
        {
            if (lastSearchTotalMs + 3000 > entity.World.ElapsedMilliseconds) return false;
            lastSearchTotalMs = entity.World.ElapsedMilliseconds;
            targetEntity = partitionUtil.GetNearestInteractableEntity(entity.Pos.XYZ, seekingRange, target => entityCodes.Find(code => code.EndsWith("*") ? target.Code.Path.StartsWith(code[..^1]) : code == target.Code.Path) != null) as EntityAgent;
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

            Vec3f targetVec = new(
                (float)(targetEntity.Pos.X - entity.Pos.X),
                (float)(targetEntity.Pos.Y - entity.Pos.Y),
                (float)(targetEntity.Pos.Z - entity.Pos.Z)
            );

            float desiredYaw = (float)Math.Atan2(targetVec.X, targetVec.Z);

            float yawDist = GameMath.AngleRadDistance(entity.Pos.Yaw, desiredYaw);
            entity.Pos.Yaw += GameMath.Clamp(yawDist, -250 * dt, 250 * dt);
            entity.Pos.Yaw = entity.Pos.Yaw % GameMath.TWOPI;

            return entity.Pos.SquareDistanceTo(targetEntity.Pos) < seekingRange * seekingRange;
        }
    }
}