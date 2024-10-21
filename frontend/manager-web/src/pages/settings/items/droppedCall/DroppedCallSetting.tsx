import { Field, Form, FormContent, Row, Wrap } from "pages/settings/items/BaseStyles";
import { BaseAction } from "pages/settings/types";
import { IDroppedCall } from "pages/settings/items/droppedCall/types";
import { Actions } from "pages/settings/SettingsHelper";
import { Control, Controller } from "react-hook-form";
import MultiSelect from "components/multiSelect/MultiSelect";
import Input from "components/input/Input";
import { useContext } from "react";
import { SettingItemContext } from "pages/settings/ContextProvider";

type ComponentProps = BaseAction & {
  control: Control<IDroppedCall>;
};

const DroppedCallSetting = ({ control, ...saveProps }: ComponentProps) => {
  const { statuses } = useContext(SettingItemContext);

  return (
    <Wrap>
      <Form>
        <FormContent>
          <Row>
            <Controller
              name="droppedCallStatus"
              control={control}
              render={({ field: { ref, ...rest } }) => {
                const value = statuses.find(item => item.value === rest.value);

                return (
                  <Field>
                    <MultiSelect
                      {...rest}
                      label="Dropped Call Status"
                      value={value ? [value] : []}
                      onChange={([{ value }]) => rest.onChange(value)}
                      options={statuses}
                    />
                  </Field>
                );
              }}
            />

            <Controller
              name="timeoutForSearching"
              control={control}
              render={({ field }) => (
                <Field>
                  <Input
                    {...field}
                    value={field.value || ""}
                    label="Timeout for searching for available agent"
                    type="number"
                  />
                </Field>
              )}
            />
            <Field />
          </Row>
        </FormContent>
        <Actions {...saveProps} />
      </Form>
    </Wrap>
  );
};

export default DroppedCallSetting;
