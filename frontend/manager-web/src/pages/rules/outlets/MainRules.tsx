import { RuleTypes } from "components/ruleEngine/types";
import RuleByType from "components/ruleEngine/components/RuleByType";

function MainRules() {
  return <RuleByType ruleType={RuleTypes.MAIN} />;
}

export default MainRules;
