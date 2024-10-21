import { RuleTypes } from "components/ruleEngine/types";
import RuleByType from "components/ruleEngine/components/RuleByType";

function LeadScoringRules() {
  return <RuleByType ruleType={RuleTypes.LEAD_SCORING} />;
}

export default LeadScoringRules;
