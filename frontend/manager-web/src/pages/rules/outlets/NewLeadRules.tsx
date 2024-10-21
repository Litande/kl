import { RuleTypes } from "components/ruleEngine/types";
import RuleByType from "components/ruleEngine/components/RuleByType";

function NewLeadRules() {
  return <RuleByType ruleType={RuleTypes.NEW_LEADS} />;
}

export default NewLeadRules;
