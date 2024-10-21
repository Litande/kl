import { useEffect } from "react";
import { useFieldArray, useForm } from "react-hook-form";

import useBehavior from "hooks/useBehavior";
import { IForm, RuleTypes } from "components/ruleEngine/types";
import { STATUS_OPTIONS } from "components/ruleEngine/constants";
import ruleConditionsActionsBehavior from "components/ruleEngine/rulesBehavior";
import RuleForm from "components/ruleEngine/components/rules/RuleForm";

interface ICreateRuleProps {
  ruleType: RuleTypes;
  urlParams: {
    groupId?;
    ruleId?;
  };
  onClose: VoidFunction;
}

function CreateRule({ ruleType, urlParams, onClose }: ICreateRuleProps) {
  const { submitRule, getConditionsAndActions } = useBehavior(ruleConditionsActionsBehavior);
  const formProps = useForm<IForm>({
    defaultValues: {
      name: "",
      status: STATUS_OPTIONS[0],
    },
  });
  const conditions = useFieldArray({
    control: formProps.control,
    name: "conditions",
  });
  const actions = useFieldArray({
    control: formProps.control,
    name: "actions",
  });

  useEffect(() => {
    getConditionsAndActions(ruleType);
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  const handleSubmitForm = data => {
    formProps.handleSubmit(submitRule(ruleType, urlParams))(data);
    formProps.formState.isValid && onClose();
  };
  const removeRowHandler = (rowType, index) => {
    const formfields = { conditions, actions };
    formfields[rowType].remove(index);
  };
  const appendRowHandler = rowType => {
    const formfields = { conditions, actions };
    const operators = {
      conditions: "comparisonOperation",
      actions: "actionOperation",
    };
    formfields[rowType].append({
      name: null,
      [operators[rowType]]: null,
      fields: null,
    });
  };

  return (
    <RuleForm
      conditions={conditions}
      actions={actions}
      submitForm={handleSubmitForm}
      onClose={onClose}
      removeRowHandler={removeRowHandler}
      appendRowHandler={appendRowHandler}
      {...formProps}
    />
  );
}

export default CreateRule;
