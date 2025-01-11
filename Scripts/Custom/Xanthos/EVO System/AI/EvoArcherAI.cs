#region AuthorHeader
//
//	EvoSystem version 2.1, by Xanthos
//
//
#endregion AuthorHeader
using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Mobiles;

namespace Xanthos.Evo
{
    class EvoArcherAI : ArcherAI
    {
        private bool m_CanAttackPlayers;

        public EvoArcherAI(BaseCreature m, bool canAttackPlayers) : base(m)
        {
            m_CanAttackPlayers = canAttackPlayers;
        }

        public void EndPickTarget(Mobile from, IDamageable damageableTarget, OrderType order)
        {
            Mobile oldTarget = m_Mobile.ControlTarget as Mobile; // Cast to Mobile if possible
            OrderType oldOrder = order;

            if (damageableTarget is Mobile target) // Check if the damageableTarget is a Mobile
            {
                if (OrderType.Attack == order && target is PlayerMobile && !m_CanAttackPlayers)
                {
                    // Not allowed to attack players so reset what was changed by EndPickTarget
                    m_Mobile.ControlTarget = oldTarget;
                    m_Mobile.ControlOrder = oldOrder;
                }
            }
        }

        public override bool DoOrderGuard()
        {
            if (m_Mobile.IsDeadPet)
                return true;

            Mobile controlMaster = m_Mobile.ControlMaster;

            if (controlMaster == null || controlMaster.Deleted)
                return true;

            IDamageable combatant = m_Mobile.Combatant; 

            List<AggressorInfo> aggressors = controlMaster.Aggressors;

            if (aggressors.Count > 0)
            {
                for (int i = 0; i < aggressors.Count; ++i)
                {
                    AggressorInfo info = aggressors[i];
                    Mobile attacker = info.Attacker;

                    if (attacker != null && !attacker.Deleted && attacker.GetDistanceToSqrt(m_Mobile) <= m_Mobile.RangePerception)
                    {
                        double combatantDistance = combatant is Mobile ? ((Mobile)combatant).GetDistanceToSqrt(controlMaster) : double.MaxValue;
                        double attackerDistance = attacker.GetDistanceToSqrt(controlMaster);

                        if (combatant == null || attackerDistance < combatantDistance)
                        {
                            if ((attacker is PlayerMobile && m_CanAttackPlayers) || !(attacker is PlayerMobile))
                                combatant = attacker;
                        }
                    }
                }

                if (combatant is Mobile mobileCombatant)
                {
                    if (!mobileCombatant.Deleted)
                        m_Mobile.DebugSay("Crap, my master has been attacked! I will attack one of those bastards!");
                }
            }

            if (combatant != null)
            {
                if (combatant is Mobile mobileCombatant)
                {
                    if (mobileCombatant != m_Mobile && mobileCombatant != controlMaster && !mobileCombatant.Deleted && mobileCombatant.Alive && !mobileCombatant.IsDeadBondedPet && m_Mobile.CanSee(mobileCombatant) && m_Mobile.CanBeHarmful(mobileCombatant, false) && mobileCombatant.Map == m_Mobile.Map)
                    {
                        m_Mobile.DebugSay("Guarding from target...");

                        m_Mobile.Combatant = combatant; // This is fine since we're setting IDamageable back to Combatant
                        m_Mobile.FocusMob = mobileCombatant;
                        Action = ActionType.Combat;

                        // Check if Think() expects Mobile or IDamageable
                        Think(); // Assuming Think() can handle IDamageable or has been adjusted
                    }
                }
                else
                {
                    // Handle cases where combatant is not a Mobile but another type of IDamageable
                    m_Mobile.DebugSay("Guarding from non-mobile damageable entity...");
                    m_Mobile.Combatant = combatant;
                    Action = ActionType.Combat;
                    // Note: If Think() expects only Mobile, you might need to skip calling it here or adjust it
                    // Think(); // Commented out if it doesn't handle non-Mobile combatants
                }
            }
            else
            {
                m_Mobile.DebugSay("Nothing to guard from");

                m_Mobile.Warmode = false;

                WalkMobileRange(controlMaster, 1, false, 0, 1);
            }

            return true;
        }
    }
}