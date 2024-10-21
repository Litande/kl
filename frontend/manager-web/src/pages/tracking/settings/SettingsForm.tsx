import React, { useState } from "react";
import styled from "styled-components";
import { Controller, Control } from "react-hook-form";
import Input from "components/input/Input";
import MultiSelect, { IOption } from "components/multiSelect/MultiSelect";
import { IFormValues } from "./types";
import { GENERAL_TAB, FORM_TABS } from "./constants";

type Props = {
  control: Control<IFormValues>;
};

const SettingsForm = ({ control }: Props) => {
  const [activeTab, setActiveTab] = useState(GENERAL_TAB.name);
  const [statuses, setStatuses] = useState<IOption[]>([]);

  return (
    <Container>
      <TabsContainer>
        {FORM_TABS.map(tab => (
          <Tab
            isActive={activeTab === tab.name}
            key={tab.name}
            onClick={() => setActiveTab(tab.name)}
          >
            {tab.label}
          </Tab>
        ))}
      </TabsContainer>
      <FormContainer>
        <Row>
          <Controller
            name="endCallButtonDelay"
            control={control}
            render={({ field: { ref, ...rest } }) => (
              <StyledInput
                inputRef={ref}
                {...rest}
                label="Show end call button after this amount of seconds of call"
              />
            )}
          />
        </Row>
        <Row>
          <Controller
            name="maxRingDuration"
            control={control}
            render={({ field: { ref, ...rest } }) => (
              <StyledInput inputRef={ref} {...rest} label="Max allowed ringing duration" />
            )}
          />
        </Row>
        <Row>
          <Controller
            name="callTimeout"
            control={control}
            render={({ field: { ref, ...rest } }) => (
              <StyledInput inputRef={ref} {...rest} label="Call origination timeout" />
            )}
          />
        </Row>
        <Row>
          <Controller
            name="busyStatus"
            control={control}
            render={({ field: { ref, ...rest } }) => (
              <SelectContainer>
                <MultiSelect {...rest} options={statuses} label="Default busy status" />
              </SelectContainer>
            )}
          />
        </Row>
        <Row>
          <Controller
            name="errorStatus"
            control={control}
            render={({ field: { ref, ...rest } }) => (
              <SelectContainer>
                <MultiSelect {...rest} options={statuses} label="Default Error status" />
              </SelectContainer>
            )}
          />
        </Row>
        <Row>
          <Controller
            name="faxStatus"
            control={control}
            render={({ field: { ref, ...rest } }) => (
              <SelectContainer>
                <MultiSelect {...rest} options={statuses} label="Default fax status" />
              </SelectContainer>
            )}
          />
        </Row>
      </FormContainer>
    </Container>
  );
};

export default SettingsForm;

const Container = styled.div`
  display: flex;
`;
const TabsContainer = styled.div`
  display: flex;
  flex-direction: column;
  box-sizing: border-box;
  width: 150px;
  gap: 30px;
  padding: 0 20px 0 0;
`;
const Tab = styled.div<{ isActive: boolean }>`
  opacity: ${({ isActive }) => (isActive ? "1" : "0.6")};
  ${({ theme }) => theme.typography.subtitle2};
  cursor: pointer;

  &:hover {
    opacity: 1;
  }
`;

const FormContainer = styled.form`
  display: flex;
  flex-direction: column;
  width: calc(100% - 150px);
  gap: 16px;
`;

const StyledInput = styled(Input)`
  flex: 1;
`;

const Row = styled.div``;

const SelectContainer = styled.div`
  flex: 1;
  margin: 5px 0 0 0;
`;
