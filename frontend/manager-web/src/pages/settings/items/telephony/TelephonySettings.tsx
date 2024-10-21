import { ContentWrap } from "pages/settings/items/BaseStyles";
import TelephonyItem from "pages/settings/items/telephony/TelephonyItem";
import { BaseAction, SettingType } from "pages/settings/types";
import { Actions, FormControlProps } from "pages/settings/SettingsHelper";
import { useFieldArray, useWatch } from "react-hook-form";
import styled from "styled-components";
import Button from "components/button/Button";

type ComponentProps = BaseAction & FormControlProps;

const TelephonySettings = ({ control, ...saveProps }: ComponentProps) => {
  const { fields, append, remove } = useFieldArray({
    control,
    name: SettingType.Telephony,
  });
  const watchFields = useWatch({ control });

  const handleAdd = () => {
    append({ phoneNumber: "", country: [] });
  };

  const hasEmptyFields = () => {
    const filtered = watchFields[SettingType.Telephony]?.filter(
      ({ country, phoneNumber }) => !(country.length && phoneNumber)
    );

    return Boolean(filtered?.length);
  };

  return (
    <div>
      <StyledContentWrap>
        {fields.map((field, index) => {
          return (
            <TelephonyItem
              onRemove={() => remove(index)}
              control={control}
              key={field.id}
              index={index}
            />
          );
        })}
      </StyledContentWrap>
      <ActionsWrap>
        <Button onClick={handleAdd}>Add New Phone</Button>
        {saveProps.areActionsAvailable && (
          <Actions {...saveProps} areActionsAvailable={!hasEmptyFields()} />
        )}
      </ActionsWrap>
    </div>
  );
};

const StyledContentWrap = styled(ContentWrap)`
  width: 100%;
  padding: 0 1rem;
  box-sizing: border-box;
  max-height: 350px;
  overflow: auto;
`;

const ActionsWrap = styled.div`
  display: flex;
  justify-content: space-between;
  padding: 1rem;
`;

export default TelephonySettings;
