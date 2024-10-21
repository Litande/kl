import Input from "components/input/Input";
import { Actions } from "pages/settings/SettingsHelper";
import { Field, Form, FormContent, Row, Wrap } from "pages/settings/items/BaseStyles";
import { IProductive } from "pages/settings/items/productiveSettings/types";
import { BaseAction } from "pages/settings/types";
import { Control, Controller } from "react-hook-form";
import MultiSelect from "components/multiSelect/MultiSelect";
import { useContext } from "react";
import { SettingItemContext } from "pages/settings/ContextProvider";

type ComponentProps = BaseAction & {
  control: Control<IProductive>;
};

const ProductiveSetting = ({ control, ...saveProps }: ComponentProps) => {
  const { statuses } = useContext(SettingItemContext);

  return (
    <Wrap>
      <Form>
        <FormContent>
          <Row>
            <Controller
              name="endCallButtonAfterThisAmountOfSeconds"
              control={control}
              render={({ field }) => (
                <Field>
                  <Input
                    {...field}
                    value={field.value || ""}
                    label="Show End Call button after this amount of seconds of call"
                    type="number"
                  />
                </Field>
              )}
            />
            <Controller
              name="ringingTimeout"
              control={control}
              render={({ field }) => (
                <Field>
                  <Input
                    {...field}
                    value={field.value || ""}
                    label="Max Allowed Ringing Duration"
                    type="number"
                  />
                </Field>
              )}
            />
            <Controller
              name="maxCallDuration"
              control={control}
              render={({ field }) => (
                <Field>
                  <Input
                    {...field}
                    value={field.value || ""}
                    label="Call Origination Timeout"
                    type="number"
                  />
                </Field>
              )}
            />
          </Row>
          <Row>
            <Controller
              name="defaultBusyStatus"
              control={control}
              render={({ field: { ref, ...rest } }) => {
                const value = statuses.find(item => item.value === rest.value);

                return (
                  <Field>
                    <MultiSelect
                      {...rest}
                      label="Default Busy Status"
                      value={value ? [value] : []}
                      onChange={([{ value }]) => rest.onChange(value)}
                      options={statuses}
                    />
                  </Field>
                );
              }}
            />
            <Field />
            <Field />
          </Row>
        </FormContent>
        <Actions {...saveProps} />
      </Form>
    </Wrap>
  );
};

export default ProductiveSetting;
