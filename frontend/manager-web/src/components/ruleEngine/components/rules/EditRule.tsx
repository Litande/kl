import { useEffect } from "react";
import { useFieldArray, useForm } from "react-hook-form";
import { useSelector } from "react-redux";

import useBehavior from "hooks/useBehavior";
import { ruleDetailsSelector, ruleLoadingStateSelector } from "components/ruleEngine/rulesSelector";
import { IForm, RuleTypes } from "components/ruleEngine/types";
import ruleConditionsActionsBehavior from "components/ruleEngine/rulesBehavior";
import RuleForm from "components/ruleEngine/components/rules/RuleForm";

interface IEditRuleProps {
  ruleType: RuleTypes;
  urlParams: {
    groupId?;
    ruleId?;
  };
  onClose: VoidFunction;
  ruleDetails?: IForm;
}

const EditRule = ({ ruleType, urlParams, onClose }: IEditRuleProps) => {
  const { getRuleData } = useBehavior(ruleConditionsActionsBehavior);
  const fetchedRuleDetails = useSelector(ruleDetailsSelector);
  const isLoading = useSelector(ruleLoadingStateSelector);

  useEffect(() => {
    getRuleData({ ruleType: ruleType, ...urlParams });
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  if (isLoading) return <span>loading...</span>;
  return (
    <EditRuleForm
      ruleType={ruleType}
      urlParams={urlParams}
      onClose={onClose}
      ruleDetails={fetchedRuleDetails}
    />
  );
};

export function EditRuleForm({ ruleType, urlParams, onClose, ruleDetails }: IEditRuleProps) {
  const { submitRule } = useBehavior(ruleConditionsActionsBehavior);
  const formProps = useForm<IForm>({
    defaultValues: ruleDetails,
  });
  const conditions = useFieldArray({
    control: formProps.control,
    name: "conditions",
  });
  const actions = useFieldArray({
    control: formProps.control,
    name: "actions",
  });

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

export default EditRule;
