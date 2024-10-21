import React, { useState, useEffect } from "react";
import styled from "styled-components";
import { Controller, useForm } from "react-hook-form";
import ModalPortal from "components/modalPortal/ModalPortal";
import Input from "components/input/Input";
import Button from "components/button/Button";
import MultiSelect, { IOption } from "components/multiSelect/MultiSelect";
import trackingApi from "services/api/tracking";
import * as z from "zod";
import { zodResolver } from "@hookform/resolvers/zod";
import useToggle from "hooks/useToggle";

type Props = {
  className?: string;
  onSave: () => void;
  onClose: () => void;
  id: number;
};

export interface IFormValues {
  userName: string;
  firstName: string;
  lastName: string;
  email: string;
  tags: IOption[];
  teamIds: IOption[];
  leadQueueIds: IOption[];
  newPassword?: string;
  confirmPassword?: string;
}

const defaultFields = {
  userName: "",
  firstName: "",
  lastName: "",
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
  teamIds: z.array(z.object({ value: z.number(), label: z.string() })).nonempty(),
  tags: z.array(z.object({ value: z.number(), label: z.string() })),
  leadQueueIds: z.array(z.object({ value: z.number(), label: z.string() })),
  newPassword: z.string().min(6).max(255).optional(),
  confirmPassword: z.string().min(6).max(255).optional(),
});
const EditAgentModal = ({ onSave, onClose, id }: Props) => {
  const { handleSubmit, control, reset, register, unregister } = useForm<IFormValues>({
    defaultValues: defaultFields,
    resolver: zodResolver(schema),
    shouldUseNativeValidation: true,
  });
  const [options, setOptions] = useState({
    teams: [],
    leads: [],
    tags: [],
  });
  const [isChangePasswordActive, toggleChangePasswordState] = useToggle();

  useEffect(() => {
    if (!isChangePasswordActive) {
      unregister(["confirmPassword", "newPassword"]);
    }
  }, [isChangePasswordActive]);

  useEffect(() => {
    Promise.all([
      trackingApi.getAgent({ id }),
      trackingApi.getTags(),
      trackingApi.getTeams(),
      trackingApi.getLeadsQueues(),
    ]).then(([{ data: agent }, { data: tags }, { data: teams }, { data: leads }]) => {
      reset({
        userName: agent.userName,
        firstName: agent.firstName,
        lastName: agent.lastName,
        email: agent.email,
        tags: agent.tags.map(({ id, name }) => ({ value: id, label: name })),
        teamIds: agent.teamIds
          .map(id => teams.items.find(({ teamId }) => teamId === id))
          .map(({ teamId, name }) => ({
            label: name,
            value: teamId,
          })),
        leadQueueIds: agent.leadQueueIds
          .map(id => leads.items.find(({ leadQueueId }) => leadQueueId === id))
          .map(({ leadQueueId, name }) => ({
            label: name,
            value: leadQueueId,
          })),
      });

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
    });
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  const submitForm = async (data: IFormValues) => {
    const {
      email,
      firstName,
      userName,
      lastName,
      leadQueueIds,
      tags,
      teamIds,
      newPassword,
      confirmPassword,
    } = data;

    const agentData = {
      id,
      userName,
      email,
      firstName,
      lastName,
      tagIds: tags.map(({ value }) => value),
      teamIds: teamIds.map(({ value }) => value),
      leadQueueIds: leadQueueIds.map(({ value }) => value),
    };
    await trackingApi.updateAgent(agentData);
    if (newPassword && confirmPassword && isChangePasswordActive) {
      await trackingApi.changeAgentPassword({
        id,
        data: {
          newPassword,
          confirmPassword,
        },
      });
    }
    onSave();
  };

  return (
    <StyledModal isOpen={true} handleClose={onClose}>
      <ModalHeader>
        Edit
        <CloseModal className="icon-close" onClick={onClose} />
      </ModalHeader>
      <ModalContent>
        <FormContainer onSubmit={handleSubmit(submitForm)}>
          <Row>
            <Controller
              name="userName"
              control={control}
              render={({ field }) => <StyledInput {...field} label="Username" />}
            />
            <Controller
              name="firstName"
              control={control}
              render={({ field }) => <StyledInput {...field} label="First Name" />}
            />
          </Row>
          <Row>
            <Controller
              name="email"
              control={control}
              render={({ field }) => <StyledInput {...field} label="Email" />}
            />
            <Controller
              name="lastName"
              control={control}
              render={({ field }) => <StyledInput {...field} label="Last Name" />}
            />
          </Row>
          <Row>
            <Controller
              name="tags"
              control={control}
              render={({ field: { ref, ...rest } }) => (
                <SelectContainer>
                  <MultiSelect {...rest} isMulti options={options.tags} label="Tags" />
                </SelectContainer>
              )}
            />
            <Flex />
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
          {isChangePasswordActive && (
            <Row>
              <StyledInput {...register("newPassword")} label="new password" />
              <StyledInput {...register("confirmPassword")} label="confirm password" />
            </Row>
          )}
          <Divider />
          <Row>
            <Button
              variant="secondary"
              onClick={e => {
                e.preventDefault();
                toggleChangePasswordState();
              }}
            >
              {isChangePasswordActive ? "Cancel change password" : "Change password"}
            </Button>
            <ApplyButton>Apply</ApplyButton>
          </Row>
        </FormContainer>
      </ModalContent>
    </StyledModal>
  );
};

export default EditAgentModal;

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
  margin: 0;
`;

const SelectContainer = styled.div`
  flex: 1;
  margin: 5px 0 0 0;
`;

const Flex = styled.div`
  flex: 1;
`;

const Divider = styled.div`
  width: 100%;
  height: 1px;
  background: ${({ theme }) => theme.colors.bg.divider};
  margin: 0 0 16px;
`;
