import { Controller } from "react-hook-form";
import styled from "styled-components";

import Input from "components/input/Input";
import MultiSelect from "components/multiSelect/MultiSelect";
import ErrorMessage from "../rules/ErrorMessage";

// eslint-disable-next-line @typescript-eslint/no-explicit-any
function GroupForm({ control, options, errorMessage }: any) {
  return (
    <form>
      <Field>
        <Controller
          name="groupName"
          control={control}
          render={({ field: { ...rest } }) => <Input {...rest} />}
          rules={{ required: "field is required" }}
        />
        {errorMessage && <ErrorMessage>{errorMessage}</ErrorMessage>}
      </Field>
      <Field>
        <Controller
          name="status"
          control={control}
          render={({ field: { ref, ...rest } }) => (
            <MultiSelect
              {...rest}
              value={[rest.value]}
              onChange={([value]) => rest.onChange(value)}
              options={options}
            />
          )}
        />
      </Field>
    </form>
  );
}

export default GroupForm;

const Field = styled.div`
  position: relative;
  box-sizing: border-box;
  width: calc(25% - 16px);

  @media (min-width: 1200px) {
    width: calc(20% - 16px);
  }

  @media (min-width: 1600px) {
    width: calc(16.6% - 16px);
  }
`;
