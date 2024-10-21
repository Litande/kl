import { ReactElement } from "react";
import styled from "styled-components";

function ErrorMessage({ children }: { children?: ReactElement | string }) {
  return <Wrapper>{children}</Wrapper>;
}

export default ErrorMessage;

const Wrapper = styled.span`
  position: absolute;
  top: 110%;
  color: ${({ theme }) => theme.colors.error};
  ${({ theme }) => theme.typography.body1};
`;
