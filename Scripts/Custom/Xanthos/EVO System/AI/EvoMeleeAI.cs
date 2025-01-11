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
    class EvoMeleeAI : MeleeAI
    {
        private bool m_CanAttackPlayers;

        public EvoMeleeAI(BaseCreature m, bool canAttackPlayers) : base(m)
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
                    // Not allowed to attack players so reset what was changed by our logic
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

            IDamageable combatant = m_Mobile.Combatant; // Changed to IDamageable

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

                if (combatant is Mobile && !((Mobile)combatant).Deleted)
                    m_Mobile.DebugSay("Crap, my master has been attacked! I will attack one of those bastards!");
            }

            if (combatant != null && combatant != m_Mobile && (combatant is Mobile ? ((Mobile)combatant) != m_Mobile.ControlMaster : true) && !combatant.Deleted && (combatant is Mobile ? ((Mobile)combatant).Alive : false) && !(combatant is Mobile && ((Mobile)combatant).IsDeadBondedPet) && m_Mobile.CanSee(combatant) && m_Mobile.CanBeHarmful(combatant, false) && combatant.Map == m_Mobile.Map)
            {
                m_Mobile.DebugSay("Guarding from target...");

                m_Mobile.Combatant = combatant;
                if (combatant is Mobile mobileCombatant)
                {
                    m_Mobile.FocusMob = mobileCombatant;
                }
                Action = ActionType.Combat;

                /*
                 * We need to call Think() here or spell casting monsters will not use
                 * spells when guarding because their target is never processed.
                 */
                Think();
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