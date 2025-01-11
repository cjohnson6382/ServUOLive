#region AuthorHeader
//
//	EvoSystem version 2.1, by Xanthos
//
#endregion AuthorHeader

using System;
using System.Collections.Generic;
using Server;
using Server.Mobiles;

namespace Xanthos.Evo
{
    class EvoMageAI : MageAI
    {
        private bool m_CanAttackPlayers;

        public EvoMageAI(BaseCreature m, bool canAttackPlayers) : base(m)
        {
            m_CanAttackPlayers = canAttackPlayers;
        }

        public void EndPickTarget(Mobile from, IDamageable damageableTarget, OrderType order)
        {
            Mobile oldTarget = m_Mobile.ControlTarget as Mobile; 
            OrderType oldOrder = order;

            if (damageableTarget is Mobile target)
            {
                if (order == OrderType.Attack && target is PlayerMobile && !m_CanAttackPlayers)
                {
                    // Not allowed to attack players, so reset what was changed by our logic
                    m_Mobile.ControlTarget = oldTarget;
                    m_Mobile.ControlOrder = oldOrder;
                    return; // Exit early to avoid unnecessary operations
                }
            }
            else
            {
                // If the target is not a Mobile, we can't proceed with attack logic
                m_Mobile.ControlTarget = oldTarget;
                m_Mobile.ControlOrder = oldOrder;
                return;
            }
        }

        public override bool DoOrderGuard()
        {
            if (m_Mobile.IsDeadPet)
                return true;

            Mobile controlMaster = m_Mobile.ControlMaster;

            if (controlMaster == null || controlMaster.Deleted)
                return true;

            List<AggressorInfo> aggressors = controlMaster.Aggressors;

            if (aggressors.Count > 0)
            {
                IDamageable combatant = m_Mobile.Combatant;
                double combatantDistance = combatant is Mobile ? ((Mobile)combatant).GetDistanceToSqrt(controlMaster) : double.MaxValue;

                foreach (AggressorInfo info in aggressors)
                {
                    Mobile attacker = info.Attacker;

                    if (attacker != null && !attacker.Deleted && attacker.GetDistanceToSqrt(m_Mobile) <= m_Mobile.RangePerception)
                    {
                        double attackerDistance = attacker.GetDistanceToSqrt(controlMaster);

                        if (combatant == null || attackerDistance < combatantDistance)
                        {
                            if ((attacker is PlayerMobile && m_CanAttackPlayers) || !(attacker is PlayerMobile))
                            {
                                combatant = attacker;
                                m_Mobile.DebugSay("Crap, my master has been attacked! I will attack one of those bastards!");
                            }
                        }
                    }
                }

                if (combatant is Mobile mobileCombatant && !mobileCombatant.Deleted)
                {
                    m_Mobile.Combatant = mobileCombatant;
                    m_Mobile.FocusMob = mobileCombatant;
                    Action = ActionType.Combat;

                    // Call Think() to ensure spell casting monsters use spells when guarding
                    Think();
                    return true;
                }
            }

            // No valid combatant found, revert to stay behavior
            m_Mobile.DebugSay("Nothing to guard from");
            m_Mobile.Warmode = false;
            WalkMobileRange(controlMaster, 1, false, 0, 1);

            return true;
        }

        // Add more error handling or null checks in other methods if necessary
    }
}