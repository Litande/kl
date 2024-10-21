export interface IRulesGroup {
  groupId: number;
  groupName: string;
  rules: Array<IRule>;
  status: Status;
}

export interface IRule {
  ruleId: string;
  ruleName: string;
  ruleData: IRuleData["rules"];
  status: Status;
}

export enum Status {
  ACTIVE = "Enable",
  DISABLE = "Disable",
}

export enum RuleTypes {
  NEW_LEADS = "newLeads",
  MAIN = "main",
  LEAD_SCORING = "leadScoring",
  API_RULES = "apiRules",
}

export type ChangedRuleData = {
  ruleType: RuleTypes;
  groupId: string;
  data: {
    name: string;
    status: Status;
  };
};

export type RuleData = {
  ruleType: RuleTypes;
  data: {
    name: string;
    status: Status;
  };
};

// { name?: string; status?: Status; rules?: IRuleData }
type ConditionOperators =
  | "Is"
  | "IsNot"
  | "Equal"
  | "NotEqual"
  | "MoreThan"
  | "LessThan"
  | "EqualForLastYHours"
  | "EqualForLastYDays"
  | "NotEqualForLastYHours"
  | "NotEqualForLastYDays"
  | "MoreThanForLastYHours"
  | "MoreThanForLastYDays"
  | "LessThanForLastYHours"
  | "LessThanForLastYDays";

export type Field = {
  dimension: string;
  isRequired: boolean;
  type: "integer" | "string";
  values: Array<string>;
};

export interface IRuleData {
  name?: string;
  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  status?: any;
  rules?: {
    combination: {
      operator: string;
      groups: Array<{
        name: string;
        comparisonOperation: ConditionOperators;
        fields: null | Array<{
          type: string;
          value: string;
        }>;
      }>;
      combination: [];
    };
    actions: Array<{
      name: string;
      actionOperation: string;
      fields: null | Array<{
        type: string;
        value: string;
      }>;
    }>;
  };
}

export type RulesState = {
  conditions: Array<{
    displayName: string;
    name: string;
    comparisonOperation?: Array<{
      label: string;
      value: ConditionOperators;
    }> | null;
    fields?: null | Array<{
      fieldId: number;
      isRequired: boolean;
      dimension: string;
      type: string;
      values: Array<{ label: string; value: string }>;
    }>;
  }>;
  actions: Array<{
    name: string;
    displayName: string;
    actionOperation?: Array<{ label: string; value: string }> | null;
    fields?: null | Array<{
      type: string;
      fieldId: number;
      isRequired: boolean;
      dimension: string;
      values: Array<{ label: string; value: string }>;
    }>;
  }>;
  fetchedRuleData: IRuleData;
  ruleData: IRuleData;
  loading: boolean;
};

export interface IForm {
  name: string;
  status: {
    label: string;
    value: string;
  };
  conditions?: Array<Condition>;
  actions?: Array<Action>;
}

export type Condition = {
  name: { label: string; value: string };
  comparisonOperation?: { label: string; value: ConditionOperators } | null;
  fields?: null | Array<{
    type: string;
    value: { label: string; value: string } | Array<{ label: string; value: string }>;
  }>;
};

export type Action = {
  name: { label: string; value: string };
  actionOperation?: { label: string; value: string } | null;
  fields?: null | Array<{
    type: string;
    value: { label: string; value: string } | Array<{ label: string; value: string }>;
  }>;
};
