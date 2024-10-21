import { IRulesGroup } from "./types";

const regionNames = new Intl.DisplayNames(["en"], { type: "region" });

export const normalizeRules = ruleGroups => {
  return ruleGroups.map(group => {
    const rules = group.rules.map(rule => {
      return {
        ruleId: rule.id,
        ruleName: rule.name,
        ruleData: JSON.parse(rule.rules),
        status: rule.status,
      };
    });

    const normalizedValues: IRulesGroup = {
      groupId: group.id,
      groupName: group.name,
      status: group.status,
      rules,
    };
    return normalizedValues;
  });
};

export const transformFieldsFromApiToForm = (fields, optionFields) => {
  if (!optionFields) return null;
  return fields.map((field, idx) => {
    if (field.type === "selectMultiItem") {
      const type = optionFields[idx].type;
      const value = field.value
        .split(",")
        .map(val => optionFields[idx].values.find(f => f.value === val));
      return { type, value };
    }
    if (["stringSet", "integerSet"].includes(field.type)) {
      const type = optionFields[idx].type;
      const value = field.value.split(",").map(value => ({ label: value, value }));
      return { type, value };
    }
    if (field.type === "select") {
      const type = field.type;
      const value = optionFields[idx].values.find(f => {
        return f.value === field.value;
      });
      return { type, value };
    }
    if (!field.type) {
      const type = optionFields[idx].type;
      const value =
        optionFields[idx].values.find(f => {
          return f.value === field.value;
        }) || field.value;
      return { type, value };
    }
    return field;
  });
};

export const fieldTypeToInputType = (fieldType: string) => {
  const fieldTypeDicrionary = {
    integer: "number",
    string: "text",
    selectMultiItem: {
      isMulti: true,
    },
    stringSet: {
      isMulti: true,
      isMultipleInput: true,
    },
    integerSet: {
      isMulti: true,
      isMultipleInput: true,
    },
    select: {
      isMulti: false,
    },
  };
  return fieldTypeDicrionary[fieldType];
};

export const getCountryNameById = (countryId: string) => {
  return regionNames.of(countryId);
};

export const camelCaseToNormalStringFormat = (string: string) => {
  if (!string) {
    return "";
  }
  return string.replace(/\B([A-Z])\B/g, " $1"); // "CamelCase" => "Camel Case", better to write unit test for this
};
