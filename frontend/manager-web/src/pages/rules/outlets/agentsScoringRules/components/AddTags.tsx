import { FC } from "react";
import styled, { useTheme } from "styled-components";
import { Controller } from "react-hook-form";

import Button from "components/button/Button";
import Input from "components/input/Input";
import Select from "components/select/Select";
import OptionsButton from "components/button/OptionsButton";
import { ACTION_OPTIONS, DURATION_OPTIONS, STATUS_OPTIONS } from "pages/tags/types";

type Props = {
  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  control: any;
  handleSubmit: () => void;
  handleCancel: () => void;
  buttonTitle?: string;
};

const AddTags: FC<Props> = ({ control, handleSubmit, handleCancel, buttonTitle = "Apply" }) => {
  const theme = useTheme();

  return (
    <Container>
      <Field>
        <Controller
          name="name"
          rules={{ required: true }}
          control={control}
          render={({ field: { ref, ...rest } }) => (
            <Input label="Tag name" placeholder="Input tag name" inputRef={ref} {...rest} />
          )}
        />
      </Field>
      <Field>
        <Controller
          name="status"
          control={control}
          render={({ field: { ref, ...rest } }) => (
            <Select label="Status" {...rest} options={STATUS_OPTIONS} />
          )}
        />
      </Field>
      <Field>
        <Controller
          name="action"
          control={control}
          render={({ field: { ref, ...rest } }) => (
            <Select label="Action" {...rest} options={ACTION_OPTIONS} />
          )}
        />
      </Field>
      <Field>
        <Controller
          name="value"
          rules={{ required: true, min: 0 }}
          control={control}
          render={({ field: { ref, ...rest } }) => (
            <Input label="Value" placeholder="Input value" inputRef={ref} {...rest} />
          )}
        />
      </Field>
      <Field>
        <Controller
          name="lifetimeSeconds"
          control={control}
          render={({ field: { ref, ...rest } }) => (
            <Select label="Duration" {...rest} options={DURATION_OPTIONS} />
          )}
        />
      </Field>
      <ButtonsContainer>
        <Button onClick={handleSubmit}>{buttonTitle}</Button>
        <OptionsButton paintTO={theme.colors.error} onClick={handleCancel} iconType="close" />
      </ButtonsContainer>
    </Container>
  );
};

const Container = styled.form`
  display: flex;
  justify-content: space-between;
  align-items: flex-end;
  width: 100%;

  button {
    &:last-child {
      i {
        display: flex;
        justify-content: center;
      }
    }
  }
`;

const Field = styled.div`
  width: 17%;
`;

const ButtonsContainer = styled.div`
  display: flex;
  align-items: flex-end;
  gap: 8px;
`;

export default AddTags;
