﻿namespace Ensage.SDK.Orbwalker
{
    using System;
    using System.Collections.Generic;

    using Ensage.SDK.Abilities;
    using Ensage.SDK.Extensions;
    using Ensage.SDK.Helpers;
    using Ensage.SDK.Service;
    using Ensage.SDK.TargetSelector;
    using Ensage.SDK.Threading;

    using SharpDX;

    [ExportTargetSelector("SDK")]
    public class Orbwalker : IOrbwalker
    {
        public float HoldZoneRange;

        public float SafeZone;

        public bool UseAttackModifier;

        private readonly EnsageServiceContext context;

        private List<TargetAbility> attackModifierAbilities = new List<TargetAbility>();

        private EntityOrPosition target;

        public Orbwalker(EnsageServiceContext context)
        {
            this.context = context;
            EnsageDispatcher.Instance.OnIngameUpdate += this.Instance_OnIngameUpdate;
        }

        public event EventHandler<EventArgs> Attacked;

        public event EventHandler<EventArgs> Attacking;

       
        public void Dispose()
        {
            EnsageDispatcher.Instance.OnIngameUpdate -= this.Instance_OnIngameUpdate;
        }

        public void Orbwalk(Unit target)
        {
            this.target = new EntityOrPosition(target);
        }

        public void Orbwalk(Vector3 position)
        {
            this.target = new EntityOrPosition(position);
        }

        protected virtual void OnAttacked()
        {
            this.Attacked?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnAttacking()
        {
            this.Attacking?.Invoke(this, EventArgs.Empty);
        }

        private void Instance_OnIngameUpdate(EventArgs args)
        {
            if (this.target == null)
            {
                return;
            }

            var targetEntity = this.target.Entity as Unit;
            if (targetEntity != null && !targetEntity.IsValid)
            {
                this.target = null;
                return;
            }

            var contextOwner = this.context.Owner;

            float distance;
            if (targetEntity != null)
            {
                var animation = contextOwner.Animation;
                distance = contextOwner.Distance2D(targetEntity);
                if (animation.IsAttackAnimation())
                {
                    var attackPoint = contextOwner.AttackPoint();
                    var animationTime = animation.SequenceUpdateTime - animation.SequenceStartTime;
                    if (animationTime < attackPoint)
                    {
                        // attack not done / in backswing animation yet
                        return;
                    }
                    this.OnAttacked();
                }
                else
                {

                    if (distance <= contextOwner.AttackRange())
                    {
                        this.OnAttacking();
                        contextOwner.Attack(targetEntity);
                        return;
                    }
                }
                if (distance < this.SafeZone)
                {

                }
            }
            else
            {
                distance = contextOwner.Distance2D(this.target.Position);
            }

            var mousePos = Game.MousePosition;
            var distanceTomouse = mousePos.Distance(contextOwner.NetworkPosition);
            if (distanceTomouse < this.HoldZoneRange)
            {
                // don't move but hold to cancel backswing etc // TODO: better move on current position?
                contextOwner.Hold();
                return;
            }

            

            contextOwner.Move(this.target.Position);
        }
    }
}