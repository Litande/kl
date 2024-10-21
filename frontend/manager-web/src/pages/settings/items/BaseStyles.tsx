import styled from "styled-components";
import { typography } from "globalStyles/theme/fonts";

export const HelperTextWrap = styled.div`
  width: 110px;
  text-transform: uppercase;
  ${typography.helperText}
`;

export const Wrap = styled.div`
  padding: 1rem;
  display: flex;
  flex-direction: row;
  gap: 2rem;
  align-items: flex-end;
`;

export const ContentWrap = styled.div`
  display: flex;
  flex-direction: column;
`;

export const Form = styled.div`
  width: 100%;
  display: flex;
  flex-direction: row;
  align-items: flex-end;
  gap: 2rem;
`;

export const FormContent = styled.div`
  display: flex;
  flex-direction: column;
  width: calc(100% - 200px - 2rem);
  gap: 1rem;
`;

export const Row = styled.div`
  width: 100%;
  display: flex;
  align-items: flex-end;
  gap: 2rem;
`;

export const Field = styled.div`
  flex: 1;
`;

export const FormActions = styled.div`
  width: 200px;
  display: flex;
  flex-direction: row;
  gap: 1rem;
`;
