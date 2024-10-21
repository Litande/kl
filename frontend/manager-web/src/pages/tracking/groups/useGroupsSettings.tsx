import Button from "components/button/Button";
import Input from "components/input/Input";
import Modal from "components/modal/ModalV2";
import MultiSelect from "components/multiSelect/MultiSelect";
import useToggle from "hooks/useToggle";
import { Controller, useForm } from "react-hook-form";
import trackingApi from "services/api/tracking";
import styled from "styled-components";
import { isObject } from "lodash";

type DropRateParameters = {
  dropRateUpperThreshold: number;
  dropRateLowerThreshold: number;
  dropRatePeriod: number;
  ratioStep: number;
  maxRatio: number | null;
  minRatio: number | null;
  ratioFreezeTime: number;
};

type IFormValues = {
  dropRateUpperThreshold: number;
  dropRateLowerThreshold: number;
  dropRatePeriod: { label: string; value: number };
  ratioStep: { label: string; value: number };
  maxRatio: { label: string; value: number } | null;
  minRatio: { label: string; value: number } | null;
};

const dropRatePeriods = [
  { label: "1 minute", value: 60 },
  { label: "5 minutes", value: 300 },
  { label: "10 minutes", value: 600 },
  { label: "30 minute", value: 1800 },
  { label: "1 hour", value: 3600 },
];

const ratiStepOptions = [
  { label: 1, value: 1 },
  { label: 2, value: 2 },
  { label: 3, value: 3 },
  { label: 4, value: 4 },
  { label: 5, value: 5 },
  { label: 6, value: 6 },
  { label: 7, value: 7 },
  { label: 8, value: 8 },
  { label: 9, value: 9 },
  { label: 10, value: 10 },
];

const normalizeValues = (params: DropRateParameters) => {
  return Object.entries(params).reduce((acc, [key, value]) => {
    if (key === "dropRatePeriod") {
      acc[key] = { label: value / 60 + " minutes", value: value };
      return acc;
    }
    if (key === "ratioStep" || key === "minRatio" || key === "maxRatio") {
      acc[key] = { label: value, value: value };
      return acc;
    }
    acc[key] = value;
    return acc;
  }, {});
};

