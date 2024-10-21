const changeRulesNestedPage = appFeatures => (to: string) => {
  appFeatures.navigate(to);
};

const rulesBehavior = {
  changeRulesNestedPage,
};

export default rulesBehavior;
