using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.PubSubSystem;
using Kingmaker.RuleSystem.Rules.Damage;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.Mechanics;
using Kingmaker.Utility;

namespace ScalingCantrips
{
  [TypeId("a7f5a54170ec4a0aa88fa1638b16dc61")]
  public class AddCasterStatToDamage  : UnitFactComponentDelegate, IInitiatorRulebookHandler<RuleCalculateDamage>
  {
    public void OnEventAboutToTrigger(RuleCalculateDamage evt)
    {
      MechanicsContext context = evt.Reason.Context;
      if ((context?.SourceAbility) == null)
      {
        return;
      }
      if ((context?.MaybeCaster) == null)
      {
        return;
      }
      if ((!context.SourceAbility.IsSpell && this.SpellsOnly) || context.SourceAbility.Type == AbilityType.Physical)
      {
        return;
      }

      if (!context.SourceAbility.IsCantrip && CantripsOnly)
      {
        return;
      }

      if (statType == 0 || (int)statType > 6)
      {
        return;
      }

      //if this is null somehow something has gone very wrong
      ModifiableValueAttributeStat CasterStat = context.MaybeCaster.Stats.GetAttribute(statType);  

      if (CasterStat == null)
      {
        return;
      }
      foreach (BaseDamage baseDamage in evt.DamageBundle)
      {
        int bonus = CasterStat.Bonus;
        if (!baseDamage.Sneak)
        {
          baseDamage.AddModifier(bonus, base.Fact);
        }
      }
    }

    public void OnEventDidTrigger(RuleCalculateDamage evt) { }

    public StatType statType;

    public bool SpellsOnly = false;

    public bool UseContextBonus;

    public bool CantripsOnly = true;

    public ContextValue Value;
  }
}
