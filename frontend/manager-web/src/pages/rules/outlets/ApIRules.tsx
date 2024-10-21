import { RuleTypes } from "components/ruleEngine/types";
import RuleByType from "components/ruleEngine/components/RuleByType";

function ApiRules() {
  return <RuleByType ruleType={RuleTypes.API_RULES} />;
}

export default ApiRules;
