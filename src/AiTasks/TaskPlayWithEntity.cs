using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Datastructures;
using Vintagestory.GameContent;
using System.Linq;

namespace Cats
{
    public class AiTaskPlayWithEntity : AiTaskSeekEntity
    {

        private long lastCheck;

        private string[] playAnimations;

        private bool finished = false;
        private long finishedAt;
        public AiTaskPlayWithEntity(EntityAgent entity) : base(entity)
        {
        }

        public override void LoadConfig(JsonObject taskConfig, JsonObject aiConfig)
        {
            base.LoadConfig(taskConfig, aiConfig);
            playAnimations = taskConfig["playAnimations"].AsArray<string>(new string[0]);
        }

        public override bool ShouldExecute()
        {
            long ellapsedMs = entity.World.ElapsedMilliseconds;
            if (lastCheck + cooldownUntilMs < ellapsedMs)
            {
                lastCheck = ellapsedMs;
                targetEntity = partitionUtil.GetNearestEntity(entity.ServerPos.XYZ, seekingRange, e => IsTargetableEntity(e, seekingRange));
                if (targetEntity != null)
                {
                    targetPos = targetEntity.ServerPos.XYZ;
                    finished = false;
                    return true;
                }
            }
            return false;
        }

        public override bool ContinueExecute(float dt)
        {
            var elapsedMs = entity.World.ElapsedMilliseconds;
            if (finished)
            {
                if (finishedAt + 1000 < elapsedMs) { return false; }
                else { return true; }
            }
            else if (entity.ServerPos.SquareHorDistanceTo(targetEntity.ServerPos.XYZ) < MinDistanceToTarget() * 2)
            {
                finished = true;
                finishedAt = elapsedMs;
                pathTraverser.Stop();
                if (playAnimations.Length > 0)
                {
                    string playAnimCode = playAnimations[entity.World.Rand.Next(0, playAnimations.Length)];
                    var playAnim = new AnimationMetaData()
                    {
                        Code = playAnimCode.ToLowerInvariant(),
                        Animation = playAnimCode.ToLowerInvariant(),
                        AnimationSpeed = 1f
                    }.Init();
                    entity.AnimManager.StartAnimation(playAnim);
                }
                entity.AnimManager.StopAnimation(animMeta.Code);
                return true;
            }
            return base.ContinueExecute(dt);
        }

        public override bool IsTargetableEntity(Entity e, float range, bool ignoreEntityCode = false)
        {
            if (!e.Alive || e.EntityId == entity.EntityId || !CanSense(e, range)) return false;
            if (ignoreEntityCode) return true;

            if (targetEntityCodesExact.Contains(e.Code.Path)) return true;

            for (int i = 0; i < targetEntityCodesBeginsWith.Length; i++)
            {
                if (e.Code.Path.StartsWith(targetEntityCodesBeginsWith[i])) return true;
            }

            return false;
        }
    }
}