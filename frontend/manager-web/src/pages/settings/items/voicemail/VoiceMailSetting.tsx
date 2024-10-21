import { Field, Form, FormContent, Row, Wrap } from "pages/settings/items/BaseStyles";
import { Actions } from "pages/settings/SettingsHelper";
import { BaseAction } from "pages/settings/types";
import { Control, Controller } from "react-hook-form";
import MultiSelect from "components/multiSelect/MultiSelect";
import { IVoicemail } from "./types";
import Input from "components/input/Input";
import { useContext } from "react";
import { SettingItemContext } from "pages/settings/ContextProvider";

type ComponentProps = BaseAction & {
  control: Control<IVoicemail>;
};

const VoiceMailSetting = ({ control, ...saveProps }: ComponentProps) => {
  const { statuses } = useContext(SettingItemContext);

  return (
    <Wrap>
      <Form>
        <FormContent>
          <Row>
            <Controller
              name="defaultVoicemailStatus"
              control={control}
              render={({ field: { ref, ...rest } }) => {
                const value = statuses.find(item => item.value === rest.value);

                return (
                  <Field>
                    <MultiSelect
                      {...rest}
                      label="Default Voice Mail Status"
                      value={value ? [value] : []}
                      onChange={([{ value }]) => rest.onChange(value)}
                      options={statuses}
                    />
                  </Field>
                );
              }}
            />
            <Controller
              name="hideVoicemailButtonAfterThisAmountOfSecondsOfCall"
              control={control}
              render={({ field }) => (
                <Field>
                  <Input
                    {...field}
                    value={field.value || ""}
                    label="Hide VoiceMail button after this amount of seconds of call"
                  />
                </Field>
              )}
            />
            <Controller
              name="showVoicemailButtonAfterThisAmountOfSecondsOfCall"
              control={control}
              render={({ field }) => (
                <Field>
                  <Input
                    {...field}
                    value={field.value || ""}
                    label="Show VoiceMail button after this amount of seconds of call"
                  />
                </Field>
              )}
            />
          </Row>
        </FormContent>
        <Actions {...saveProps} />
      </Form>
    </Wrap>
  );
};

export default VoiceMailSetting;
