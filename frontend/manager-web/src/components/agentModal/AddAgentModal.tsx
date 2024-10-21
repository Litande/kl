import React, { useState, useEffect } from "react";
import styled from "styled-components";
import { Controller, useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import ModalPortal from "components/modalPortal/ModalPortal";
import Input from "components/input/Input";
import Button from "components/button/Button";
import MultiSelect, { IOption } from "components/multiSelect/MultiSelect";
import trackingApi from "services/api/tracking";
import * as z from "zod";

type Props = {
  className?: string;
  onSave: () => void;
  onClose: () => void;
};

export interface IFormValues {
  userName: string;
  firstName: string;
  lastName: string;
  password: string;
  email: string;
  tags: IOption[];
  teamIds: IOption[];
  leadQueueIds: IOption[];
}

const defaultFields = {
  userName: "",
  firstName: "",
  lastName: "",
  password: "",
  email: "",
  tags: [],
  teamIds: [],
  leadQueueIds: [],
};

const schema = z.object({
  userName: z.string().min(2).max(255),
  firstName: z.string().min(2).max(255),
  lastName: z.string().min(2).max(255),
  email: z.string({ required_error: "Required" }).email(),
  password: z.string().min(6).max(255),
  teamIds: z.array(z.object({ value: z.number(), label: z.string() })).nonempty(),
  tags: z.array(z.object({ value: z.number(), label: z.string() })),
  leadQueueIds: z.array(z.object({ value: z.number(), label: z.string() })),
});

const AddAgentModal = ({ onSave, onClose }: Props) => {
  const { handleSubmit, control, register } = useForm<IFormValues>({
    defaultValues: defaultFields,
    resolver: zodResolver(schema),
    shouldUseNativeValidation: true,
  });
  const [options, setOptions] = useState({
    teams: [],
    leads: [],
    tags: [],
  });

  useEffect(() => {
    Promise.all([trackingApi.getTags(), trackingApi.getTeams(), trackingApi.getLeadsQueues()]).then(
      ([{ data: tags }, { data: teams }, { data: leads }]) => {
        setOptions({
          tags: tags.items.map(({ id, name }) => ({ label: name, value: id })),
          teams: teams.items.map(({ teamId, name }) => ({
            label: name,
            value: teamId,
          })),
          leads: leads.items.map(({ leadQueueId, name }) => ({
            label: name,
            value: leadQueueId,
          })),
        });
      }
    );
  }, []);

  const submitForm = (data: IFormValues) => {
    const { password, email, firstName, userName, lastName, leadQueueIds, tags, teamIds } = data;

    const agentData = {
      userName,
      email,
      firstName,
      lastName,
      password,
      tagIds: tags?.map(({ value }) => value),
      teamIds: teamIds?.map(({ value }) => value),
      leadQueueIds: leadQueueIds?.map(({ value }) => value),
    };
    trackingApi.createAgent(agentData).then(() => {
      onSave();
    });
  };

  return (
    <StyledModal isOpen handleClose={onClose}>
      <ModalHeader>
        New agent
        <CloseModal className="icon-close" onClick={onClose} />
      </ModalHeader>
      <ModalContent>
        <FormContainer onSubmit={handleSubmit(submitForm)}>
          <Row>
            <Controller
              name="userName"
              control={control}
              render={({ field: { ref, ...rest } }) => (
                <StyledInput {...register("userName")} {...rest} label="Username" />
              )}
            />
            <Controller
              name="firstName"
              control={control}
              render={({ field: { ref, ...rest } }) => (
                <StyledInput {...register("firstName")} {...rest} label="First Name" />
              )}
            />
          </Row>
          <Row>
            <Controller
              name="email"
              control={control}
              render={({ field: { ref, ...rest } }) => (
                <StyledInput {...register("email")} {...rest} label="Email" />
              )}
            />
            <Controller
              name="lastName"
              control={control}
              render={({ field: { ref, ...rest } }) => (
                <StyledInput inputRef={ref} {...register("lastName")} {...rest} label="Last Name" />
              )}
            />
          </Row>
          <Row>
            <Controller
              name="password"
              control={control}
              render={({ field: { ref, ...rest } }) => (
                <StyledInput inputRef={ref} {...register("password")} {...rest} label="Password" />
              )}
            />
            <Controller
              name="tags"
              control={control}
              render={({ field: { ref, ...rest } }) => (
                <SelectContainer>
                  <MultiSelect {...rest} isMulti options={options.tags} label="Tags" />
                </SelectContainer>
              )}
            />
          </Row>
          <Row>
            <Controller
              name="teamIds"
              control={control}
              render={({ field: { ref, ...rest } }) => (
                <SelectContainer>
                  <SelectLabel>Team</SelectLabel>
                  <MultiSelect {...rest} options={options.teams} isMulti placeholder="Select..." />
                </SelectContainer>
              )}
            />
            <Controller
              name="leadQueueIds"
              control={control}
              render={({ field: { ref, ...rest } }) => (
                <SelectContainer>
                  <SelectLabel>In the Leads Group</SelectLabel>
                  <MultiSelect {...rest} options={options.leads} isMulti placeholder="Select..." />
                </SelectContainer>
              )}
            />
          </Row>
          <Divider />
          <Row>
            <ApplyButton>Apply</ApplyButton>
          </Row>
        </FormContainer>
      </ModalContent>
    </StyledModal>
  );
};

export default AddAgentModal;

const StyledModal = styled(ModalPortal)`
  width: 580px;
  background: ${({ theme }) => theme.colors.bg.ternary};
  box-shadow: 0 4px 8px ${({ theme }) => theme.colors.fg.secondary_light_disabled};
  border-radius: 6px;
  opacity: 1;
`;

const ModalHeader = styled.div`
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 16px 20px 16px 25px;
  background: ${({ theme }) => theme.colors.bg.primary};
  border-radius: 6px 6px 0 0;
  text-transform: uppercase;
  ${({ theme }) => theme.typography.subtitle3};
  color: ${({ theme }) => theme.colors.btn.primary};
`;

const CloseModal = styled.i`
  cursor: pointer;
`;

const ModalContent = styled.div``;

const FormContainer = styled.form`
  display: flex;
  flex-direction: row;
  flex-wrap: wrap;
  padding: 16px 0 0;
  border-bottom: 1px solid ${({ theme }) => theme.colors.border.primary};
`;

const StyledInput = styled(Input)`
  flex: 1;
`;

const Row = styled.div`
  display: flex;
  justify-content: space-between;
  width: 100%;
  gap: 20px;
  padding: 0 16px 16px;
`;

const SelectLabel = styled.div`
  ${({ theme }) => theme.typography.subtitle2};
  color: black;
  padding: 0 0 15px;
`;

const ApplyButton = styled(Button)`
  margin: 0 auto;
`;

const SelectContainer = styled.div`
  flex: 1;
  margin: 5px 0 0 0;
`;

const Divider = styled.div`
  width: 100%;
  height: 1px;
  background: ${({ theme }) => theme.colors.bg.divider};
  margin: 0 0 16px;
`;