export default function useGroupsSettings({
  queueId,
  dropRateParameters,
  groupName,
}: {
  queueId;
  dropRateParameters: DropRateParameters;
  groupName: string;
}) {
  const [isOpen, toggle] = useToggle();
  const { handleSubmit, control } = useForm<IFormValues>({
    defaultValues: normalizeValues(dropRateParameters),
  });

  const submitForm = async (data: IFormValues) => {
    const preparedData = Object.entries(data).reduce((acc, [key, prop]) => {
      acc[key] = isObject(prop) ? prop.value : prop;
      return acc;
    }, {}) as DropRateParameters;
    const ratioFreezeTime = Math.round(preparedData.dropRatePeriod / 3);
    await trackingApi.updateLeadQueue({ id: queueId, data: { ...preparedData, ratioFreezeTime } });
    toggle();
  };

  const groupSettingsModal = () => {
    return (
      isOpen && (
        <StyledModal onCancel={toggle} title="Drop Rate Parameters" hasCloseIcon>
          <ModalContent onSubmit={handleSubmit(submitForm)}>
            <Label>{groupName}</Label>
            <Field>
              <Controller
                name="dropRatePeriod"
                control={control}
                render={({ field: { ref, ...rest } }) => {
                  return (
                    <MultiSelect
                      {...rest}
                      value={[rest.value]}
                      onChange={([value]) => rest.onChange(value)}
                      options={dropRatePeriods}
                      label="Time Frame"
                    />
                  );
                }}
              />
            </Field>
            <DropRateBlock>
              <Label>Drop rate</Label>
              <Field>
                <Controller
                  name="dropRateUpperThreshold"
                  control={control}
                  render={({ field: { ref, ...rest } }) => {
                    return (
                      <Input
                        {...rest}
                        type="number"
                        inputIcon={<InputIcon>{">="}</InputIcon>}
                        labelComponent={<DropRateInputLabel>Ratio Increasing</DropRateInputLabel>}
                      />
                    );
                  }}
                />
                <Controller
                  name="dropRateLowerThreshold"
                  control={control}
                  render={({ field: { ref, ...rest } }) => {
                    return (
                      <Input
                        {...rest}
                        type="number"
                        inputIcon={<InputIcon>{"<="}</InputIcon>}
                        labelComponent={<DropRateInputLabel>Ratio Decreasing</DropRateInputLabel>}
                      />
                    );
                  }}
                />
              </Field>
            </DropRateBlock>
            <Field>
              <Controller
                name="ratioStep"
                control={control}
                render={({ field: { ref, ...rest } }) => {
                  return (
                    <MultiSelect
                      {...rest}
                      value={[rest.value]}
                      onChange={([value]) => rest.onChange(value)}
                      options={ratiStepOptions}
                      label="Ratio Step"
                    />
                  );
                }}
              />
            </Field>
            <Field>
              <Controller
                name="minRatio"
                control={control}
                render={({ field: { ref, ...rest } }) => {
                  return (
                    <MultiSelect
                      {...rest}
                      value={[rest.value]}
                      onChange={([value]) => rest.onChange(value)}
                      options={ratiStepOptions}
                      label="Min Ratio"
                    />
                  );
                }}
              />
              <Controller
                name="maxRatio"
                control={control}
                render={({ field: { ref, ...rest } }) => {
                  return (
                    <MultiSelect
                      {...rest}
                      value={[rest.value]}
                      onChange={([value]) => rest.onChange(value)}
                      options={ratiStepOptions}
                      label="Max Ratio"
                    />
                  );
                }}
              />
            </Field>
            <ButtonWrap>
              <StyledButton>Apply</StyledButton>
            </ButtonWrap>
          </ModalContent>
        </StyledModal>
      )
    );
  };

  return { groupSettingsModal, toggleModal: toggle };
}

const StyledModal = styled(Modal)`
  width: 300px;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.16);
  border-radius: 4px;
  background: ${({ theme }) => theme.colors.bg.ternary};
`;

const ModalContent = styled.form`
  width: 100%;
  box-sizing: border-box;
  background-color: white;
  display: flex;
  justify-content: center;
  flex-direction: column;
  gap: 0.7rem;
`;
const ButtonWrap = styled.div`
  width: 100%;
  display: flex;
  padding: 30px 0 0;
  gap: 16px;
`;
const StyledButton = styled(Button)`
  width: 100%;
  text-transform: uppercase;
`;

const Field = styled.div`
  display: flex;
  gap: 1rem;
  position: relative;
  box-sizing: border-box;
  width: 100%;
`;

const DropRateBlock = styled.div`
  position: relative;
  display: flex;
  flex-direction: column;
  width: 100%;
  padding: 1rem 0;

  ::before {
    position: absolute;
    content: " ";
    width: 268px;
    height: 1px;
    background: rgba(0, 0, 0, 0.12);
    top: 0;
  }

  ::after {
    bottom: 0;
    position: absolute;
    content: " ";
    width: 268px;
    height: 1px;
    background: rgba(0, 0, 0, 0.12);
  }
`;

const Label = styled.div`
  padding: 0 0 6px 10px;
  ${({ theme }) => theme.typography.smallText1}
  color: ${({ theme }) => theme.colors.fg.secondary_light};
  text-transform: uppercase;
`;

const DropRateInputLabel = styled.span`
  padding: 0 0 6px 10px;
  ${({ theme }) => theme.typography.smallText1}
  color: ${({ theme }) => theme.colors.fg.secondary};
`;

const InputIcon = styled.i`
  position: absolute;
  left: 0.5rem;
  top: calc(50% - 0.5rem);
  z-index: 1;
  letter-spacing: 3px;
`;
