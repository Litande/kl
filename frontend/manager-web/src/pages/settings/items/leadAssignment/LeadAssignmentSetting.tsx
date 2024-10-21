import Input from "components/input/Input";
import { Actions } from "pages/settings/SettingsHelper";
import { Field, FormContent, Form, Row, Wrap } from "pages/settings/items/BaseStyles";
import { BaseAction } from "pages/settings/types";
import { Control, Controller } from "react-hook-form";

type ComponentProps = BaseAction & {
  control: Control<ILeadAssignment>;
};

const LeadAssignmentSetting = ({ control, ...saveProps }: ComponentProps) => {
  return (
    <Wrap>
      <Form>
        <FormContent>
          <Row>
            <Field>
              <Controller
                name="permanentAssignExpirationDaysIsAgentOffline"
                control={control}
                render={({ field }) => (
                  <Input
                    {...field}
                    value={field.value || ""}
                    label="Permanent assignment expiration days if agent is offline"
                    type="number"
                  />
                )}
              />
            </Field>
            <Field />
          </Row>
        </FormContent>
        <Actions {...saveProps} />
      </Form>
    </Wrap>
  );
};

export default LeadAssignmentSetting;
