import { Actions } from "pages/settings/SettingsHelper";
import { IFeedback } from "pages/settings/items/feedback/types";
import { BaseAction } from "pages/settings/types";
import { Field, FormContent, Form, Row, Wrap } from "pages/settings/items/BaseStyles";
import { Control, Controller } from "react-hook-form";
import Input from "components/input/Input";
import MultiSelect from "components/multiSelect/MultiSelect";
import { useContext } from "react";
import { SettingItemContext } from "../../ContextProvider";

type ComponentProps = BaseAction & {
  control: Control<IFeedback>;
};

const FeedbackSetting = ({ control, ...saveProps }: ComponentProps) => {
  const { statuses } = useContext(SettingItemContext);

  return (
    <Wrap>
      <Form>
        <FormContent>
          <Row>
            <Field>
              <Controller
                name="pageTimeout"
                control={control}
                render={({ field }) => (
                  <Input
                    {...field}
                    value={field.value || ""}
                    label="Feedback page timeout"
                    type="number"
                  />
                )}
              />
            </Field>
            <Field>
              <Controller
                name="defaultStatus"
                control={control}
                render={({ field: { ref, ...rest } }) => {
                  const value = statuses.find(item => item.value === rest.value);

                  return (
                    <MultiSelect
                      {...rest}
                      options={value ? [value] : []}
                      value={value ? [value] : []}
                      onChange={([{ value }]) => rest.onChange(value)}
                      label="Feedback Default Status"
                    />
                  );
                }}
              />
            </Field>
            <Field>
              <Controller
                name="redialsLimit"
                control={control}
                render={({ field }) => (
                  <Input
                    {...field}
                    value={field.value || ""}
                    label="Redials limit (0 - unlimited)"
                    type="number"
                  />
                )}
              />
            </Field>
          </Row>
        </FormContent>
        <Actions {...saveProps} />
      </Form>
    </Wrap>
  );
};

export default FeedbackSetting;
